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
                mofaWatchConnectivityManager.OnRoundStarted()
            }
            
            Spacer()
                .frame(height: 50)
            
            Button("End Round") {
                mofaWatchConnectivityManager.OnRoundEnded(.victory, 24, 0.43, 72)
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
