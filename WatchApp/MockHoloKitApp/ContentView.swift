import SwiftUI

struct ContentView: View {
    
    @EnvironmentObject var watchConnectivityManager: MockHoloKitAppWatchConnectivityManager
    
    var body: some View {
        VStack {
            Button("Activate") {
                self.watchConnectivityManager.Activate()
            }
            
            Spacer(minLength: 10)
            
            Button("Paired") {
                print("Has paired apple watch: \(self.watchConnectivityManager.HasPairedAppleWatch())")
            }
            
            Spacer(minLength: 10)
            
            Button("Watch App Installed") {
                print("Is watch app installed: \(self.watchConnectivityManager.IsWatchAppInstalled())")
            }
            
            Spacer(minLength: 10)
            
            Button("Is Reachable") {
                print("Is reachable: \(self.watchConnectivityManager.IsReachable())")
            }
            
            Spacer(minLength: 10)
            
            Button("Send Message") {
                self.watchConnectivityManager.SendMessage()
            }
            
//            Spacer(minLength: 10)
//            
//            Button("Update Current Reality") {
//                self.watchConnectivityManager.UpdateCurrentReality(1)
//            }
        }
        .padding()
    }
}

struct ContentView_Previews: PreviewProvider {
    static var previews: some View {
        ContentView()
            .environmentObject(MockHoloKitAppWatchConnectivityManager())
    }
}
