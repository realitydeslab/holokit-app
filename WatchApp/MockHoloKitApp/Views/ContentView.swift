import SwiftUI

struct ContentView: View {
    
    @EnvironmentObject var holokitAppWatchConnectivityManager: MockHoloKitAppWatchConnectivityManager
    
    @EnvironmentObject var mofaWatchConnectivityManager: MockMofaWatchConnectivityManager
    
    var body: some View {
        if (holokitAppWatchConnectivityManager.currentReality == .nothing) {
            HoloKitAppView()
                .environmentObject(self.holokitAppWatchConnectivityManager)
        } else if (holokitAppWatchConnectivityManager.currentReality == .mofaTheTraining) {
            MofaView()
                .environmentObject(self.mofaWatchConnectivityManager)
        }
    }
}

struct ContentView_Previews: PreviewProvider {
    static var previews: some View {
        ContentView()
            .environmentObject(MockHoloKitAppWatchConnectivityManager())
            .environmentObject(MockMofaWatchConnectivityManager())
    }
}
