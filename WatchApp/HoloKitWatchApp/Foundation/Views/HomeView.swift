import SwiftUI

struct HomeView: View {
    
    @EnvironmentObject var holokitAppWatchManager: HoloKitAppWatchManager

    @StateObject private var mofaWatchManager = MofaWatchManager()
    
    var body: some View {
        if (self.holokitAppWatchManager.currentReality == .nothing) {
            VStack {
                defaultIntroView
                    .padding(.bottom)
                Text("HoloKit")
                    .font(Font.custom("ObjectSans-BoldSlanted", size: 16))
            }
        } else if (self.holokitAppWatchManager.currentReality == .mofaTheTraining) {
            MofaHomeView()
                .environmentObject(self.mofaWatchManager)
                .onAppear {
                    self.mofaWatchManager.InitializeMofaWCSessionDelegate()
                }
        } else if (self.holokitAppWatchManager.currentReality == .mofaTheDuel) {
            
        }
    }
    
    var defaultIntroView: some View {
        Image("HoloKit")
            .resizable()
            .frame(maxWidth: 120, maxHeight: 120)
    }
}

struct HomeView_Previews: PreviewProvider {
    static var previews: some View {
        HomeView().environmentObject(HoloKitAppWatchManager())
    }
}
