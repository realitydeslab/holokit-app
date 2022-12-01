//
//  PanelView.swift
//  HoloKitWatchApp
//
//  Created by Yuchen Zhang on 2022/11/1.
//

import SwiftUI

struct PanelView: View {
    
    @EnvironmentObject var holokitAppWatchManager: HoloKitWatchAppManager
    
    let panelIndex: Int
    
    let panelName: String
    
    var body: some View {
        HStack {
            Spacer()
            VStack {
                Text("Panel #00\(panelIndex)")
                    .font(Font.custom("ObjectSans-Regular", size: 16))
                
                Button {
                    holokitAppWatchManager.currentPanel = HoloKitWatchAppPanel(rawValue: panelIndex) ?? .none
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
