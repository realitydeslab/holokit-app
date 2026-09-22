// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

import SwiftUI

struct MofaFightingView: View {
    
    @ObservedObject var mofaWatchAppManager = HoloKitWatchAppManager.shared.mofaWatchAppManager
    
    var body: some View {
        VStack {
//            HStack {
//                Button {
//                    mofaWatchAppManager.holokitWatchAppManager?.currentPanel = .none
//                } label: {
//                    Image("back")
//                        .resizable()
//                        .foregroundColor(.white)
//                        .frame(maxWidth: 24, maxHeight: 24)
//                }
//                .buttonStyle(.plain)
//                Spacer()
//            }
            
            Spacer()
            spellImage
            Spacer()
            fightingText
        }
    }
    
    var spellImage: some View {
        Image("mofa-spell-" + String(self.mofaWatchAppManager.magicSchool.rawValue))
            .resizable()
            .aspectRatio(contentMode: .fill)
            .frame(maxWidth: 100, maxHeight: 100)
    }
    
    var fightingText: some View {
        Text("SWING YOUR ARM TO CAST SPELLS")
            .multilineTextAlignment(.center)
            .font(Font.custom("ObjectSans-BoldSlanted", size: 14))
    }
}

struct MofaFightingView_Previews: PreviewProvider {
    static var previews: some View {
        MofaFightingView()
    }
}
