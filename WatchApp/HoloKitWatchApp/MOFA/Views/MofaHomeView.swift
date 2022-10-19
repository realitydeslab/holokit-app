import SwiftUI

struct MofaHomeView: View {
    
    @EnvironmentObject var mofaWatchAppManager: MofaWatchAppManager
    
    var body: some View {
        if (self.mofaWatchAppManager.currentView == .introView) {
            MofaIntroView()
        } else if (self.mofaWatchAppManager.currentView == .handednessView) {
            MofaHandednessView()
        } else if (self.mofaWatchAppManager.currentView == .fightingView) {
            MofaFightingView()
        } else if (self.mofaWatchAppManager.currentView == .resultView) {
            MofaResultView()
        }
    }
}

struct MofaHomeView_Previews: PreviewProvider {
    static var previews: some View {
        MofaHomeView().environmentObject(MofaWatchAppManager())
    }
}
