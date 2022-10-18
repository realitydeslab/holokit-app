import SwiftUI

struct HomeView: View {
    
    @EnvironmentObject var holokitAppWatchManager: HoloKitWatchAppManager
    
    @EnvironmentObject var mofaWatchAppManager: MofaWatchAppManager
    
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
                .environmentObject(self.holokitAppWatchManager.mofaWatchAppManager)
                .onAppear {
                    DispatchQueue.main.async {
                        mofaWatchAppManager.GiveWCSessionDelegateControl()
                    }
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
        HomeView()
            .environmentObject(HoloKitWatchAppManager())
            .environmentObject(MofaWatchAppManager())
    }
}
