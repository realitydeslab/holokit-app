//
//  MofaView.swift
//  MockHoloKitApp
//
//  Created by Yuchen Zhang on 2022/10/19.
//

import SwiftUI

struct MofaView: View {
    
    @ObservedObject var mofaWatchConnectivityManager = MockHoloKitAppWatchConnectivityManager.shared.mofaWatchConnectivityManager
    
    var body: some View {
        VStack {
            Text("MOFA")
            
            Spacer()
                .frame(height: 50)
            
            Button("Start Round") {
                mofaWatchConnectivityManager.onRoundStarted()
            }
            
            Spacer()
                .frame(height: 50)
            
            Button("End Round") {
                mofaWatchConnectivityManager.onRoundEnded(roundResult: .victory, kill: 8, hitRate: 24)
            }
            
            Spacer()
                .frame(height: 50)
            
            Button("Load Homepage") {
                MockHoloKitAppWatchConnectivityManager.shared.updatePanel(panelIndex: 0)
            }
        }
    }
}

struct MofaView_Previews: PreviewProvider {
    static var previews: some View {
        MofaView()
    }
}
