import WatchConnectivity

enum HoloKitWatchPanel: Int {
    case none = 0
    case mofa = 1
}

class MockHoloKitAppWatchConnectivityManager: NSObject, ObservableObject {
    
    @Published var currentWatchPanel: HoloKitWatchPanel = .none
    
    @Published var isWatchAppInstalledVar: Bool = false
    
    @Published var isReachableVar: Bool = false
    
    private var wcSession: WCSession!
    
    override init() {
        super.init()
        
        takeControlWatchConnectivitySession()
    }
    
    func takeControlWatchConnectivitySession() {
        if (WCSession.isSupported()) {
            wcSession = WCSession.default
            wcSession.delegate = self
            wcSession.activate()
            print("HoloKitAppWatchConnectivityManager took control")
        }
    }
    
    func hasPairedAppleWatch() -> Bool {
        return self.wcSession.isPaired;
    }
    
    func isWatchAppInstalled() -> Bool {
        isWatchAppInstalledVar = self.wcSession.isWatchAppInstalled
        return isWatchAppInstalledVar
    }
    
    func activate() {
        self.wcSession.activate()
    }
    
    func isReachable() -> Bool {
        isReachableVar = self.wcSession.isReachable
        return isReachableVar
    }
    
    func updateCurrentWatchPanel(_ watchPanelIndex: Int) {
        let context = ["CurrentWatchPanel" : watchPanelIndex,
                       "Timestamp" : ProcessInfo.processInfo.systemUptime] as [String : Any];
        do {
            try self.wcSession.updateApplicationContext(context)
            print("Updated current watch panel: \(watchPanelIndex)")
        } catch {
            print("Failed to update current watch panel")
        }
    }
}

extension MockHoloKitAppWatchConnectivityManager: WCSessionDelegate {
    
    func session(_ session: WCSession, activationDidCompleteWith activationState: WCSessionActivationState, error: Error?) {
        if (activationState == .activated) {
            print("iPhone's WCSession activated");
        } else {
            print("iPhone's WCSession activation failed")
        }
    }
    
    func sessionDidBecomeInactive(_ session: WCSession) {
        
    }
    
    func sessionDidDeactivate(_ session: WCSession) {
        
    }
    
    func sessionReachabilityDidChange(_ session: WCSession) {
        print("WCSession reachability did change: \(session.isReachable)")
    }
    
    func session(_ session: WCSession, didReceiveMessage message: [String : Any]) {
        
    }
}

