//
//  HoloKitAppApp.swift
//  HoloKitApp
//
//  Created by Yuchen Zhang on 2022/10/6.
//

import SwiftUI

@main
struct MockHoloKitApp: App {
    
    @State var holokitWatchAppManager = MockHoloKitWatchAppManager()
    
    var body: some Scene {
        WindowGroup {
            ContentView()
                .environmentObject(holokitWatchAppManager)
        }
    }
}
