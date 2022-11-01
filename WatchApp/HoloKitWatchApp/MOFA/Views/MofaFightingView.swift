import SwiftUI

struct MofaFightingView: View {
    
    @EnvironmentObject var mofaWatchAppManager: MofaWatchAppManager
    
    var body: some View {
        VStack {
            HStack {
                Button {
                    mofaWatchAppManager.holokitWatchAppManager?.currentController = .nothing
                } label: {
                    Image("back")
                        .resizable()
                        .foregroundColor(.white)
                        .frame(maxWidth: 24, maxHeight: 24)
                }
                .buttonStyle(.plain)
                Spacer()
            }
            
            Image("mofa-weapon")
                .resizable()
                .frame(maxWidth: 120, maxHeight: 120)
            
            Spacer()
            
            if (self.mofaWatchAppManager.isFighting) {
                fightingText
            } else {
                startButton
            }
        }
    }
    
    var startButton: some View {
        Button {
            self.mofaWatchAppManager.sendStartRoundMessage()
        } label: {
            ZStack {
                Rectangle()
                    .frame(maxWidth: 100, maxHeight: 40)
                
                HStack {
                    Text("Ready")
                        .font(Font.custom("ObjectSans-BoldSlanted", size: 13))
                        .foregroundColor(.black)
                    
                    Image("arrow-right")
                        .renderingMode(.template)
                        .resizable()
                        .frame(maxWidth: 10, maxHeight: 10)
                        .foregroundColor(.black)
                }
            }
        }
        .buttonStyle(.plain)
    }
    
    var fightingText: some View {
        Text("SWING YOUR ARM TO CAST THE SPELL")
            .multilineTextAlignment(.center)
            .font(Font.custom("ObjectSans-BoldSlanted", size: 14))
    }
}

struct MofaFightingView_Previews: PreviewProvider {
    static var previews: some View {
        MofaFightingView()
            .environmentObject(MofaWatchAppManager())
    }
}
