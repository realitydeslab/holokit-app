#import <WatchConnectivity/WatchConnectivity.h>

// This must be identical to the enum on the Watch side.
typedef enum {
    None = 0,
    MOFA = 1
} HoloKitWatchPanel;

@interface HoloKitAppWatchConnectivityManager: NSObject

@property (nonatomic, strong) WCSession *wcSession;

@property (assign) HoloKitWatchPanel watchPanel;

+ (id)sharedInstance;

@end
