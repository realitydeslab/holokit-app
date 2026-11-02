// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Botao 'Amber' Hu <amber@reality.design>
// SPDX-License-Identifier: MIT

#import "VideoRecorder.h"

#include <TargetConditionals.h>

#if TARGET_OS_IOS
#import <UIKit/UIKit.h>
#endif

#import <AVFoundation/AVFoundation.h>
#import <Metal/Metal.h>
#import <CoreMedia/CMBlockBuffer.h>
#import <Accelerate/Accelerate.h>
#import <AudioToolbox/AudioToolbox.h>
#include <os/lock.h>
#include <algorithm>
#include <vector>

// The microphone may run ahead of the game sound by at most this much; older samples are dropped.
static const double MicrophoneBufferSeconds = 0.5;

@interface VideoRecorder ()

@property (nonatomic, strong) AVAssetWriter* writer;
@property (nonatomic, strong) AVAssetWriterInput* videoWriterInput;
// The game sound alone (Unity's mix). With the microphone on, two more tracks: the game and the microphone mixed
// (the default track, written first) and the microphone alone.
@property (nonatomic, strong) AVAssetWriterInput* audioWriterInput;
@property (nonatomic, strong) AVAssetWriterInput* mixWriterInput;
@property (nonatomic, strong) AVAssetWriterInput* micWriterInput;
@property (nonatomic, strong) AVAssetWriterInputPixelBufferAdaptor* bufferAdaptor;
@property (assign) AudioStreamBasicDescription audioFormat;
@property (assign) CMFormatDescriptionRef cmFormat;
@property (assign) bool isRecording;
@property (assign) int width;
@property (assign) int height;
@property (assign) bool recordMicrophone;
@property (nonatomic, strong) AVAudioEngine* audioEngine;
@property (nonatomic, strong) AVAudioConverter* micConverter;

@end

@implementation VideoRecorder
{
    // Microphone samples converted to the game sound's format, interleaved, waiting for the game sound to pair with.
    std::vector<float> _micRing;
    size_t _micStart;
    size_t _micCount;
    os_unfair_lock _micLock;
    std::vector<float> _micScratch;
    std::vector<float> _mixScratch;
}

- (instancetype)init {
    if (self = [super init]) {
        _micLock = OS_UNFAIR_LOCK_INIT;
        _micStart = 0;
        _micCount = 0;
    }
    return self;
}

+ (id)sharedInstance {
    static dispatch_once_t onceToken = 0;
    static id _sharedObject = nil;
    dispatch_once(&onceToken, ^{
        _sharedObject = [[self alloc] init];
    });
    return _sharedObject;
}

- (void)setMicrophoneRequested:(bool)requested
{
    self.recordMicrophone = requested;
}

/// Starts capturing the microphone, converted to the game sound's format. False when the microphone is off, not
/// permitted or unavailable: the recording then has the game sound alone.
- (bool)startMicrophoneWithSampleRate:(double)sampleRate channels:(int)channels
{
    if (!self.recordMicrophone || sampleRate <= 0 || channels <= 0)
        return false;
    if ([AVAudioApplication sharedInstance].recordPermission != AVAudioApplicationRecordPermissionGranted)
    {
        NSLog(@"[VideoRecorder] Microphone permission not granted; recording the game sound only.");
        return false;
    }
    AVAudioEngine* engine = [[AVAudioEngine alloc] init];
    AVAudioInputNode* input = engine.inputNode;
    AVAudioFormat* inputFormat = [input outputFormatForBus:0];
    if (inputFormat.sampleRate <= 0 || inputFormat.channelCount == 0)
    {
        NSLog(@"[VideoRecorder] No microphone input (%@); recording the game sound only.", inputFormat);
        return false;
    }
    AVAudioFormat* gameFormat = [[AVAudioFormat alloc] initWithCommonFormat:AVAudioPCMFormatFloat32
                                                                 sampleRate:sampleRate
                                                                   channels:(AVAudioChannelCount)channels
                                                                interleaved:YES];
    AVAudioConverter* converter = [[AVAudioConverter alloc] initFromFormat:inputFormat toFormat:gameFormat];
    if (converter == nil)
    {
        NSLog(@"[VideoRecorder] Cannot convert the microphone from %@ to %@.", inputFormat, gameFormat);
        return false;
    }
    if (inputFormat.channelCount == 1 && channels > 1)
    {
        // A mono microphone goes to every channel; the converter would otherwise leave the others silent.
        NSMutableArray<NSNumber*>* map = [NSMutableArray array];
        for (int i = 0; i < channels; i++) [map addObject:@0];
        converter.channelMap = map;
    }
    os_unfair_lock_lock(&_micLock);
    _micRing.assign((size_t)(sampleRate * MicrophoneBufferSeconds) * channels, 0.f);
    _micStart = 0;
    _micCount = 0;
    os_unfair_lock_unlock(&_micLock);
    self.micConverter = converter;

    __weak VideoRecorder* weakSelf = self;
    [input installTapOnBus:0 bufferSize:1024 format:inputFormat block:^(AVAudioPCMBuffer* buffer, AVAudioTime* when) {
        [weakSelf receiveMicrophone:buffer];
    }];
    NSError* error = nil;
    [engine prepare];
    if (![engine startAndReturnError:&error])
    {
        NSLog(@"[VideoRecorder] Cannot start the microphone (%@); recording the game sound only.", error);
        [input removeTapOnBus:0];
        self.micConverter = nil;
        return false;
    }
    self.audioEngine = engine;
    NSLog(@"[VideoRecorder] Recording the microphone (%@) as its own track and in the mix.", inputFormat);
    return true;
}

