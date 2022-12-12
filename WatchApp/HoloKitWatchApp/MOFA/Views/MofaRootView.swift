import SwiftUI

struct MofaRootView: View {
    
    @ObservedObject var mofaWatchAppManager = HoloKitWatchAppManager.shared.mofaWatchAppManager
    
    var body: some View {
        if (mofaWatchAppManager.view == .readyView) {
            MofaReadyView()
        } else if (mofaWatchAppManager.view == .handednessView) {
            MofaHandednessView()
        } else if (mofaWatchAppManager.view == .fightingView) {
            MofaFightingView()
        } else if (mofaWatchAppManager.view == .resultView) {
            MofaResultView()
        }
    }
}

struct MofaHomeView_Previews: PreviewProvider {
    static var previews: some View {
        MofaRootView()
    }
}
