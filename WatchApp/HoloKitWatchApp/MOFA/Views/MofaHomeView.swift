import SwiftUI

struct MofaHomeView: View {
    
    @EnvironmentObject var holokitWatchAppManager: HoloKitWatchAppManager
    
    var body: some View {
        if (self.holokitWatchAppManager.mofaWatchAppManager.currentView == .introView) {
            MofaIntroView()
        } else if (self.holokitWatchAppManager.mofaWatchAppManager.currentView == .handednessView) {
            MofaHandednessView()
        } else if (self.holokitWatchAppManager.mofaWatchAppManager.currentView == .fightingView) {
            MofaFightingView()
        } else if (self.holokitWatchAppManager.mofaWatchAppManager.currentView == .resultView) {
            MofaResultView()
        }
    }
}

struct MofaHomeView_Previews: PreviewProvider {
    static var previews: some View {
        MofaHomeView().environmentObject(HoloKitWatchAppManager())
    }
}
