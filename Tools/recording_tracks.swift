// Writes a short movie with the audio tracks laid out as HoloKit's recorder lays them out
// (Packages/com.holoi.xr.holokit/Sources~/holokit/VideoRecorder.mm): an MPEG-4 file with an H.264 video track and three
// AAC tracks, the mix, the game and the microphone, added in that order and grouped as alternates with the mix as the
// default. Each audio track is a tone of its own (mix 440 Hz, game 660 Hz, microphone 880 Hz), so whatever a player or
// an exporter keeps can be told apart by frequency (Tools/check_recording_playback.py).
//
// Usage: xcrun swift Tools/recording_tracks.swift OUT.mp4
import AVFoundation
import CoreMedia
import Foundation

let sampleRate = 48000.0
let channels = 2
let seconds = 2.0
let tones = [440.0, 660.0, 880.0]  // mix, game, microphone

func fail(_ message: String) -> Never {
    FileHandle.standardError.write((message + "\n").data(using: .utf8)!)
    exit(1)
}

// Waits until an input takes more data; a writer that stops taking data is an error, not a hang.
func waitUntilReady(_ input: AVAssetWriterInput, _ name: String, _ writer: AVAssetWriter) {
    var waited = 0
    while !input.isReadyForMoreMediaData {
        if writer.status == .failed { fail("\(name): \(String(describing: writer.error))") }
        waited += 1
        if waited > 5000 { fail("\(name) took no data for 5 s; writer status \(writer.status.rawValue)") }
        usleep(1000)
    }
}

func audioBuffer(frequency: Double, start: Int, frames: Int, format: CMAudioFormatDescription) -> CMSampleBuffer {
    var samples = [Float](repeating: 0, count: frames * channels)
    for frame in 0..<frames {
        let value = Float(0.3 * sin(2 * Double.pi * frequency * Double(start + frame) / sampleRate))
        for channel in 0..<channels { samples[frame * channels + channel] = value }
    }
    let bytes = samples.count * MemoryLayout<Float>.size
    var block: CMBlockBuffer?
    guard CMBlockBufferCreateWithMemoryBlock(allocator: kCFAllocatorDefault, memoryBlock: nil, blockLength: bytes,
                                             blockAllocator: kCFAllocatorDefault, customBlockSource: nil,
                                             offsetToData: 0, dataLength: bytes, flags: 0,
                                             blockBufferOut: &block) == noErr, let block else { fail("block buffer") }
    samples.withUnsafeBytes { raw in
        _ = CMBlockBufferReplaceDataBytes(with: raw.baseAddress!, blockBuffer: block, offsetIntoDestination: 0,
                                          dataLength: bytes)
    }
    var buffer: CMSampleBuffer?
    let time = CMTime(value: CMTimeValue(start), timescale: CMTimeScale(sampleRate))
    guard CMAudioSampleBufferCreateReadyWithPacketDescriptions(allocator: kCFAllocatorDefault, dataBuffer: block,
                                                               formatDescription: format, sampleCount: frames,
                                                               presentationTimeStamp: time,
                                                               packetDescriptions: nil,
                                                               sampleBufferOut: &buffer) == noErr,
          let buffer else { fail("sample buffer") }
    return buffer
}

guard CommandLine.arguments.count == 2 else { fail("usage: recording_tracks.swift OUT.mp4") }
let url = URL(fileURLWithPath: CommandLine.arguments[1])
try? FileManager.default.removeItem(at: url)
guard let writer = try? AVAssetWriter(outputURL: url, fileType: .mp4) else { fail("cannot write \(url.path)") }

let size = 64
let video = AVAssetWriterInput(mediaType: .video, outputSettings: [
    AVVideoCodecKey: AVVideoCodecType.h264, AVVideoWidthKey: size, AVVideoHeightKey: size])
let adaptor = AVAssetWriterInputPixelBufferAdaptor(assetWriterInput: video, sourcePixelBufferAttributes: [
    kCVPixelBufferPixelFormatTypeKey as String: kCVPixelFormatType_32BGRA,
    kCVPixelBufferWidthKey as String: size, kCVPixelBufferHeightKey as String: size])
let audioSettings: [String: Any] = [
    AVFormatIDKey: kAudioFormatMPEG4AAC, AVSampleRateKey: sampleRate, AVNumberOfChannelsKey: channels,
    AVEncoderAudioQualityKey: AVAudioQuality.high.rawValue]
let audio = tones.map { _ in AVAssetWriterInput(mediaType: .audio, outputSettings: audioSettings) }
writer.add(video)
for input in [video] + audio { input.expectsMediaDataInRealTime = true }  // as VideoRecorder.mm
for input in audio { writer.add(input) }
let group = AVAssetWriterInputGroup(inputs: audio, defaultInput: audio[0])
guard writer.canAdd(group) else { fail("cannot group the audio tracks") }
writer.add(group)

var description = AudioStreamBasicDescription(
    mSampleRate: sampleRate, mFormatID: kAudioFormatLinearPCM, mFormatFlags: kLinearPCMFormatFlagIsFloat,
    mBytesPerPacket: UInt32(channels * 4), mFramesPerPacket: 1, mBytesPerFrame: UInt32(channels * 4),
    mChannelsPerFrame: UInt32(channels), mBitsPerChannel: 32, mReserved: 0)
var format: CMAudioFormatDescription?
guard CMAudioFormatDescriptionCreate(allocator: kCFAllocatorDefault, asbd: &description, layoutSize: 0, layout: nil,
                                     magicCookieSize: 0, magicCookie: nil, extensions: nil,
                                     formatDescriptionOut: &format) == noErr, let format else { fail("format") }

guard writer.startWriting() else { fail("start: \(String(describing: writer.error))") }
writer.startSession(atSourceTime: .zero)

// Append in time order, a slice at a time, so the writer can interleave the tracks.
let fps = 30
let chunk = 1024
let total = Int(seconds * sampleRate)
var frame = 0
var start = 0
while start < total {
    let frames = min(chunk, total - start)
    for (input, frequency) in zip(audio, tones) {
        waitUntilReady(input, "audio \(frequency) Hz", writer)
        if !input.append(audioBuffer(frequency: frequency, start: start, frames: frames, format: format)) {
            fail("audio \(frequency) Hz append: \(String(describing: writer.error))")
        }
    }
    start += frames
    while frame < Int(seconds) * fps && Double(frame) / Double(fps) < Double(start) / sampleRate {
        waitUntilReady(video, "video", writer)
        var pixels: CVPixelBuffer?
        CVPixelBufferPoolCreatePixelBuffer(nil, adaptor.pixelBufferPool!, &pixels)
        guard let pixels else { fail("pixel buffer") }
        adaptor.append(pixels, withPresentationTime: CMTime(value: CMTimeValue(frame), timescale: CMTimeScale(fps)))
        frame += 1
    }
}
video.markAsFinished()
for input in audio { input.markAsFinished() }

let done = DispatchSemaphore(value: 0)
writer.finishWriting { done.signal() }
done.wait()
guard writer.status == .completed else { fail("finish: \(String(describing: writer.error))") }
print(url.path)
