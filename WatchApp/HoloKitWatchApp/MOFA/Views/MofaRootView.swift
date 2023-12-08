// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

import SwiftUI

struct MofaRootView: View {
    
    @ObservedObject var mofaWatchAppManager = HoloKitWatchAppManager.shared.mofaWatchAppManager
    
    var body: some View {
        if (mofaWatchAppManager.view == .readyView) {
            MofaReadyView()
        } else if (mofaWatchAppManager.view == .handednessView) {
            MofaHandednessView()
        } else if (mofaWatchAppManager.view == .fightingView) {
            MofaFightingView()
        } else if (mofaWatchAppManager.view == .resultView) {
            MofaResultView()
        }
    }
}

struct MofaHomeView_Previews: PreviewProvider {
    static var previews: some View {
        MofaRootView()
    }
}
