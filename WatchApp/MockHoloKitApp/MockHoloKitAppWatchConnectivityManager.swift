import WatchConnectivity

class MockHoloKitAppWatchConnectivityManager: NSObject, ObservableObject {
    
    private var wcSession: WCSession!
    
    override init() {
        super.init()
        
        if (WCSession.isSupported()){
            wcSession = WCSession.default
            wcSession.delegate = self
        }
    }
    
    func HasPairedAppleWatch() -> Bool {
        return self.wcSession.isPaired;
    }
    
    func IsWatchAppInstalled() -> Bool {
        return self.wcSession.isWatchAppInstalled;
    }
    
    func Activate() {
        self.wcSession.activate()
    }
    
    func IsReachable() -> Bool {
        return self.wcSession.isReachable
    }
    
    func UpdateCurrentReality(_ realityIndex: Int) {
        let context = ["CurrentReality" : realityIndex];
        guard (try? self.wcSession.updateApplicationContext(context)) != nil else {
            print("Failed to update current reality")
            return
        }
        print("Updated current reality to watch")
    }
    
    func SendMessage() {
        if (self.wcSession.isReachable) {
            let message = ["CurrentReality" : 0];
            self.wcSession.sendMessage(message, replyHandler: nil)
            print("Message sent")
        } else {
            print("Cannot send message since watch is not reachable")
        }
    }
}

extension MockHoloKitAppWatchConnectivityManager: WCSessionDelegate {
    
    func session(_ session: WCSession, activationDidCompleteWith activationState: WCSessionActivationState, error: Error?) {
        if (activationState == .activated) {
            print("WCSession activated");
        } else {
            print("WCSession activation failed")
        }
    }
    
    func sessionDidBecomeInactive(_ session: WCSession) {
        
    }
    
    func sessionDidDeactivate(_ session: WCSession) {
        
    }
}