- (void)stopMicrophone
{
    if (self.audioEngine)
    {
        [self.audioEngine.inputNode removeTapOnBus:0];
        [self.audioEngine stop];
    }
    self.audioEngine = nil;
    self.micConverter = nil;
    os_unfair_lock_lock(&_micLock);
    _micStart = 0;
    _micCount = 0;
    os_unfair_lock_unlock(&_micLock);
}

- (void)receiveMicrophone:(AVAudioPCMBuffer*)buffer
{
    AVAudioConverter* converter = self.micConverter;
    if (converter == nil || buffer.frameLength == 0)
        return;
    double ratio = converter.outputFormat.sampleRate / converter.inputFormat.sampleRate;
    AVAudioFrameCount capacity = (AVAudioFrameCount)ceil(buffer.frameLength * ratio) + 64;
    AVAudioPCMBuffer* converted = [[AVAudioPCMBuffer alloc] initWithPCMFormat:converter.outputFormat frameCapacity:capacity];
    __block bool supplied = false;
    NSError* error = nil;
    AVAudioConverterOutputStatus status = [converter convertToBuffer:converted error:&error
        withInputFromBlock:^AVAudioBuffer* (AVAudioPacketCount count, AVAudioConverterInputStatus* inputStatus) {
            if (supplied)
            {
                *inputStatus = AVAudioConverterInputStatus_NoDataNow;
                return nil;
            }
            supplied = true;
            *inputStatus = AVAudioConverterInputStatus_HaveData;
            return buffer;
        }];
    if (status == AVAudioConverterOutputStatus_Error || converted.frameLength == 0)
        return;
    // Interleaved: one buffer holds every channel.
    const float* samples = converted.floatChannelData[0];
    size_t count = (size_t)converted.frameLength * converter.outputFormat.channelCount;
    os_unfair_lock_lock(&_micLock);
    size_t size = _micRing.size();
    if (size > 0)
    {
        for (size_t i = 0; i < count; i++)
        {
            if (_micCount == size)
            {
                // Full: the oldest sample goes, so the microphone never lags the game sound by more than the ring.
                _micStart = (_micStart + 1) % size;
                _micCount--;
            }
            _micRing[(_micStart + _micCount) % size] = samples[i];
            _micCount++;
        }
    }
    os_unfair_lock_unlock(&_micLock);
}

/// Takes up to count microphone samples; the rest of out is silence (the microphone had not delivered them yet).
- (void)takeMicrophoneSamples:(float*)out count:(size_t)count
{
    os_unfair_lock_lock(&_micLock);
    size_t size = _micRing.size();
    size_t taken = std::min(count, _micCount);
    for (size_t i = 0; i < taken; i++)
        out[i] = _micRing[(_micStart + i) % size];
    if (taken > 0)
    {
        _micStart = (_micStart + taken) % size;
        _micCount -= taken;
    }
    os_unfair_lock_unlock(&_micLock);
    std::fill(out + taken, out + count, 0.f);
}

- (AVAssetWriterInput*)audioInputWithSettings:(NSDictionary*)settings
{
    AVAssetWriterInput* input = [AVAssetWriterInput assetWriterInputWithMediaType:AVMediaTypeAudio outputSettings:settings];
    input.expectsMediaDataInRealTime = YES;
    return input;
}

