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
    
    var wcSession: WCSession!
    
    override init() {
        super.init()
        
        if (WCSession.isSupported()) {
            wcSession = WCSession.default
            wcSession.delegate = self
            wcSession.activate()
        }
    }
}

// MARK: - WCSessionDelegate
extension HoloKitWatchAppManager: WCSessionDelegate {
    
    func session(_ session: WCSession, activationDidCompleteWith activationState: WCSessionActivationState, error: Error?) {
        if (activationState == .activated) {
            print("WCSession activated");
        } else {
            print("WCSession activation failed");
        }
    }
    
    func session(_ session: WCSession, didReceiveApplicationContext applicationContext: [String : Any]) {
        print("didReceiveApplicationContext")
        if let realityIndex = applicationContext["CurrentReality"] as? Int {
            print("Received current reality updated message with reality index: \(realityIndex)")
            if let reality = Reality(rawValue: realityIndex) {
                self.currentReality = reality
            }
        }
    }
    
    func session(_ session: WCSession, didReceiveMessage message: [String : Any]) {
        print("didReceiveMessage")
    }
}
