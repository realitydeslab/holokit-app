// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

#import <CoreNFC/CoreNFC.h>
#import "holokit_sdk-Swift.h"
#import "HoloKitOptics.h"
#import "HoloKitProfile.h"
#import <CommonCrypto/CommonDigest.h>

void (*OnNFCSessionCompleted)(bool, float *) = NULL;

@interface NFCSessionController : NSObject

@end

@interface NFCSessionController () <NFCTagReaderSessionDelegate>

@property (nonatomic, strong) NFCTagReaderSession* readerSession;
@property (assign) bool success;
@property (assign) int holokitType;
@property (assign) float ipd;
@property (assign) float farClipPlane;
@property (nonatomic, strong) NSString *passwordHash;

@end

@implementation NFCSessionController

- (instancetype)init {
    self = [super init];
    if (self) {
        self.passwordHash = @"9bno9SrTNl6QM+SOBAxcEis64ncJcAFa7w95rznjq90=";
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

- (void)startReaderSessionWithAlertMessage:(NSString *)alertMessage {
    self.success = NO;
    self.readerSession = [[NFCTagReaderSession alloc] initWithPollingOption:NFCPollingISO14443 delegate:self queue:nil];
    self.readerSession.alertMessage = alertMessage;
    [self.readerSession beginSession];
}

// https://stackoverflow.com/questions/9372815/how-can-i-convert-my-device-token-nsdata-into-an-nsstring
+ (NSString *)stringWithDeviceToken:(NSData *)deviceToken {
    const char *data = [deviceToken bytes];
    NSMutableString *token = [NSMutableString string];

    for (NSUInteger i = 0; i < [deviceToken length]; i++) {
        [token appendFormat:@"%02.2hhx", data[i]];
    }

    return [token copy];
}

- (void)OnNFCSessionCompleted {
    if (OnNFCSessionCompleted != NULL) {
        dispatch_async(dispatch_get_main_queue(), ^{
            if (self.success) {
                HoloKitModel holokitModel = [HoloKitProfile getHoloKitModel:(HoloKitType)self.holokitType];
                HoloKitCameraData holokitCameraData = [HoloKitOptics getHoloKitCameraData:holokitModel ipd:self.ipd farClipPlane:self.farClipPlane];
                // Convert camera data to float pointer
                float *result = (float *)malloc(sizeof(float) * 55);
                result[0] = holokitCameraData.LeftViewportRect.x;
                result[1] = holokitCameraData.LeftViewportRect.y;
                result[2] = holokitCameraData.LeftViewportRect.z;
                result[3] = holokitCameraData.LeftViewportRect.w;
                result[4] = holokitCameraData.RightViewportRect.x;
                result[5] = holokitCameraData.RightViewportRect.y;
                result[6] = holokitCameraData.RightViewportRect.z;
                result[7] = holokitCameraData.RightViewportRect.w;
                result[8] = holokitCameraData.NearClipPlane;
                result[9] = holokitCameraData.FarClipPlane;
                for (int i = 10; i < 14; i++) {
                    result[i] = holokitCameraData.LeftProjectionMatrix.columns[i - 10].x;
                }
                for (int i = 14; i < 18; i++) {
                    result[i] = holokitCameraData.LeftProjectionMatrix.columns[i - 14].y;
                }
                for (int i = 18; i < 22; i++) {
                    result[i] = holokitCameraData.LeftProjectionMatrix.columns[i - 18].z;
                }
                for (int i = 22; i < 26; i++) {
                    result[i] = holokitCameraData.LeftProjectionMatrix.columns[i - 22].w;
                }
                for (int i = 26; i < 30; i++) {
                    result[i] = holokitCameraData.RightProjectionMatrix.columns[i - 26].x;
                }
                for (int i = 30; i < 34; i++) {
                    result[i] = holokitCameraData.RightProjectionMatrix.columns[i - 30].y;
                }
                for (int i = 34; i < 38; i++) {
                    result[i] = holokitCameraData.RightProjectionMatrix.columns[i - 34].z;
                }
                for (int i = 38; i < 42; i++) {
                    result[i] = holokitCameraData.RightProjectionMatrix.columns[i - 38].w;
                }
                result[42] = holokitCameraData.CameraToCenterEyeOffset.x;
                result[43] = holokitCameraData.CameraToCenterEyeOffset.y;
                result[44] = holokitCameraData.CameraToCenterEyeOffset.z;
                result[45] = holokitCameraData.CameraToScreenCenterOffset.x;
                result[46] = holokitCameraData.CameraToScreenCenterOffset.y;
                result[47] = holokitCameraData.CameraToScreenCenterOffset.z;
                result[48] = holokitCameraData.CenterEyeToLeftEyeOffset.x;
                result[49] = holokitCameraData.CenterEyeToLeftEyeOffset.y;
                result[50] = holokitCameraData.CenterEyeToLeftEyeOffset.z;
                result[51] = holokitCameraData.CenterEyeToRightEyeOffset.x;
                result[52] = holokitCameraData.CenterEyeToRightEyeOffset.y;
                result[53] = holokitCameraData.CenterEyeToRightEyeOffset.z;
                result[54] = holokitCameraData.AlignmentMarkerOffset;
                
                OnNFCSessionCompleted(self.success, result);
                free(result);
            } else {
                OnNFCSessionCompleted(self.success, nil);
            }
        });
    }
}

+ (NSString *)getSignatureFromRawContent:(NSString *)rawContent {
    NSString *a = [rawContent componentsSeparatedByString:@"s="][1];
    NSString *b = [a componentsSeparatedByString:@"&"][0];
    return b;
}

+ (NSString *)getContentFromRawContent:(NSString *)rawContent {
    NSString *a = [rawContent componentsSeparatedByString:@"c="][1];
    return a;
}

+ (NSString*)sha256HashFor:(NSString*)input {
    NSData* data = [input dataUsingEncoding:NSUTF8StringEncoding];
    NSMutableData *sha256Data = [NSMutableData dataWithLength:CC_SHA256_DIGEST_LENGTH];
    CC_SHA256([data bytes], (CC_LONG)[data length], [sha256Data mutableBytes]);
    return [sha256Data base64EncodedStringWithOptions:0];
}

#pragma mark - Delegates

- (void)tagReaderSessionDidBecomeActive:(NFCTagReaderSession *)session {

}

- (void)tagReaderSession:(NFCTagReaderSession *)session didInvalidateWithError:(NSError *)error {
    [self OnNFCSessionCompleted];
}

- (void)tagReaderSession:(NFCTagReaderSession *)session didDetectTags:(NSArray<__kindof id<NFCTag>> *)tags {
    if ([tags count] > 1) {
        [session invalidateSessionWithErrorMessage:@"More than one NFC tags were detected"];
        return;
    }
    id<NFCTag> tag = tags[0];
    [session connectToTag:tag completionHandler:^(NSError * _Nullable error) {
        if (error != nil) {
            [session invalidateSessionWithErrorMessage:@"Failed to connect to the NFC tag"];
            return;
        }
        id<NFCISO7816Tag> sTag = [tag asNFCISO7816Tag];
        if (sTag == nil) {
            id<NFCMiFareTag> sTag2 = [tag asNFCMiFareTag];
            if (sTag2 == nil) {
                [session invalidateSessionWithErrorMessage:@"This tag is not compliant to ISO7816 or MiFare"];
                return;
            }
            [self processMiFareTag:sTag2 session:session];
        }
        [self processISO7816Tag:sTag session:session];
    }];
}

- (void)processISO7816Tag:(id<NFCISO7816Tag>)sTag session:(NFCTagReaderSession *)session{
    NSString *uid = [NFCSessionController stringWithDeviceToken:[sTag identifier]];
    uid = [uid uppercaseString];
    uid = [NSString stringWithFormat:@"%@%@", @"0x", uid];
    //NSLog(@"[nfc_session] tag uid %@", uid);
    [sTag queryNDEFStatusWithCompletionHandler:^(NFCNDEFStatus status, NSUInteger capacity, NSError * _Nullable error) {
        if (error != nil) {
            [session invalidateSessionWithErrorMessage:@"Failed to query NDEF status of the tag"];
            return;
        }
        switch (status) {
            case NFCNDEFStatusNotSupported:
                [session invalidateSessionWithErrorMessage:@"NDEF is not supported on this tag"];
                return;
            case NFCNDEFStatusReadOnly:
            case NFCNDEFStatusReadWrite: {
                [sTag readNDEFWithCompletionHandler:^(NFCNDEFMessage * _Nullable message, NSError * _Nullable error) {
                    if (error != nil) {
                        [session invalidateSessionWithErrorMessage:@"Failed to write data to the tag"];
                        return;
                    }
                    if ([message.records count] < 1 || message.records[0].payload == nil) {
                        [session invalidateSessionWithErrorMessage:@"There is no data in this tag"];
                        return;
                    }
                    NSString *rawContent = [[NSString alloc] initWithData:message.records[0].payload encoding:NSUTF8StringEncoding];
                    NSString *signature = [NFCSessionController getSignatureFromRawContent:rawContent];
                    NSString *content = [NFCSessionController getContentFromRawContent:rawContent];
                    if ([content isEqualToString:uid]) {
                        if ([Crypto validateSignatureWithSignature:signature content:content]) {
                            self.success = YES;
                            [session setAlertMessage:@"NFC authentication succeeded"];
                            [session invalidateSession];
                            return;
                        } else {
                            [session invalidateSessionWithErrorMessage:@"NFC authentication failed"];
                            return;
                        }
                    } else {
                        [session invalidateSessionWithErrorMessage:@"NFC authentication failed"];
                        return;
                    }
                }];
                break;
            }
            default:
                [session invalidateSessionWithErrorMessage:@"Failed to write data to the tag"];
                return;
        }
    }];
}

- (void)processMiFareTag:(id<NFCMiFareTag>)sTag session:(NFCTagReaderSession *)session{
    NSString *uid = [NFCSessionController stringWithDeviceToken:[sTag identifier]];
    uid = [uid uppercaseString];
    uid = [NSString stringWithFormat:@"%@%@", @"0x", uid];
    //NSLog(@"[nfc_session] tag uid %@", uid);
    [sTag queryNDEFStatusWithCompletionHandler:^(NFCNDEFStatus status, NSUInteger capacity, NSError * _Nullable error) {
        if (error != nil) {
            [session invalidateSessionWithErrorMessage:@"Failed to query NDEF status of the tag"];
            return;
        }
        switch (status) {
            case NFCNDEFStatusNotSupported:
                [session invalidateSessionWithErrorMessage:@"NDEF is not supported on this tag"];
                return;
            case NFCNDEFStatusReadOnly:
            case NFCNDEFStatusReadWrite: {
                [sTag readNDEFWithCompletionHandler:^(NFCNDEFMessage * _Nullable message, NSError * _Nullable error) {
                    if (error != nil) {
                        [session invalidateSessionWithErrorMessage:@"Failed to write data to the tag"];
                        return;
                    }
                    if ([message.records count] < 1 || message.records[0].payload == nil) {
                        [session invalidateSessionWithErrorMessage:@"There is no data in this tag"];
                        return;
                    }
                    NSString *rawContent = [[NSString alloc] initWithData:message.records[0].payload encoding:NSUTF8StringEncoding];
                    NSString *signature = [NFCSessionController getSignatureFromRawContent:rawContent];
                    NSString *content = [NFCSessionController getContentFromRawContent:rawContent];
                    if ([content isEqualToString:uid]) {
                        if ([Crypto validateSignatureWithSignature:signature content:content]) {
                            self.success = YES;
                            [session setAlertMessage:@"NFC authentication succeeded"];
                            [session invalidateSession];
                            return;
                        } else {
                            [session invalidateSessionWithErrorMessage:@"NFC authentication failed"];
                            return;
                        }
                    } else {
                        [session invalidateSessionWithErrorMessage:@"NFC authentication failed"];
                        return;
                    }
                }];
                break;
            }
            default:
                [session invalidateSessionWithErrorMessage:@"Failed to write data to the tag"];
                return;
        }
    }];
}

@end


extern void HoloKitSDK_StartNFCSession(const char *alertMessage, int holokitType, float ipd, float farClipPlane) {
    NFCSessionController *controller = [NFCSessionController sharedInstance];
    controller.holokitType = holokitType;
    controller.ipd = ipd;
    controller.farClipPlane = farClipPlane;
    if (alertMessage == NULL) {
        [controller startReaderSessionWithAlertMessage:nil];
    } else {
        [controller startReaderSessionWithAlertMessage:[NSString stringWithUTF8String:alertMessage]];
    }
}

extern void HoloKitSDK_SkipNFCSessionWithPassword(const char *password, int holokitType, float ipd, float farClipPlane) {
    NSString *hash = [NFCSessionController sha256HashFor:[NSString stringWithUTF8String:password]];
    NFCSessionController *controller = [NFCSessionController sharedInstance];
    if ([controller.passwordHash isEqualToString:hash]) {
        NSLog(@"Password correct");
        controller.holokitType = holokitType;
        controller.ipd = ipd;
        controller.farClipPlane = farClipPlane;
        
        controller.success = YES;
        [controller OnNFCSessionCompleted];
    } else {
        NSLog(@"Password not correct");
    }
}

extern void HoloKitSDK_RegisterNFCSessionControllerDelegates(void (*OnNFCSessionCompletedDelegate)(bool, float *)) {
    OnNFCSessionCompleted = OnNFCSessionCompletedDelegate;
}

