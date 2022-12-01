import Foundation
import WatchKit
import WatchConnectivity
import HealthKit

enum HoloKitWatchAppPanel: Int {
    case none = 0
    case mofa = 1
}

class HoloKitWatchAppManager: NSObject, ObservableObject {
    
    @Published var currentPanel: HoloKitWatchAppPanel = .none
    
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
            wcSession.delegate = self
        }
        self.currentPanel = .none
    }
}

// MARK: - WCSessionDelegate
extension HoloKitWatchAppManager: WCSessionDelegate {
    
    func session(_ session: WCSession, activationDidCompleteWith activationState: WCSessionActivationState, error: Error?) {
        if (activationState == .activated) {
            print("[HoloKitWatchAppManager] WCSession activated");
        } else {
            print("[HoloKitWatchAppManager] Failed to activate WCSession");
        }
    }
    
    func session(_ session: WCSession, didReceiveApplicationContext applicationContext: [String : Any]) {
        if let panelIndex = applicationContext["CurrentPanel"] as? Int {
            if let panel = HoloKitWatchAppPanel(rawValue: panelIndex) {
                print("Switched to panel: \(String(describing: panel))")
                DispatchQueue.main.async {
                    self.currentPanel = panel
                }
            }
        }
    }
    
    func session(_ session: WCSession, didReceiveMessage message: [String : Any]) {

    }
}
