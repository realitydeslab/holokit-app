import SwiftUI

struct RootView: View {
    
    @ObservedObject var holokitAppWatchConnectivityManager = MockHoloKitAppWatchConnectivityManager.shared
    
    var body: some View {
        ZStack {
            if (holokitAppWatchConnectivityManager.panel == .none) {
                HoloKitAppView()
            } else if (holokitAppWatchConnectivityManager.panel == .mofa) {
                MofaView()
            }
        }
        .onDisappear {
            print("RootView onDisappear")
            holokitAppWatchConnectivityManager.updatePanel(panelIndex: 0)
        }
    }
}

struct ContentView_Previews: PreviewProvider {
    static var previews: some View {
        RootView()
    }
}
