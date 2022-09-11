//
//  mofawatchApp.swift
//  mofawatch Watch App
//
//  Created by Botao Hu on 9/1/22.
//

import SwiftUI

@main
struct mofawatch_Watch_AppApp: App {
    @StateObject private var viewModel = ViewModel()
        @SceneBuilder var body: some Scene {
            WindowGroup {
                NavigationView {
                    SessionPagingView()
                }
                .sheet(isPresented: $viewModel.showingSummaryView) {
                    SummaryView()
                }
                .environmentObject(viewModel)
            }
        }
}