- (int)startRecording:(const char *)filePath width:(int)width height:(int)height audioSampleRate:(float)audioSampleRate audioChannelCount:(int)audioChannelCount
{
    if (self.writer)
    {
        NSLog(@"Recording has already been initiated.");
        return -1;
    }
    
    // Asset writer setup
    NSURL* filePathURL = [NSURL fileURLWithPath:[NSString stringWithUTF8String:filePath]];
    
    NSError* err;
    self.writer = [[AVAssetWriter alloc] initWithURL: filePathURL
                                        fileType: AVFileTypeMPEG4
                                           error: &err];
    
    if (err)
    {
        NSLog(@"Failed to initialize AVAssetWriter (%@)", err);
        return -1;
    }
    
    NSDictionary* settings = @{ 
        AVVideoCodecKey: AVVideoCodecTypeHEVC,
        AVVideoWidthKey: @(width),
        AVVideoHeightKey: @(height) };
    
    self.videoWriterInput = [AVAssetWriterInput assetWriterInputWithMediaType: AVMediaTypeVideo outputSettings: settings];
    self.videoWriterInput.expectsMediaDataInRealTime = YES;

    // Pixel buffer adaptor setup
    NSDictionary* attribs = @{ 
        (NSString*)kCVPixelBufferPixelFormatTypeKey: @(kCVPixelFormatType_32BGRA),
        (NSString*)kCVPixelBufferWidthKey: @(width),
        (NSString*)kCVPixelBufferHeightKey: @(height) 
    };
    
    self.bufferAdaptor = [AVAssetWriterInputPixelBufferAdaptor
                      assetWriterInputPixelBufferAdaptorWithAssetWriterInput: self.videoWriterInput
                      sourcePixelBufferAttributes: attribs];
    
    // Audio writer input setup
    NSDictionary* audioSettings = @{
        AVFormatIDKey: @(kAudioFormatMPEG4AAC),
        AVSampleRateKey: @(audioSampleRate),
        AVNumberOfChannelsKey: @(audioChannelCount),
        AVEncoderAudioQualityKey: @(AVAudioQualityHigh)
    };
    self.audioWriterInput = [self audioInputWithSettings:audioSettings];
    bool microphone = [self startMicrophoneWithSampleRate:audioSampleRate channels:audioChannelCount];
    self.mixWriterInput = microphone ? [self audioInputWithSettings:audioSettings] : nil;
    self.micWriterInput = microphone ? [self audioInputWithSettings:audioSettings] : nil;

    AudioStreamBasicDescription audioFormat;
    audioFormat = {0};
    audioFormat.mSampleRate = audioSampleRate; // Sample rate, 44100Hz is CD quality
    audioFormat.mFormatID = kAudioFormatLinearPCM; // Specify the data format to be PCM
    audioFormat.mFormatFlags = kLinearPCMFormatFlagIsFloat; // Flags specific for the format
    audioFormat.mFramesPerPacket = 1; // Each packet contains one frame for PCM data
    audioFormat.mChannelsPerFrame = (uint32_t) audioChannelCount; // Set the number of channels
    audioFormat.mBitsPerChannel = sizeof(float) * 8; // Number of bits per channel, 32 for float
    audioFormat.mBytesPerFrame = (uint32_t) audioChannelCount * sizeof(float); // Bytes per frame
    audioFormat.mBytesPerPacket = audioFormat.mBytesPerFrame * audioFormat.mFramesPerPacket; // Bytes per packet
    audioFormat.mReserved = 0;
    self.audioFormat = audioFormat;
    
    CMFormatDescriptionRef cmFormat;
    OSStatus status = CMAudioFormatDescriptionCreate(kCFAllocatorDefault,
                                   &audioFormat,
                                   0,
                                   NULL,
                                   0,
                                   NULL,
                                   NULL,
                                   &cmFormat
                                   );
    
    if (status != noErr) {
        NSLog(@"CMAudioFormatDescriptionCreate hasn't been initiated.");
        return -1;
    }
    self.cmFormat = cmFormat;
   
    [self.writer addInput:self.videoWriterInput];
    if (microphone)
    {
        // Track order: the mix, the game, the microphone. As alternates with the mix as the default, players
        // (Photos, social apps) play the mix; editors still get the game and the microphone apart.
        [self.writer addInput:self.mixWriterInput];
        [self.writer addInput:self.audioWriterInput];
        [self.writer addInput:self.micWriterInput];
        AVAssetWriterInputGroup* group = [AVAssetWriterInputGroup
            assetWriterInputGroupWithInputs:@[self.mixWriterInput, self.audioWriterInput, self.micWriterInput]
                               defaultInput:self.mixWriterInput];
        if ([self.writer canAddInputGroup:group])
            [self.writer addInputGroup:group];
        else
            NSLog(@"[VideoRecorder] Cannot group the audio tracks; every track will play by default.");
    }
    else
    {
        [self.writer addInput:self.audioWriterInput];
    }

    // Recording start
    if (![self.writer startWriting])
    {
        NSLog(@"Failed to start (%ld: %@)", self.writer.status, self.writer.error);
        [self stopMicrophone];
        return -1;
    }

    self.width = width;
    self.height = height;
    [self.writer startSessionAtSourceTime:kCMTimeZero];
    self.isRecording = YES;  

    return 0;
}

