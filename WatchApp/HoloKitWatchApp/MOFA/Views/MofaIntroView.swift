import SwiftUI

struct MofaIntroView: View {
    
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
            

            Spacer()
            Text("Panel #001")
                .font(Font.custom("ObjectSans-Regular", size: 13))
                .padding()
            Text("MOFA")
                .font(Font.custom("ObjectSans-BoldSlanted", size: 18))
            Spacer()
            
            startButton
        }
    }
    
    var startButton: some View {
        Button {
            self.mofaWatchAppManager.sendStartRoundMessage()
        } label: {
            ZStack {
                Rectangle()
                    .foregroundColor(.white)
                    .frame(maxWidth: 100, maxHeight: 40)
                
                Text("Ready")
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
