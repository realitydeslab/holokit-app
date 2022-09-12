//
//  SessionPagingView.swift
//  magikverse WatchKit Extension
//
//  Created by Yuchen Zhang on 2022/5/25.
//

import SwiftUI

enum Tab {
    case controls, fighting
}

struct SessionPagingView: View {
    
    @EnvironmentObject var viewModel: ViewModel
    @State private var selection: Tab = .controls
    
    var body: some View {
        TabView(selection: $selection) {
            ControlsView(selection: $selection).tag(Tab.controls)
            FightingView().tag(Tab.fighting)
        }
    }
}
