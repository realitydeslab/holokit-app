// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

import SwiftUI

struct MofaHandednessView: View {
    
    @ObservedObject var mofaWatchAppManager = HoloKitWatchAppManager.shared.mofaWatchAppManager

    var body: some View {
        VStack {
            HStack {
                Button {
                    mofaWatchAppManager.view = .readyView
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
            mofaWatchAppManager.handedness = .right
        } label: {
            ZStack {
                Rectangle()
                    .frame(maxWidth: 120, maxHeight: 50)
                    .foregroundColor(mofaWatchAppManager.handedness == .right ? .white : .black)
                    .border(Color.white)
                HStack {
                    Text("Right Hand")
                        .font(Font.custom("ObjectSans-BoldSlanted", size: 13))
                    Image("arrow-right")
                        .renderingMode(.template)
                        .resizable()
                        .frame(maxWidth: 16, maxHeight: 16)
                }
                .foregroundColor(mofaWatchAppManager.handedness == .right ? .black : .white)
            }
        }
        .buttonStyle(.plain)
    }
    
    var leftHandButton: some View {
        Button {
            mofaWatchAppManager.handedness = .left
        } label: {
            ZStack {
                Rectangle()
                    .frame(maxWidth: 120, maxHeight: 50)
                    .foregroundColor(mofaWatchAppManager.handedness == .right ? .black : .white)
                    .border(Color.white)
                HStack {
                    Text("Left Hand")
                        .font(Font.custom("ObjectSans-BoldSlanted", size: 13))
                    Image("arrow-right")
                        .renderingMode(.template)
                        .resizable()
                        .frame(maxWidth: 16, maxHeight: 16)

                }
                .foregroundColor(mofaWatchAppManager.handedness == .right ? .white : .black)

            }
        }
        .buttonStyle(.plain)
    }
}

struct MofaHandednessView_Previews: PreviewProvider {
    static var previews: some View {
        MofaHandednessView()
    }
}
