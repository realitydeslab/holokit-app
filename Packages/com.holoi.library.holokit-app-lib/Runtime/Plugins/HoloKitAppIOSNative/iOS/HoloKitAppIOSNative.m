// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

@interface HoloKitAppIOSNative : NSObject

@end

@implementation HoloKitAppIOSNative

@end

void HoloKitAppIOSNative_OpenURL(const char *url) {
    [[UIApplication sharedApplication] openURL:[NSURL URLWithString:[NSString stringWithUTF8String:url]] options:@{} completionHandler:^(BOOL success) {
        if (!success) {
            NSLog(@"[IOSNative] Failed to open URL: %@", [NSString stringWithUTF8String:url]);
        }
    }];
}

void HoloKitAppIOSNative_ShowFeedbackAlert(const char *title, const char *message, const char *actionTitle, const char *url) {
    UIAlertController *alert = [UIAlertController alertControllerWithTitle:[NSString stringWithUTF8String:title] message:[NSString stringWithUTF8String:message] preferredStyle:UIAlertControllerStyleAlert];
    NSString *discordUrl = [NSString stringWithUTF8String:url];
    UIAlertAction *action = [UIAlertAction actionWithTitle:[NSString stringWithUTF8String:actionTitle] style:UIAlertActionStyleDefault handler:^(UIAlertAction * _Nonnull action) {
        [[UIApplication sharedApplication] openURL:[NSURL URLWithString:discordUrl] options:@{} completionHandler:^(BOOL success) {
            if (!success) {
                NSLog(@"[IOSNative] Failed to open URL: %@", discordUrl);
            }
        }];
    }];
    UIAlertAction *cancelAction = [UIAlertAction actionWithTitle:@"Cancel" style:UIAlertActionStyleDefault handler:nil];
    [alert addAction:action];
    [alert addAction:cancelAction];
    UIWindow * currentwindow = [[UIApplication sharedApplication] delegate].window;
    [currentwindow.rootViewController presentViewController:alert animated:YES completion:nil];
}
