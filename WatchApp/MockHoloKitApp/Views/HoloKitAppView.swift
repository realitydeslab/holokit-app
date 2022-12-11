//
//  HoloKitAppView.swift
//  MockHoloKitApp
//
//  Created by Yuchen Zhang on 2022/10/19.
//

import SwiftUI

struct HoloKitAppView: View {
    
    @EnvironmentObject var holokitAppWatchConnectivityManager: MockHoloKitAppWatchConnectivityManager
    
    var body: some View {
        VStack {
            Text("HoloKit App")
            
            Spacer()
                .frame(height: 50)
            
            HStack {
                Button("Is Watch App Installed") {
                    print("Is watch app installed: \(self.holokitAppWatchConnectivityManager.isWatchAppInstalled())")
                }
                
                Text(": \(self.holokitAppWatchConnectivityManager.isWatchAppInstalledVar)" as String)
            }
            
            Spacer()
                .frame(height: 50)
            
            HStack {
                Button("Is Reachable") {
                    print("Is reachable: \(self.holokitAppWatchConnectivityManager.isReachable())")
                }
                
                Text(": \(self.holokitAppWatchConnectivityManager.isReachableVar)" as String)
            }
            
            Spacer()
                .frame(height: 50)
            
            Button("Play MOFA") {
                holokitAppWatchConnectivityManager.currentWatchPanel = .mofa
                holokitAppWatchConnectivityManager.updateCurrentWatchPanel(holokitAppWatchConnectivityManager.currentWatchPanel.rawValue)
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
