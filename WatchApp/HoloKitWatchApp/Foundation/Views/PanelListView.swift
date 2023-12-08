// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
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
