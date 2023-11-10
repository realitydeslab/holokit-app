# HoloKit App

*HoloKit App* is the official app on [Apple App Store], which contains multiple demos to showcase HoloKit X. 

[Apple App Store]: https://apps.apple.com/us/app/holokit/id6444073276

## How to compile it?

### LFS
This repository uses Git LFS to manage large files. 
To download the large files, run `git lfs pull` in your terminal. 

### Unity Version
It compiles with Unity 2021.3 LTS.

## Architecture

This app is structured in packages.
Each reality is a package. 

## Native Plugins

There are some native plugins for this app. 

* [HoloKitAppIOSNative] compiles to
`Packages/com.holoi.library.holokit-app/Runtime/Plugins/HoloKitAppIOSNative/iOS/libHoloKitAppIOSNative.a`

* [HoloKitAppWatchConnectivityNativePlugin] compiles to `Packages/com.holoi.library.holokit-app/Runtime/Plugins/HoloKitAppWatchConnectivity/iOS/libHoloKitAppWatchConnectivityNativePlugin.a`

* [HoloiLibraryPermissionsNativePlugin] compiles to `Packages/com.holoi.library.permissions/Runtime/iOS/libHoloiLibraryPermissionsNativePlugin.a`

[HoloKitAppIOSNative]: https://github.com/holoi/HoloKitAppIOSNative
[HoloKitAppWatchConnectivityNativePlugin]: https://github.com/holoi/HoloKitAppWatchConnectivityNativePlugin
[HoloiLibraryPermissionsNativePlugin]: https://github.com/holoi/HoloiLibraryPermissionsNativePlugin

