#import "MofaWatchConnectivityManager.h"
#import "HoloKitAppWatchConnectivityManager.h"

// This callback is called when the iPhone received start round message from the Watch
void (*OnReceivedStartRoundMessage)(void) = NULL;
// This callback is called when the Watch changed its orientation relative to the ground
void (*OnWatchStateChanged)(int) = NULL;
// This callback is called when the Watch reaches a certain acceleration threshold.
void (*OnWatchTriggered)(void) = NULL;
// This callback is called when the iPhone receives the health data message including
// dist and energy.
void (*OnReceivedHealthDataMessage)(float, float) = NULL;

@interface MofaWatchConnectivityManager()

@end

@implementation MofaWatchConnectivityManager

- (instancetype)init {
    if (self = [super init]) {
        
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

- (void)didReceiveMessage:(NSDictionary<NSString *,id> *)message {
    // For watch state change
    id watchState = [message objectForKey:@"WatchState"];
    if (watchState != nil) {
        if (OnWatchStateChanged != NULL) {
            dispatch_async(dispatch_get_main_queue(), ^{
                OnWatchStateChanged([watchState intValue]);
            });
        }
        return;
    }
    
    // For watch trigger action
    id watchTriggered = [message objectForKey:@"WatchTriggered"];
    if (watchTriggered != nil) {
        if (OnWatchTriggered != NULL) {
            dispatch_async(dispatch_get_main_queue(), ^{
                OnWatchTriggered();
            });
        }
        return;
    }
    
    // For round start message
    id start = [message objectForKey:@"Start"];
    if (start != nil) {
        if (OnReceivedStartRoundMessage != NULL) {
            dispatch_async(dispatch_get_main_queue(), ^{
                OnReceivedStartRoundMessage();
            });
        }
        return;
    }
    
    // For health data message
    id dist = [message objectForKey:@"Dist"];
    id energy = [message objectForKey:@"Energy"];
    if (dist != nil && energy != nil) {
        if (OnReceivedHealthDataMessage != NULL) {
            dispatch_async(dispatch_get_main_queue(), ^{
                OnReceivedHealthDataMessage([dist floatValue], [energy floatValue]);
            });
        }
    }
}

- (void)OnRoundStarted:(int)magicSchoolIndex {
    NSDictionary<NSString *, id> *message = @{
        @"MOFA" : [NSNumber numberWithBool:YES],
        @"Start" : [NSNumber numberWithBool:YES],
        @"Timestamp" : [NSNumber numberWithDouble:[[NSProcessInfo processInfo] systemUptime]],
        @"MagicSchool": [NSNumber numberWithInt:magicSchoolIndex]
    };
    [[[HoloKitAppWatchConnectivityManager sharedInstance] wcSession] updateApplicationContext:message error:nil];
}

- (void)queryWatchState {
    NSDictionary<NSString *, id> *message = @{ @"QueryWatchState" : [NSNumber numberWithBool:YES] };
    [[[HoloKitAppWatchConnectivityManager sharedInstance] wcSession] sendMessage:message
    replyHandler:^(NSDictionary<NSString *,id> * _Nonnull replyMessage) {
        id watchState = [replyMessage objectForKey:@"WatchState"];
        if (watchState != nil) {
            if (OnWatchStateChanged != NULL) {
                dispatch_async(dispatch_get_main_queue(), ^{
                    OnWatchStateChanged([watchState intValue]);
                });
            }
        }
    } errorHandler:^(NSError * _Nonnull error) {
        if (error != nil) {
            NSLog(@"[WatchConnectivity_MOFA] Failed to send query watch state message to watch");
        }
    }];
}

- (void)OnRoundEndedWithResult:(int)result kill:(int)kill hitRate:(int)hitRate {
    NSDictionary<NSString *, id> *message = @{
        @"MOFA" : [NSNumber numberWithBool:YES],
        @"End" : [NSNumber numberWithBool:YES],
        @"Result" : [NSNumber numberWithInt:result],
        @"Kill" : [NSNumber numberWithInt:kill],
        @"HitRate" : [NSNumber numberWithInt:hitRate]
    };
    [[[HoloKitAppWatchConnectivityManager sharedInstance] wcSession] updateApplicationContext:message error:nil];
}

@end

#pragma mark - Marshal

void MofaWatchConnectivity_Initialize(void (*OnReceivedStartRoundMessageDelegate)(void),
                                      void (*OnWatchStateChangedDelegate)(int),
                                      void (*OnWatchTriggeredDelegate)(void),
                                      void (*OnReceivedHealthDataMessageDelegate)(float, float)) {
    OnReceivedStartRoundMessage = OnReceivedStartRoundMessageDelegate;
    OnWatchStateChanged = OnWatchStateChangedDelegate;
    OnWatchTriggered = OnWatchTriggeredDelegate;
    OnReceivedHealthDataMessage = OnReceivedHealthDataMessageDelegate;
}

void MofaWatchConnectivity_OnRoundStarted(int magicSchoolIndex) {
    [[MofaWatchConnectivityManager sharedInstance] OnRoundStarted:magicSchoolIndex];
}

void MofaWatchConnectivity_QueryWatchState(void) {
    [[MofaWatchConnectivityManager sharedInstance] queryWatchState];
}

void MofaWatchConnectivity_OnRoundEnded(int result, int kill, int hitRate) {
    [[MofaWatchConnectivityManager sharedInstance] OnRoundEndedWithResult:result kill:kill hitRate:hitRate];
}
