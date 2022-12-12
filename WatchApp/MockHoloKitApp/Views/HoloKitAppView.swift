//
//  HoloKitAppView.swift
//  MockHoloKitApp
//
//  Created by Yuchen Zhang on 2022/10/19.
//

import SwiftUI

struct HoloKitAppView: View {
    
    @ObservedObject var holokitAppWatchConnectivityManager = MockHoloKitAppWatchConnectivityManager.shared
    
    var body: some View {
        VStack {
            Text("HoloKit App")
            
            Spacer()
                .frame(height: 50)
            
            HStack {
                Button("Is Watch App Installed") {
                    print("Is watch app installed: \(holokitAppWatchConnectivityManager.isWatchAppInstalled())")
                }
                
                Text(": \(holokitAppWatchConnectivityManager.isWatchAppInstalledVar)" as String)
            }
            
            Spacer()
                .frame(height: 50)
            
            HStack {
                Button("Is Reachable") {
                    print("Is reachable: \(holokitAppWatchConnectivityManager.isReachable())")
                }
                
                Text(": \(holokitAppWatchConnectivityManager.isReachableVar)" as String)
            }
            
            Spacer()
                .frame(height: 50)
            
            Button("Play MOFA") {
                holokitAppWatchConnectivityManager.updatePanel(panelIndex: 1)
            }
        }
        .padding()
    }
}

struct HoloKitAppView_Previews: PreviewProvider {
    static var previews: some View {
        HoloKitAppView()
    }
}
