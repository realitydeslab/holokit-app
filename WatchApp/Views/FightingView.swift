//
//  FightingView.swift
//  magikverse WatchKit Extension
//
//  Created by Yuchen Zhang on 2022/5/25.
//

import SwiftUI

struct FightingView: View {
    
    @EnvironmentObject var viewModel: ViewModel
    
    var body: some View {
        if viewModel.isFighting {
            Text("Fighting!")
                .font(.title2)
        } else {
            if viewModel.isWorkoutSessionRunning {
                Text("Swipe left to fight again")
                    .font(.headline)
            } else {
                Text("Swipe left to start fighting")
                    .font(.headline)
            }
        }
    }
}
