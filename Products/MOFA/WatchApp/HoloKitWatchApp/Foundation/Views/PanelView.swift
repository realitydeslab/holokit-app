// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

import SwiftUI

struct PanelView: View {
    
    @ObservedObject var holokitWatchAppManager = HoloKitWatchAppManager.shared
    
    let panelIndex: Int
    
    let panelName: String
    
    var body: some View {
        HStack {
            Spacer()
            VStack {
                Text("Panel #00\(panelIndex)")
                    .font(Font.custom("ObjectSans-Regular", size: 16))
                
                Button {
                    holokitWatchAppManager.panel = HoloKitWatchPanel(rawValue: panelIndex) ?? .none
                } label: {
                    ZStack {
                        Rectangle()
                            .foregroundColor(.white)
                            .frame(minWidth: 40, maxWidth: .infinity, minHeight: 40, maxHeight: 40)
                        
                        HStack(alignment: .top) {
                            Image("arrow-right")
                                .resizable()
                                .frame(maxWidth: 16, maxHeight: 16)
                            Text("MOFA")
                        }
                        .font(Font.custom("ObjectSans-BoldSlanted", size: 22))
                        .foregroundColor(.black)
                    }
                }
                .buttonStyle(.plain)
            }
            Spacer()
        }
    }
}

struct PanelView_Previews: PreviewProvider {
    static var previews: some View {
        PanelView(panelIndex: 1, panelName: "MOFA")
    }
}
