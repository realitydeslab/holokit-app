// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

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
