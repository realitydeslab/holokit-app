//
//  MockMofaWatchConnectivityManager.swift
//  MockHoloKitApp
//
//  Created by Yuchen Zhang on 2022/10/19.
//

import WatchConnectivity

enum MofaRoundResult: Int {
    case victory = 0
    case defeat = 1
    case draw = 2
}

class MockMofaWatchConnectivityManager: NSObject, ObservableObject {
    
    private var wcSession: WCSession!
    
    override init() {
        super.init()
    }
    
    func takeControlWatchConnectivitySession() {
        if (WCSession.isSupported()) {
            wcSession = WCSession.default
            wcSession.delegate = self
        }
        print("MofaWatchConnectivityManager took control")
    }
    
    func onRoundStarted() {
        let context = ["IsFighting" : true, "Timestamp" : ProcessInfo.processInfo.systemUptime] as [String : Any];
        do {
            try self.wcSession.updateApplicationContext(context)
            print("Fighting phase synced")
        } catch {
            print("Failed to sync fighting phase")
        }
    }
    
    func onRoundEnded(_ roundResult: MofaRoundResult, _ kill: Int, _ hitRate: Float, _ distance: Float) {
        let context = ["IsFighting" : false,
                       "RoundResult" : roundResult.rawValue,
                       "Kill" : kill,
                       "HitRate" : hitRate,
                       "Distance" : distance,
                       "Timestamp" : ProcessInfo.processInfo.systemUptime] as [String : Any];
        do {
            try self.wcSession.updateApplicationContext(context)
            print("Round result synced")
        } catch {
            print("Failed to sync round result")
        }
    }
    
    func updateCurrentReality(_ realityIndex: Int) {
        let context = ["CurrentReality" : realityIndex,
                       "Timestamp" : ProcessInfo.processInfo.systemUptime] as [String : Any];
        do {
            try self.wcSession.updateApplicationContext(context)
            print("Updated current reality")
        } catch {
            print("Failed to update current reality")
        }
    }
    
    func queryWatchState() {
        let message = ["QueryWatchState" : 0]
        self.wcSession.sendMessage(message) { replyMessage in
            if let watchStateIndex = replyMessage["WatchState"] as? Int {
                print("On received query watch state replay: \(watchStateIndex)")
            }
        }
    }
}

extension MockMofaWatchConnectivityManager: WCSessionDelegate {
    
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
    
    func session(_ session: WCSession, didReceiveMessage message: [String : Any]) {
        if message["StartRound"] is Int {
            print("Start round message received")
            self.onRoundStarted()
        }
    }
}
