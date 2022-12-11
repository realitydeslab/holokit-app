//
//  MofaView.swift
//  MockHoloKitApp
//
//  Created by Yuchen Zhang on 2022/10/19.
//

import SwiftUI

struct MofaView: View {
    
    @EnvironmentObject var mofaWatchConnectivityManager: MockMofaWatchConnectivityManager
    
    @EnvironmentObject var holokitAppWatchConnectivityManager: MockHoloKitAppWatchConnectivityManager
    
    var body: some View {
        VStack {
            Text("MOFA")
            
            Spacer()
                .frame(height: 50)
            
            Button("Start Round") {
                mofaWatchConnectivityManager.onRoundStarted(magicSchoolIndex: 1)
            }
            
            Spacer()
                .frame(height: 50)
            
            Button("End Round") {
                mofaWatchConnectivityManager.onRoundEnded(.victory, 24, 0.43, 72)
            }
            
            Spacer()
                .frame(height: 50)
            
            Button("Load Homepage") {
                mofaWatchConnectivityManager.updateCurrentWatchPanel(0)
                self.holokitAppWatchConnectivityManager.currentWatchPanel = .none
            }
        }
    }
}

struct MofaView_Previews: PreviewProvider {
    static var previews: some View {
        MofaView()
            .environmentObject(MockMofaWatchConnectivityManager())
    }
}
