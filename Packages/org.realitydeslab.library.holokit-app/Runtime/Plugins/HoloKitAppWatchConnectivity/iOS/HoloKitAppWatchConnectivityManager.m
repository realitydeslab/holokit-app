// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

#import "HoloKitAppWatchConnectivityManager.h"
#import "MofaWatchConnectivityManager.h"

void (*OnSessionReachabilityChanged)(bool) = NULL;

@interface HoloKitAppWatchConnectivityManager() <WCSessionDelegate>

// We keep a reference for each watch panel manager
@property (nonatomic, strong) MofaWatchConnectivityManager *mofaWatchConnectivityManager;

@end

@implementation HoloKitAppWatchConnectivityManager

- (instancetype)init {
    if (self = [super init]) {
        if ([WCSession isSupported]) {
            self.wcSession = [WCSession defaultSession];
            self.wcSession.delegate = self;
        }
        self.mofaWatchConnectivityManager = [MofaWatchConnectivityManager sharedInstance];
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

- (void)activateWCSession {
    [self.wcSession activateSession];
}

- (BOOL)deviceHasPairedAppleWatch {
    return [self.wcSession isPaired];
}

- (BOOL)isWatchAppInstalled {
    return [self.wcSession isWatchAppInstalled];
}

- (BOOL)isWatchReachable {
    return [self.wcSession isReachable];
}

- (void)updateWatchPanel:(int)watchPanelIndex {
    NSDictionary<NSString *, id> *context = @{ @"WatchPanel" : [NSNumber numberWithInt:watchPanelIndex],
                                               @"Timestamp" : [NSNumber numberWithDouble:[[NSProcessInfo processInfo] systemUptime]] };
    NSError *error = nil;
    [self.wcSession updateApplicationContext:context error:&error];
    if (error == nil) {
        NSLog(@"[WatchConnectivity] Updated watch panel with index %d", watchPanelIndex);
    } else {
        NSLog(@"[WatchConnectivity] Failed to update watch panel");
    }
}

#pragma mark - WCSessionDelegate

- (void)session:(nonnull WCSession *)session activationDidCompleteWithState:(WCSessionActivationState)activationState error:(nullable NSError *)error {
    if (activationState == WCSessionActivationStateActivated) {
        NSLog(@"[WatchConnectivity] WCSession activated");
    } else {
        NSLog(@"[WatchConnectivity] Failed to activate WCSession");
    }
}

- (void)sessionDidBecomeInactive:(nonnull WCSession *)session {
    NSLog(@"[WatchConnectivity] sessionDidBecomeInactive");
}

- (void)sessionDidDeactivate:(nonnull WCSession *)session {
    NSLog(@"[WatchConnectivity] sessionDidDeactivate");
}

- (void)sessionReachabilityDidChange:(WCSession *)session {
    if (session.isReachable) {
        NSLog(@"[WatchConnectivity] Watch reachable");
    } else {
        NSLog(@"[WatchConnectivity] Watch not reachable");
    }
    if (OnSessionReachabilityChanged != NULL) {
        dispatch_async(dispatch_get_main_queue(), ^{
            OnSessionReachabilityChanged(session.isReachable);
        });
    }
}

- (void)session:(WCSession *)session didReceiveMessage:(NSDictionary<NSString *,id> *)message {
    // If this is a MOFA message
    id mofa = [message objectForKey:@"MOFA"];
    if (mofa != nil) {
        [self.mofaWatchConnectivityManager didReceiveMessage:message];
        return;
    }
}

- (void)session:(WCSession *)session didReceiveMessage:(NSDictionary<NSString *,id> *)message replyHandler:(void (^)(NSDictionary<NSString *,id> * _Nonnull))replyHandler {
    
}

@end

#pragma mark - Marshal

void HoloKitAppWatchConnectivity_ActivateWCSession(void (*OnSessionReachabilityChangedDelegate)(bool)) {
    OnSessionReachabilityChanged = OnSessionReachabilityChangedDelegate;
    [[HoloKitAppWatchConnectivityManager sharedInstance] activateWCSession];
}

bool HoloKitAppWatchConnectivity_DeviceHasPairedAppleWatch(void) {
    return [[HoloKitAppWatchConnectivityManager sharedInstance] deviceHasPairedAppleWatch];
}

bool HoloKitAppWatchConnectivity_IsWatchAppInstalled(void) {
    return [[HoloKitAppWatchConnectivityManager sharedInstance] isWatchAppInstalled];
}

bool HoloKitAppWatchConnectivity_IsWatchReachable(void) {
    return [[HoloKitAppWatchConnectivityManager sharedInstance] isWatchReachable];
}

void HoloKitAppWatchConnectivity_UpdateWatchPanel(int watchPanelIndex) {
    [[HoloKitAppWatchConnectivityManager sharedInstance] updateWatchPanel:watchPanelIndex];
}
