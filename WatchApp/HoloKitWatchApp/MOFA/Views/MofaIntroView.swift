import SwiftUI

struct MofaIntroView: View {
    
    @EnvironmentObject var mofaWatchAppManager: MofaWatchAppManager
    
    var body: some View {
        VStack {
            Spacer()
            Text("Reality #002")
                .font(Font.custom("ObjectSans-Regular", size: 13))
                .padding()
            Text("MOFA - The Training")
                .font(Font.custom("ObjectSans-BoldSlanted", size: 18))
            Spacer()
            
            startButton
        }
    }
    
    var startButton: some View {
        Button {
            self.mofaWatchAppManager.currentView = .fightingView
        } label: {
            ZStack {
                Rectangle()
                    .foregroundColor(.white)
                    .frame(maxWidth: 100, maxHeight: 40)
                
                Text("Start")
                    .font(Font.custom("ObjectSans-BoldSlanted", size: 16))
                    .foregroundColor(.black)
            }
        }
        .buttonStyle(.plain)
    }
}

struct MofaIntroView_Previews: PreviewProvider {
    static var previews: some View {
        MofaIntroView()
            .environmentObject(MofaWatchAppManager())
    }
}
