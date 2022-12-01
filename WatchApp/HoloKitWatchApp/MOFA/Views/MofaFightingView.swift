import SwiftUI

struct MofaFightingView: View {
    
    @EnvironmentObject var mofaWatchAppManager: MofaWatchAppManager
    
    var body: some View {
        VStack {
//            HStack {
//                Button {
//                    mofaWatchAppManager.holokitWatchAppManager?.currentPanel = .none
//                } label: {
//                    Image("back")
//                        .resizable()
//                        .foregroundColor(.white)
//                        .frame(maxWidth: 24, maxHeight: 24)
//                }
//                .buttonStyle(.plain)
//                Spacer()
//            }
            
            Spacer()
            spellImage
            Spacer()
            fightingText
        }
    }
    
    var spellImage: some View {
        Image("mofa-spell-" + String(self.mofaWatchAppManager.magicSchool.rawValue))
            .resizable()
            .aspectRatio(contentMode: .fill)
            .frame(maxWidth: 100, maxHeight: 100)
    }
    
//    var startButton: some View {
//        Button {
//            self.mofaWatchAppManager.sendStartRoundMessage()
//        } label: {
//            ZStack {
//                Rectangle()
//                    .frame(maxWidth: 100, maxHeight: 40)
//                
//                HStack {
//                    Text("Ready")
//                        .font(Font.custom("ObjectSans-BoldSlanted", size: 13))
//                        .foregroundColor(.black)
//                    
//                    Image("arrow-right")
//                        .renderingMode(.template)
//                        .resizable()
//                        .frame(maxWidth: 10, maxHeight: 10)
//                        .foregroundColor(.black)
//                }
//            }
//        }
//        .buttonStyle(.plain)
//    }
    
    var fightingText: some View {
        Text("SWING YOUR ARM TO CAST SPELLS")
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
