import SwiftUI

struct MofaResultView: View {
    
    @EnvironmentObject var mofaWatchAppManager: MofaWatchAppManager
    
    @State var result: Bool = true
    
    var body: some View {
        VStack {
//            if (self.result) {
//                youWinImage
//            } else {
//                youLoseImage
//            }
            //Spacer()
            dataList
                .padding(.top)
                .padding(.bottom)
            //Spacer()
            gotItButton
        }
    }
    
    var youWinImage: some View {
        Image("you-win")
            .resizable()
            .frame(maxWidth: 140, maxHeight: 30)
    }
    
    var youLoseImage: some View {
        Image("you-lose")
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
            self.mofaWatchAppManager.currentView = .fightingView
        } label: {
            HStack {
                Text("Got it")
                    .font(Font.custom("ObjectSans-BoldSlanted", size: 14))
                Image("arrow-right")
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
        MofaResultView().environmentObject(MofaWatchAppManager())
    }
}
