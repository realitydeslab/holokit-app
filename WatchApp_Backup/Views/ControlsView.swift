//
//  ControlsView.swift
//  magikverse WatchKit Extension
//
//  Created by Yuchen Zhang on 2022/5/25.
//

import SwiftUI

struct ControlsView: View {
    
    @EnvironmentObject var viewModel: ViewModel
    @Binding var selection: Tab
    
    var body: some View {
        HStack {
            if viewModel.isFighting {
                stopButton
            } else {
                VStack {
                    startButton
                    
                    if (viewModel.isWorkoutSessionRunning) {
                        stopButton
                    }
                }
                
            }
        }
    }
    
    var startButton: some View {
        VStack {
            Text("Start")
                .font(.title3)
            Button {
                viewModel.startRound()
                selection = .fighting
            } label: {
                Image(systemName: "play")
            }
            .font(.title)
            .tint(.yellow)
            .frame(width: 80)
        }
    }
    
    var stopButton: some View {
        VStack {
            Text("Stop")
                .font(.title3)
            
            Button {
                viewModel.endWorkout()
            } label: {
                Image(systemName: "xmark")
            }
            .font(.title2)
            .tint(.red)
            .frame(width: 80)
        }
    }
}