- (int)appendAudioFrame:(void *)source size:(int)size time:(double)time
{
    if (!self.isRecording) {
        return -2;
    }

    if (!self.writer)
    {
        NSLog(@"Recording hasn't been initiated.");
        return -1;
    }

    if (self.mixWriterInput == nil)
        return [self appendAudio:source size:size time:time to:self.audioWriterInput];

    // The microphone samples that arrived since the last game buffer, as many as the game delivered, so all three
    // tracks share this buffer's timestamp.
    size_t count = (size_t)size / sizeof(float);
    _micScratch.resize(count);
    _mixScratch.resize(count);
    [self takeMicrophoneSamples:_micScratch.data() count:count];
    const float* game = (const float*)source;
    for (size_t i = 0; i < count; i++)
        _mixScratch[i] = std::max(-1.f, std::min(1.f, game[i] + _micScratch[i]));
    int mix = [self appendAudio:_mixScratch.data() size:size time:time to:self.mixWriterInput];
    int alone = [self appendAudio:source size:size time:time to:self.audioWriterInput];
    int mic = [self appendAudio:_micScratch.data() size:size time:time to:self.micWriterInput];
    return (mix == 0 && alone == 0 && mic == 0) ? 0 : -1;
}

- (int)appendAudio:(const void *)source size:(int)size time:(double)time to:(AVAssetWriterInput*)input
{
    if (!input.isReadyForMoreMediaData)
    {
        NSLog(@"Audio Writer is not ready.");
        return -1;
    }

    CMTime presentationTimestamp = CMTimeMakeWithSeconds(time, 240); // Adjust timescale as needed
    
    CMBlockBufferRef blockBuffer;
    OSStatus status = CMBlockBufferCreateWithMemoryBlock(kCFAllocatorDefault, NULL, size,
                                                         kCFAllocatorDefault, NULL, 0, size,
                                                         kCMBlockBufferAssureMemoryNowFlag, &blockBuffer);
    
    if (status != kCMBlockBufferNoErr) {
        NSLog(@"CMBlockBufferCreateWithMemoryBlock error");
        return -1;
    }
    
    status = CMBlockBufferReplaceDataBytes(source, blockBuffer, 0, size);
    
    if (status != kCMBlockBufferNoErr) {
        NSLog(@"CMBlockBufferReplaceDataBytes error");
        CFRelease(blockBuffer);
        return -1;
    }

    CMItemCount nSamples = size / self.audioFormat.mBytesPerFrame;
    
    CMSampleBufferRef sampleBuffer;
    status = CMAudioSampleBufferCreateReadyWithPacketDescriptions(kCFAllocatorDefault,
                                                                blockBuffer,
                                                                self.cmFormat,
                                                                nSamples,
                                                                presentationTimestamp,
                                                                NULL,
                                                                &sampleBuffer);
    
    if (status != noErr) {
        CFRelease(blockBuffer);
        return -1;
    }

    bool result = CMSampleBufferDataIsReady(sampleBuffer) && CMSampleBufferIsValid(sampleBuffer) &&
        [input appendSampleBuffer:sampleBuffer];
    if (!result) {
        NSLog(@"appendSampleBuffer error");
    }
    
    CFRelease(sampleBuffer);
    CFRelease(blockBuffer);
    return result ? 0 : -1;
}

