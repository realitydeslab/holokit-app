import SwiftUI

struct MofaResultView: View {
    
    @EnvironmentObject var holokitWatchAppManager: HoloKitWatchAppManager
    
    @State var result: Bool = true
    
    var body: some View {
        VStack {
            if (self.result) {
                youWinImage
            } else {
                youLoseImage
            }
            //Spacer()
            dataList
                .padding(.top)
                .padding(.bottom)
            //Spacer()
            gotItButton
        }
    }
    
    var youWinImage: some View {
        Image("You_Win")
            .resizable()
            .frame(maxWidth: 140, maxHeight: 30)
    }
    
    var youLoseImage: some View {
        Image("You_Lose")
            .resizable()
            .frame(maxWidth: 140, maxHeight: 30)
    }
    
    var dataList: some View {
        VStack {
            Text("Kills: ")
            Text("Hit Rate: ")
            Text("Dist: ")
            Text("Calorie: ")
        }
        .font(Font.custom("ObjectSans-BoldSlanted", size: 14))
    }
    
    var gotItButton: some View {
        Button {
            self.holokitWatchAppManager.mofaWatchAppManager.currentView = .fightingView
        } label: {
            HStack {
                Text("Got it")
                    .font(Font.custom("ObjectSans-BoldSlanted", size: 14))
                Image("Arrow_Right")
                    .renderingMode(.template)
                    .resizable()
                    .frame(maxWidth: 10, maxHeight: 10)
            }
                .frame(maxWidth: 120, maxHeight: 30)
                .font(.headline)
                .fontWeight(.semibold)
                .foregroundColor(.black)
                .padding()
                .background(Color.white)
        }
        .buttonStyle(.plain)
    }
}

struct MofaResultView_Previews: PreviewProvider {
    static var previews: some View {
        MofaResultView().environmentObject(HoloKitWatchAppManager())
    }
}
