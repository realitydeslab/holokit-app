import Foundation
import WatchKit
import WatchConnectivity
import HealthKit

enum HoloKitWatchPanel: Int {
    case none = 0
    case mofa = 1
}

class HoloKitWatchAppManager: NSObject, ObservableObject {
    
    @Published var currentWatchPanel: HoloKitWatchPanel = .none
    
    private var wcSession: WCSession!
    
    override init() {
        super.init()
        if (WCSession.isSupported()) {
            wcSession = WCSession.default
            wcSession.delegate = self
            wcSession.activate()
        }
    }
    
    // Register the delegate of the WCSession
    func takeControlWatchConnectivitySession() {
        if (WCSession.isSupported()) {
            wcSession = WCSession.default
            wcSession.delegate = self
        }
        self.currentWatchPanel = .none
    }
}

// MARK: - WCSessionDelegate
extension HoloKitWatchAppManager: WCSessionDelegate {
    
    func session(_ session: WCSession, activationDidCompleteWith activationState: WCSessionActivationState, error: Error?) {
        if (activationState == .activated) {
            print("Apple Watch's WCSession activated");
        } else {
            print("Apple Watch's WCSession activation failed");
        }
    }
    
    func session(_ session: WCSession, didReceiveApplicationContext applicationContext: [String : Any]) {
        if let watchPanelIndex = applicationContext["CurrentWatchPanel"] as? Int {
            if let watchPanel = HoloKitWatchPanel(rawValue: watchPanelIndex) {
                print("Switched to panel: \(String(describing: watchPanel))")
                DispatchQueue.main.async {
                    self.currentWatchPanel = watchPanel
                }
            }
        }
    }
    
    func session(_ session: WCSession, didReceiveMessage message: [String : Any]) {

    }
}
