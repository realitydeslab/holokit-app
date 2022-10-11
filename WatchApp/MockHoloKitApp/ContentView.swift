//
//  ContentView.swift
//  HoloKitApp
//
//  Created by Yuchen Zhang on 2022/10/6.
//

import SwiftUI

struct ContentView: View {
    
    @EnvironmentObject var holokitWatchAppManager: MockHoloKitWatchAppManager
    
    var body: some View {
        VStack {
            Image(systemName: "globe")
                .imageScale(.large)
                .foregroundColor(.accentColor)
            Text("Hello, world!")
            
            Button("Is Watch App Installed") {
                _ = self.holokitWatchAppManager.isWatchAppInstalled()
            }
            
            Button ("Initialize with RealityId") {
                self.holokitWatchAppManager.initializeWithRealityId(realityId: 1)
            }
        }
        .padding()
    }
}

struct ContentView_Previews: PreviewProvider {
    static var previews: some View {
        ContentView()
            .environmentObject(MockHoloKitWatchAppManager())
    }
}
