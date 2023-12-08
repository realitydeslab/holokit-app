// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
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
