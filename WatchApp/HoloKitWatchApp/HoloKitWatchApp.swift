//
//  HoloKitAppApp.swift
//  HoloKitApp Watch App
//
//  Created by Yuchen Zhang on 2022/10/6.
//

import SwiftUI

@main
struct HoloKitWatchApp: App {
    
    @StateObject private var holokitWatchAppManager = HoloKitWatchAppManager()

    var body: some Scene {
        WindowGroup {
            HomeView()
                .environmentObject(holokitWatchAppManager)
        }
    }
}
