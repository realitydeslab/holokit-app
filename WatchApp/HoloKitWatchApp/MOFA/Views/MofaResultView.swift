import SwiftUI
import Foundation

struct MofaResultView: View {
    
    @EnvironmentObject var mofaWatchAppManager: MofaWatchAppManager
    
    @State var result: Bool = true
    
    var body: some View {
        VStack {
            resultText
            //Spacer()
            dataList
                .padding(.top, 5)
                .padding(.bottom, 5)
            playAgainButton
        }
    }
    
    var resultText: some View {
        var resultText: Text
        if (self.mofaWatchAppManager.roundResult == .victory) {
            resultText = Text("Victory")
        } else if (self.mofaWatchAppManager.roundResult == .defeat) {
            resultText = Text("Defeat")
        } else {
            resultText = Text("Draw")
        }
        return resultText
            .font(Font.custom("ObjectSans-Bold", size: 20))
    }
    
    var dataList: some View {
        HStack {
            VStack (alignment: .leading, spacing: 5) {
                Text("Kills: \(self.mofaWatchAppManager.kill)")
                Text("Hit Rate: \(self.mofaWatchAppManager.hitRate)%")
                Text("Dist: \(Int(self.mofaWatchAppManager.distance * self.mofaWatchAppManager.meterToFeet)) ft")
                Text("Energy: \(Int(self.mofaWatchAppManager.activeEnergy)) kcal")
            }
            .font(Font.custom("ObjectSans-BoldSlanted", size: 12))
            
            Spacer()
            
            MofaActivityRingsView(healthStore: self.mofaWatchAppManager.healthStore)
                .frame(width: 40, height: 40)
        }
        .padding(.horizontal, 8)
    }
    
    var playAgainButton: some View {
        Button {
            self.mofaWatchAppManager.currentView = .readyView
        } label: {
            HStack {
                Text("Play Again")
                    .font(Font.custom("ObjectSans-BoldSlanted", size: 14))
                Image("arrow-right")
                    .renderingMode(.template)
                    .resizable()
                    .frame(maxWidth: 10, maxHeight: 10)
            }
            .font(.headline)
            .fontWeight(.semibold)
            .padding(6)
            .foregroundColor(.black)
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
