import SwiftUI

struct RootView: View {
    
    @EnvironmentObject var holokitAppWatchConnectivityManager: MockHoloKitAppWatchConnectivityManager
    
    @EnvironmentObject var mofaWatchConnectivityManager: MockMofaWatchConnectivityManager
    
    var body: some View {
        if (holokitAppWatchConnectivityManager.currentWatchPanel == .none) {
            HoloKitAppView()
                .environmentObject(self.holokitAppWatchConnectivityManager)
                .onAppear {
                    self.holokitAppWatchConnectivityManager.takeControlWatchConnectivitySession()
                }
        } else if (holokitAppWatchConnectivityManager.currentWatchPanel == .mofa) {
            MofaView()
                .environmentObject(self.mofaWatchConnectivityManager)
                .environmentObject(self.holokitAppWatchConnectivityManager)
                .onAppear {
                    self.mofaWatchConnectivityManager.takeControlWatchConnectivitySession()
                }
        }
    }
}

struct ContentView_Previews: PreviewProvider {
    static var previews: some View {
        RootView()
            .environmentObject(MockHoloKitAppWatchConnectivityManager())
            .environmentObject(MockMofaWatchConnectivityManager())
    }
}
