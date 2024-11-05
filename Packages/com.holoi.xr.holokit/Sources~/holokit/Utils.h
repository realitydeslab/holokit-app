// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

#import <Foundation/Foundation.h>
#import <simd/simd.h>

@interface Utils : NSObject

+ (simd_float4x4)getSimdFloat4x4WithPosition:(float [3])position rotation:(float [4])rotation;

+ (float *)getUnityMatrix:(simd_float4x4)arkitMatrix;

@end