- (int)appendVideoFrame:(void *)source size:(int)size time:(double)time
{
    if (!self.isRecording) {
        return -2;
    }

    if (!self.writer)
    {
        NSLog(@"Recording hasn't been initiated.");
        return -1;
    }
    
    if (!self.videoWriterInput.isReadyForMoreMediaData)
    {
        NSLog(@"Video Writer is not ready.");
        return -1;
    }
    
    if (!self.bufferAdaptor.pixelBufferPool)
    {
        NSLog(@"Video Writer pixelBufferPool is empty.");
        return -1;
    }

    // Buffer allocation
    CVPixelBufferRef pixelBuffer = nil;
    CVReturn ret = CVPixelBufferPoolCreatePixelBuffer(NULL, self.bufferAdaptor.pixelBufferPool, &pixelBuffer);
    
    if (ret != kCVReturnSuccess)
    {
        NSLog(@"Can't allocate a pixel buffer (%d)", ret);
        NSLog(@"%ld: %@", self.writer.status, self.writer.error);
        return -1;
    }
    
    // Buffer copy
    CVPixelBufferLockBaseAddress(pixelBuffer, 0);
    void* baseAddress = CVPixelBufferGetBaseAddress(pixelBuffer);
    int bytesPerRow = (int) CVPixelBufferGetBytesPerRow(pixelBuffer);
    int bufferSize = (int) CVPixelBufferGetDataSize(pixelBuffer);
   
    vImage_Buffer srcBuffer;
    srcBuffer.data = source;
    srcBuffer.height = self.height;
    srcBuffer.width = self.width;
    srcBuffer.rowBytes = self.width * 4;

    vImage_Buffer dstBuffer;
    dstBuffer.data = baseAddress;
    dstBuffer.height = self.height;
    dstBuffer.width = self.width;
    dstBuffer.rowBytes = bytesPerRow;

    // ARGB to BGRA
    // ARGB
    // 0 denotes the alpha channel, 1 the red channel, 2 the green channel, and 3 the blue channel.
    const uint8_t permuteMap[4] = { 0, 1, 2, 3 };
    assert(size == self.width * self.height * 4);
    vImagePermuteChannels_ARGB8888(&srcBuffer, &dstBuffer, permuteMap, kvImageNoFlags);
    CVPixelBufferUnlockBaseAddress(pixelBuffer, 0);

    // Buffer submission
    [self.bufferAdaptor appendPixelBuffer:pixelBuffer
                    withPresentationTime:CMTimeMakeWithSeconds(time, 240)];
    
    CVPixelBufferRelease(pixelBuffer);

    return 0;
}

- (void)endRecording
{
    if (!self.isRecording) {
        return;
    }

    if (!self.writer)
    {
        NSLog(@"Recording hasn't been initiated.");
        return;
    }
    
    [self stopMicrophone];
    [self.videoWriterInput markAsFinished];
    [self.mixWriterInput markAsFinished];
    [self.audioWriterInput markAsFinished];
    [self.micWriterInput markAsFinished];

#if TARGET_OS_IOS
    NSString* path = self.writer.outputURL.path;
    [self.writer finishWritingWithCompletionHandler: ^{
        if (UIVideoAtPathIsCompatibleWithSavedPhotosAlbum(path)) {
            UISaveVideoAtPathToSavedPhotosAlbum(path, nil, nil, nil);
        }
    }];
#else
    [self.writer finishWritingWithCompletionHandler: ^{}];
#endif
    self.writer = NULL;
    self.videoWriterInput = NULL;
    self.mixWriterInput = NULL;
    self.audioWriterInput = NULL;
    self.micWriterInput = NULL;
    self.bufferAdaptor = NULL;
    self.isRecording = NO;
}

@end


extern "C" {

int HoloKitVideoRecorder_StartRecording(const char* filePath, int width, int height,
                                        float audioSampleRate, int audioChannelCount) {
    return [[VideoRecorder sharedInstance] startRecording:filePath width:width height:height audioSampleRate:audioSampleRate audioChannelCount:audioChannelCount];
}

int HoloKitVideoRecorder_AppendAudioFrame(void* source, int size, double time)
{
    return [[VideoRecorder sharedInstance] appendAudioFrame:source size:size time:time];
}

int HoloKitVideoRecorder_AppendVideoFrame(void* source, int size, double time)
{
    return [[VideoRecorder sharedInstance] appendVideoFrame:source size:size time:time];
}

void HoloKitVideoRecorder_EndRecording(void)
{
    [[VideoRecorder sharedInstance] endRecording];
}

// Takes effect at the next start: with the microphone permitted, the recording gets the game + microphone mix (the
// default track), the game alone and the microphone alone.
void HoloKitVideoRecorder_SetRecordMicrophone(int enabled)
{
    [[VideoRecorder sharedInstance] setMicrophoneRequested:enabled != 0];
}

}
