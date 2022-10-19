//
//  HoloKitAppView.swift
//  MockHoloKitApp
//
//  Created by Yuchen Zhang on 2022/10/19.
//

import SwiftUI

struct HoloKitAppView: View {
    
    @EnvironmentObject var watchConnectivityManager: MockHoloKitAppWatchConnectivityManager
    
    var body: some View {
        VStack {
            Text("HoloKit App")
            
            Spacer()
                .frame(height: 50)
            
            Button("Activate") {
                self.watchConnectivityManager.activate()
            }
            
            Spacer()
                .frame(height: 50)
            
            Button("Watch App Installed") {
                print("Is watch app installed: \(self.watchConnectivityManager.isWatchAppInstalled())")
            }
            
            Spacer()
                .frame(height: 50)
            
            Button("Is Reachable") {
                print("Is reachable: \(self.watchConnectivityManager.isReachable())")
            }
            
            Spacer()
                .frame(height: 50)
            
            Button("Go to MOFA") {
                watchConnectivityManager.currentReality = .mofaTheTraining
                watchConnectivityManager.updateCurrentReality(watchConnectivityManager.currentReality.rawValue)
            }
        }
        .padding()
    }
}

struct HoloKitAppView_Previews: PreviewProvider {
    static var previews: some View {
        HoloKitAppView()
            .environmentObject(MockHoloKitAppWatchConnectivityManager())
    }
}
