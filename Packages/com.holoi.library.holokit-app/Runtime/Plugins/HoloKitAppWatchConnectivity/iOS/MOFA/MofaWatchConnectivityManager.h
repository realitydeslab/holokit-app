#import <Foundation/Foundation.h>

@interface MofaWatchConnectivityManager : NSObject

+ (id)sharedInstance;

- (void)didReceiveMessage:(NSDictionary<NSString *,id> *)message;

@end
