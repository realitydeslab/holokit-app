import SwiftUI

struct MofaFightingView: View {
    
    @EnvironmentObject var mofaWatchManager: MofaWatchAppManager
    
    @State var isFighting: Bool = false
    
    var body: some View {
        VStack {
            Image("MOFA_Weapon")
                .resizable()
                .frame(maxWidth: 120, maxHeight: 120)
            
            Spacer()
            
            if (self.mofaWatchManager.isFighting) {
                fightingText
            } else {
                startButton
            }
        }
    }
    
    var startButton: some View {
        Button {
            self.mofaWatchManager.isFighting = true
        } label: {
            ZStack {
                Rectangle()
                    .frame(maxWidth: 100, maxHeight: 40)
                
                HStack {
                    Text("Start")
                        .font(Font.custom("ObjectSans-BoldSlanted", size: 13))
                        .foregroundColor(.black)
                    
                    Image("Arrow_Right")
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
        MofaFightingView().environmentObject(MofaWatchAppManager())
    }
}
