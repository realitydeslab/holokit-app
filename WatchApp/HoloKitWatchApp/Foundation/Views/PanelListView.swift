// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

import SwiftUI

struct PanelListView: View {
    
    var body: some View {
        List {
            PanelView(panelIndex: 1, panelName: "MOFA")
                .listRowPlatterColor(.clear)
        }
    }
}

struct PanelListView_Previews: PreviewProvider {
    static var previews: some View {
        PanelListView()
    }
}
