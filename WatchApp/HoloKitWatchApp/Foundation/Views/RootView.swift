// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

import SwiftUI

struct RootView: View {
    
    @ObservedObject var holokitWatchAppManager = HoloKitWatchAppManager.shared
    
    var body: some View {
        if (holokitWatchAppManager.panel == .none) {
            PanelListView()
        } else if (holokitWatchAppManager.panel == .mofa) {
            MofaRootView()
                .onAppear {
                    holokitWatchAppManager.mofaWatchAppManager.onAppear()
                }
                .onDisappear {
                    holokitWatchAppManager.mofaWatchAppManager.OnDisappear()
                }
        }
    }
    
    var defaultIntroView: some View {
        VStack {
            Image("holokit-icon")
                .resizable()
                .frame(maxWidth: 120, maxHeight: 120)
                .padding(.bottom)
            Text("HoloKit")
                .font(Font.custom("ObjectSans-BoldSlanted", size: 16))
        }
    }
}

struct HomeView_Previews: PreviewProvider {
    static var previews: some View {
        RootView()
    }
}
