import WatchConnectivity

enum Reality: Int {
    case nothing = 0
    case mofaTheTraining = 1
}

class MockHoloKitAppWatchConnectivityManager: NSObject, ObservableObject {
    
    @Published var currentReality: Reality = .nothing
    
    private var wcSession: WCSession!
    
    override init() {
        super.init()
        
        if (WCSession.isSupported()) {
            wcSession = WCSession.default
            wcSession.delegate = self
            wcSession.activate()
        }
    }
    
    func takeControlWatchConnectivitySession() {
        if (WCSession.isSupported()) {
            wcSession = WCSession.default
            wcSession.delegate = self
        }
        print("HoloKitAppWatchConnectivityManager took control")
    }
    
    func hasPairedAppleWatch() -> Bool {
        return self.wcSession.isPaired;
    }
    
    func isWatchAppInstalled() -> Bool {
        return self.wcSession.isWatchAppInstalled;
    }
    
    func activate() {
        self.wcSession.activate()
    }
    
    func isReachable() -> Bool {
        return self.wcSession.isReachable
    }
    
    func updateCurrentReality(_ panelIndex: Int) {
        let context = ["CurrentPanel" : panelIndex,
                       "Timestamp" : ProcessInfo.processInfo.systemUptime] as [String : Any];
        do {
            try self.wcSession.updateApplicationContext(context)
            print("Updated current reality")
        } catch {
            print("Failed to update current reality")
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

