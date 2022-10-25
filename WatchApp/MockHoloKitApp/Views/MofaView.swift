//
//  MofaView.swift
//  MockHoloKitApp
//
//  Created by Yuchen Zhang on 2022/10/19.
//

import SwiftUI

struct MofaView: View {
    
    @EnvironmentObject var mofaWatchConnectivityManager: MockMofaWatchConnectivityManager
    
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
                mofaWatchConnectivityManager.onRoundEnded(.victory, 24, 0.43, 72)
            }
            
//            Spacer()
//                .frame(height: 50)
//            
//            Button("Query Watch State") {
//                mofaWatchConnectivityManager.queryWatchState()
//            }
            
            Spacer()
                .frame(height: 50)
            
            Button("Back") {
                mofaWatchConnectivityManager.updateCurrentReality(0)
            }
            
            Spacer()
                .frame(height: 50)
            
            Button("Re-enter") {
                mofaWatchConnectivityManager.updateCurrentReality(1)
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
