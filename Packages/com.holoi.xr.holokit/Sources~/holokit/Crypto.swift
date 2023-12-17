// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

import Foundation
import CryptoKit

public class Crypto: NSObject {
    
    static let PUBLIC_KEY: String = "aaf31dbb6eacc81799017ba9ec13f24d2c9b0727c6126ba4799157aff1c0ff04"
    
    @objc public static func validateSignature(signature: String, content: String) -> Bool {
        guard let signatureData = Data(base64Encoded: signature) else {
            NSLog("[crypto] failed to encode the signature")
            return false
        }
        guard let contentData = content.data(using: .utf8) else {
            NSLog("[crypto] failed to encode the content")
            return false
        }
        let publicKey = try! Curve25519.Signing.PublicKey(rawRepresentation: dataWithHexString(hex: PUBLIC_KEY))
        if publicKey.isValidSignature(signatureData, for: contentData) {
            //NSLog("[crypto] signature is true")
            return true
        } else {
            //NSLog("[crypto] signature is false")
            return false
        }
    }
    
    static func dataWithHexString(hex: String) -> Data {
        var hex = hex
        var data = Data()
        while(hex.count > 0) {
            let subIndex = hex.index(hex.startIndex, offsetBy: 2)
            let c = String(hex[..<subIndex])
            hex = String(hex[subIndex...])
            var ch: UInt64 = 0
            Scanner(string: c).scanHexInt64(&ch)
            var char = UInt8(ch)
            data.append(&char, count: 1)
        }
        return data
    }
}
