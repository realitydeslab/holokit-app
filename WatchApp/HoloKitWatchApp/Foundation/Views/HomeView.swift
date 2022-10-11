import SwiftUI

struct HomeView: View {
    
    @EnvironmentObject var holokitAppWatchManager: HoloKitWatchAppManager

    @StateObject private var mofaWatchManager = MofaWatchAppManager()
    
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
        HomeView().environmentObject(HoloKitWatchAppManager())
    }
}
