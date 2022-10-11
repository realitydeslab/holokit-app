//
//  HoloKitAppApp.swift
//  HoloKitApp Watch App
//
//  Created by Yuchen Zhang on 2022/10/6.
//

import SwiftUI

@main
struct HoloKitWatchApp: App {
    @StateObject private var holokitAppWatch = HoloKitWatchAppManager()

    //@StateObject private var mofaWatchManager = MofaWatchManager()
    
    var body: some Scene {
        WindowGroup {
            HomeView()
                .environmentObject(holokitAppWatch)
        }
    }
}
