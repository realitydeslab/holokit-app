// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

#import <Vision/Vision.h>
#import <ARKit/ARKit.h>

@interface HandTracker : NSObject

@property (assign) bool active;

+ (id)sharedInstance;
- (void)performHumanHandPoseRequest:(ARFrame *)frame;

@end
