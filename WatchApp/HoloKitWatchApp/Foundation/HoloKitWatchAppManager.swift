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
        
        initializeHoloKitAppWCSessionDelegate()
    }
    
    public func initializeHoloKitAppWCSessionDelegate() {
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
        
    }
    
    func session(_ session: WCSession, didReceiveMessage message: [String : Any]) {
        if let realityId = message["RealityId"] as? Int {
            print("Received realityId: \(realityId)")
            if let reality = Reality(rawValue: realityId) {
                self.currentReality = reality
            }
        }
    }
}
