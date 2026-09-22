// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

import WatchConnectivity

enum MofaRoundResult: Int {
    case victory = 0
    case defeat = 1
    case draw = 2
}

class MockMofaWatchConnectivityManager: NSObject, ObservableObject {
    
    var isFighting = false
    
    override init() {
        super.init()
    }
    
    func onRoundStarted() {
        let context = ["MOFA" : true,
                       "Start" : true,
                       "MagicSchool" : 4,
                       "Timestamp" : ProcessInfo.processInfo.systemUptime] as [String : Any]
        
        do {
            try MockHoloKitAppWatchConnectivityManager.shared.wcSession.updateApplicationContext(context)
            self.isFighting = true
            print("Fighting phase synced")
        } catch {
            print("Failed to sync fighting phase")
        }
    }
    
    func onRoundEnded(roundResult: MofaRoundResult, kill: Int, hitRate: Float) {
        let context = ["MOFA" : true,
                       "End" : true,
                       "RoundResult" : roundResult.rawValue,
                       "Kill" : kill,
                       "HitRate" : hitRate] as [String : Any]
        do {
            try MockHoloKitAppWatchConnectivityManager.shared.wcSession.updateApplicationContext(context)
            self.isFighting = false
            print("Round result synced")
        } catch {
            print("Failed to sync round result")
        }
    }
    
    func queryWatchState() {
        let message = ["MOFA" : true,
                       "QueryState" : 0] as [String : Any]
        MockHoloKitAppWatchConnectivityManager.shared.wcSession.sendMessage(message) { replyMessage in
            if let watchStateIndex = replyMessage["WatchState"] as? Int {
                print("On received query watch state replay: \(watchStateIndex)")
            }
        }
    }
    
    func didReceiveMessage(message: [String : Any]) {
        if message["Start"] is Bool {
            onRoundStarted()
        }
    }
}
