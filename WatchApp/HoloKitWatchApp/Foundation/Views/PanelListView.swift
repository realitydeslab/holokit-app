//
//  PanelListView.swift
//  HoloKitWatchApp
//
//  Created by Yuchen Zhang on 2022/11/1.
//

import SwiftUI

struct PanelListView: View {
    
    @EnvironmentObject var holokitAppWatchManager: HoloKitWatchAppManager
    
    var body: some View {
        List {
            PanelView(panelIndex: 1, panelName: "MOFA")
                .environmentObject(holokitAppWatchManager)
                .listRowPlatterColor(.clear)
        }
        
    }
}

struct PanelListView_Previews: PreviewProvider {
    static var previews: some View {
        PanelListView()
            .environmentObject(HoloKitWatchAppManager())
    }
}
