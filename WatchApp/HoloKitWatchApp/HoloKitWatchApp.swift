//
//  HoloKitAppApp.swift
//  HoloKitApp Watch App
//
//  Created by Yuchen Zhang on 2022/10/6.
//

import SwiftUI

@main
struct HoloKitWatchApp: App {
    
    @StateObject var holokitWatchAppManager = HoloKitWatchAppManager()
    
    @StateObject var mofaWatchAppManager = MofaWatchAppManager()

    var body: some Scene {
        WindowGroup {
            HomeView()
                .environmentObject(holokitWatchAppManager)
                .environmentObject(mofaWatchAppManager)
        }
    }
}
