import Foundation
import WatchKit
import WatchConnectivity
import HealthKit

enum Reality: Int {
    case nothing = 0
    case mofaTheTraining = 1
}

class HoloKitWatchAppManager: NSObject, ObservableObject {
    
    @Published var currentReality: Reality = .nothing
    
    public var mofaWatchAppManager = MofaWatchAppManager()
    
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
        if let realityIndex = applicationContext["CurrentReality"] as? Int {
            if let reality = Reality(rawValue: realityIndex) {
                print("Switched to reality: \(String(describing: reality))")
                DispatchQueue.main.async {
                    self.currentReality = reality
                }
            }
        }
    }
    
    func session(_ session: WCSession, didReceiveMessage message: [String : Any]) {
        
    }
}
