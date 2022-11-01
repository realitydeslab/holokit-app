import SwiftUI

struct HomeView: View {
    
    @EnvironmentObject var holokitAppWatchManager: HoloKitWatchAppManager
    
    @EnvironmentObject var mofaWatchAppManager: MofaWatchAppManager
    
    var body: some View {
        if (self.holokitAppWatchManager.currentController == .nothing) {
            PanelListView()
                .environmentObject(holokitAppWatchManager)
                .onAppear {
                    DispatchQueue.main.async {
                        holokitAppWatchManager.takeControlWatchConnectivitySession()
                    }
                }
        } else if (self.holokitAppWatchManager.currentController == .mofa) {
            MofaHomeView()
                .environmentObject(mofaWatchAppManager)
                .onAppear {
                    DispatchQueue.main.async {
                        mofaWatchAppManager.takeControlWatchConnectivitySession()
                    }
                }
        }
    }
    
    var defaultIntroView: some View {
        VStack {
            Image("holokit-icon")
                .resizable()
                .frame(maxWidth: 120, maxHeight: 120)
                .padding(.bottom)
            Text("HoloKit")
                .font(Font.custom("ObjectSans-BoldSlanted", size: 16))
        }
    }
}

struct HomeView_Previews: PreviewProvider {
    static var previews: some View {
        HomeView()
            .environmentObject(HoloKitWatchAppManager())
            .environmentObject(MofaWatchAppManager())
    }
}
