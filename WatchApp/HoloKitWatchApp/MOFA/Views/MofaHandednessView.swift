import SwiftUI

struct MofaHandednessView: View {
    
    @EnvironmentObject var mofaWatchManager: MofaWatchAppManager

    var body: some View {
        VStack {
            Text("The watch is on your")
                .font(Font.custom("ObjectSans-BoldSlanted", size: 13))
                .padding(.bottom)
            Spacer()
            rightHandButton
                .padding(.bottom)
            Spacer()
            leftHandButton
                .padding(.bottom)
            Spacer()
            rememberSelectionTick
        }
    }
    
    var rightHandButton: some View {
        Button {
            self.mofaWatchManager.isRightHand = true
            self.mofaWatchManager.currentView = .fightingView
        } label: {
            ZStack {
                Rectangle()
                    .frame(maxWidth: 120, maxHeight: 50)
                    .foregroundColor(self.mofaWatchManager.isRightHand ? .white : .black)
                HStack {
                    Text("Right Hand")
                        .font(Font.custom("ObjectSans-BoldSlanted", size: 13))
                    Image("Arrow_Right")
                        .resizable()
                        .frame(maxWidth: 16, maxHeight: 16)
                }
                .foregroundColor(self.mofaWatchManager.isRightHand ? .black : .white)
            }
        }
        .buttonStyle(.plain)
    }
    
    var leftHandButton: some View {
        Button {
            self.mofaWatchManager.isRightHand = false
            self.mofaWatchManager.currentView = .fightingView
        } label: {
            ZStack {
                Rectangle()
                    .frame(maxWidth: 120, maxHeight: 50)
                    .foregroundColor(self.mofaWatchManager.isRightHand ? .black : .white)
                    .border(Color.white)
                HStack {
                    Text("Left Hand")
                        .font(Font.custom("ObjectSans-BoldSlanted", size: 13))
                    Image("Arrow_Right")
                        .renderingMode(.template)
                        .resizable()
                        .frame(maxWidth: 16, maxHeight: 16)

                }
                .foregroundColor(self.mofaWatchManager.isRightHand ? .white : .black)

            }
        }
        .buttonStyle(.plain)
    }
    
    var rememberSelectionTick: some View {
        HStack {
            Button {
                self.mofaWatchManager.rememberHandedness.toggle()
            } label: {
                if (self.mofaWatchManager.rememberHandedness){
                    Circle()
                    .frame(maxWidth: 20, maxHeight: 20)
                } else {
                    Circle()
                    .stroke(Color.white, lineWidth: 2)
                    .frame(maxWidth: 20, maxHeight: 20)
                }
            }
            .buttonStyle(.plain)
            
            Text("Remember my selection")
                .font(Font.custom("ObjectSans-BoldSlanted", size: 13))
        }
    }
}

struct MofaHandednessView_Previews: PreviewProvider {
    static var previews: some View {
        MofaHandednessView().environmentObject(MofaWatchAppManager())
    }
}
