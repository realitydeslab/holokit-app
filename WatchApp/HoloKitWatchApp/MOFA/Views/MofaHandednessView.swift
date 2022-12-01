import SwiftUI

struct MofaHandednessView: View {
    
    @EnvironmentObject var mofaWatchAppManager: MofaWatchAppManager

    var body: some View {
        VStack {
            HStack {
                Button {
                    mofaWatchAppManager.currentView = .readyView
                } label: {
                    Image("back")
                        .resizable()
                        .foregroundColor(.white)
                        .frame(maxWidth: 24, maxHeight: 24)
                }
                .buttonStyle(.plain)
                Spacer()
            }
            
            Text("The watch is on your")
                .font(Font.custom("ObjectSans-BoldSlanted", size: 13))
                .padding(.bottom)
            Spacer()
            rightHandButton
                .padding(.bottom)
            Spacer()
            leftHandButton
                .padding(.bottom)
        }
    }
    
    var rightHandButton: some View {
        Button {
            self.mofaWatchAppManager.isRightHanded = true
        } label: {
            ZStack {
                Rectangle()
                    .frame(maxWidth: 120, maxHeight: 50)
                    .foregroundColor(self.mofaWatchAppManager.isRightHanded ? .white : .black)
                    .border(Color.white)
                HStack {
                    Text("Right Hand")
                        .font(Font.custom("ObjectSans-BoldSlanted", size: 13))
                    Image("arrow-right")
                        .renderingMode(.template)
                        .resizable()
                        .frame(maxWidth: 16, maxHeight: 16)
                }
                .foregroundColor(self.mofaWatchAppManager.isRightHanded ? .black : .white)
            }
        }
        .buttonStyle(.plain)
    }
    
    var leftHandButton: some View {
        Button {
            self.mofaWatchAppManager.isRightHanded = false
        } label: {
            ZStack {
                Rectangle()
                    .frame(maxWidth: 120, maxHeight: 50)
                    .foregroundColor(self.mofaWatchAppManager.isRightHanded ? .black : .white)
                    .border(Color.white)
                HStack {
                    Text("Left Hand")
                        .font(Font.custom("ObjectSans-BoldSlanted", size: 13))
                    Image("arrow-right")
                        .renderingMode(.template)
                        .resizable()
                        .frame(maxWidth: 16, maxHeight: 16)

                }
                .foregroundColor(self.mofaWatchAppManager.isRightHanded ? .white : .black)

            }
        }
        .buttonStyle(.plain)
    }
}

struct MofaHandednessView_Previews: PreviewProvider {
    static var previews: some View {
        MofaHandednessView().environmentObject(MofaWatchAppManager())
    }
}
