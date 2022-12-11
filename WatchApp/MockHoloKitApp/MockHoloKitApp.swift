//
//  HoloKitAppApp.swift
//  HoloKitApp
//
//  Created by Yuchen Zhang on 2022/10/6.
//

import SwiftUI

@main
struct MockHoloKitApp: App {
    
    @State var holokitAppWatchConnectivityManager = MockHoloKitAppWatchConnectivityManager()
    
    @State var mofaWatchConnectivityManager = MockMofaWatchConnectivityManager()
    
    var body: some Scene {
        WindowGroup {
            RootView()
                .environmentObject(holokitAppWatchConnectivityManager)
                .environmentObject(mofaWatchConnectivityManager)
        }
    }
}
