import Foundation
import WatchKit
import WatchConnectivity
import HealthKit

// Make sure this enum is identical to the corresponding C# enum
enum HoloKitWatchPanel: Int {
    case none = 0
    case mofa = 1
}

class HoloKitWatchAppManager: NSObject, ObservableObject {
    
    // This class is a singleton
    static let shared = HoloKitWatchAppManager()
    
    // The current active panel of the Watch App
    @Published var panel: HoloKitWatchPanel = .none
    
    // There is only one default WatchConnectivity session
    var wcSession: WCSession!
    
    // We keep a reference of each watch panel's manager
    let mofaWatchAppManager: MofaWatchAppManager = MofaWatchAppManager()
    
    override init() {
        super.init()
        if (WCSession.isSupported()) {
            wcSession = WCSession.default
            wcSession.delegate = self
            wcSession.activate()
        }
    }
    
    func switchPanel(panel: HoloKitWatchPanel) {
        self.panel = panel
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
    
    func sessionReachabilityDidChange(_ session: WCSession) {
        print("Apple Watch sessionReachabilityDidChange: \(session.isReachable)")
    }
    
    func session(_ session: WCSession, didReceiveApplicationContext applicationContext: [String : Any]) {
        // Switch to the corresponding panel after receiving a panel switch message
        if let watchPanelIndex = applicationContext["Panel"] as? Int {
            if let watchPanel = HoloKitWatchPanel(rawValue: watchPanelIndex) {
                if (self.panel != watchPanel) {
                    print("Switched to panel: \(String(describing: watchPanel))")
                    DispatchQueue.main.async {
                        self.panel = watchPanel
                    }
                }
            }
            return
        }
        
        if applicationContext["MOFA"] is Bool {
            mofaWatchAppManager.didReceiveApplicationContext(applicationContext: applicationContext)
            return
        }
    }
    
    func session(_ session: WCSession, didReceiveMessage message: [String : Any]) {
        
    }
    
    func session(_ session: WCSession, didReceiveMessage message: [String : Any], replyHandler: @escaping ([String : Any]) -> Void) {
        
    }
}
