import SwiftUI

struct MofaHomeView: View {
    
    @EnvironmentObject var mofaWatchManager: MofaWatchManager
    
    var body: some View {
        if (self.mofaWatchManager.currentView == .introView) {
            MofaIntroView()
        } else if (self.mofaWatchManager.currentView == .handednessView) {
            MofaHandednessView()
        } else if (self.mofaWatchManager.currentView == .fightingView) {
            MofaFightingView()
        } else if (self.mofaWatchManager.currentView == .resultView) {
            MofaResultView()
        }
    }
}

struct MofaHomeView_Previews: PreviewProvider {
    static var previews: some View {
        MofaHomeView().environmentObject(MofaWatchManager())
    }
}
