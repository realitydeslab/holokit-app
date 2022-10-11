//
//  MockHoloKitWatchAppManager.swift
//  MockHoloKitApp
//
//  Created by Yuchen Zhang on 2022/10/11.
//

import WatchConnectivity

class MockHoloKitWatchAppManager: NSObject, ObservableObject, WCSessionDelegate {
    
    var wcSession: WCSession!
    
    override init() {
        super.init()
        
        if (WCSession.isSupported()) {
            wcSession = WCSession.default
            wcSession.delegate = self
            wcSession.activate()
        }
    }
    
    func isWatchAppInstalled() -> Bool {
        print("isWatchAppInstalled: \(wcSession.isWatchAppInstalled)");
        return wcSession.isWatchAppInstalled;
    }
    
    func initializeWithRealityId(realityId: Int) {
        if (self.wcSession.isReachable) {
            let message = ["RealityId" : realityId]
            self.wcSession.sendMessage(message, replyHandler: nil)
        } else {
            print("Watch is not reachable")
        }
    }
    
    // MARK: WCSessionDelegate
    func session(_ session: WCSession, activationDidCompleteWith activationState: WCSessionActivationState, error: Error?) {
        switch (activationState) {
        case .notActivated:
            print("activationDidCompleteWith: notActivated");
            break;
        case .inactive:
            print("activationDidCompleteWith: inactive");
            break;
        case .activated:
            print("activationDidCompleteWith: activated");
            break;
        @unknown default:
            break;
        }
    }
    
    func sessionDidBecomeInactive(_ session: WCSession) {
        
    }
    
    func sessionDidDeactivate(_ session: WCSession) {
        
    }
    
    func sessionReachabilityDidChange(_ session: WCSession) {
        print("sessionReachabilityDidChange: \(session.isReachable)")
    }
    
    func sessionWatchStateDidChange(_ session: WCSession) {
        print("sessionWatchStateDidChange")
    }
}
