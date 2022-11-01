import Foundation
import WatchKit
import WatchConnectivity
import HealthKit

enum HoloKitController: Int {
    case nothing = 0
    case mofa = 1
}

class HoloKitWatchAppManager: NSObject, ObservableObject {
    
    @Published var currentController: HoloKitController = .nothing
    
    private var wcSession: WCSession!
    
    override init() {
        super.init()
        print("[HoloKitWatchAppManager] init")
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
        self.currentController = .nothing
    }
}

// MARK: - WCSessionDelegate
extension HoloKitWatchAppManager: WCSessionDelegate {
    
    func session(_ session: WCSession, activationDidCompleteWith activationState: WCSessionActivationState, error: Error?) {
        if (activationState == .activated) {
            print("[HoloKitWatchAppManager] WCSession activated");
        } else {
            print("[HoloKitWatchAppManager] WCSession activation failed");
        }
    }
    
    func session(_ session: WCSession, didReceiveApplicationContext applicationContext: [String : Any]) {
        print("[HoloKitWatchAppManager] didReceiveApplicationContext")
        if let realityIndex = applicationContext["CurrentReality"] as? Int {
            if let reality = HoloKitController(rawValue: realityIndex) {
                print("Switched to reality: \(String(describing: reality))")
                DispatchQueue.main.async {
                    self.currentController = reality
                }
            }
        }
    }
    
    func session(_ session: WCSession, didReceiveMessage message: [String : Any]) {
        print("[HoloKitWatchAppManager] didReceiveMessage")
    }
}
