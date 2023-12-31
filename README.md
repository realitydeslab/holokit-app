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

This app is organized into distinct packages, where each reality is represented by a differenct package. The core of these packages is `Holoi.Library.HoloKitApp`, which serves as the base package that all other reality packages depend on.

<img width="398" alt="image" src="https://github.com/holoi/holokit-app/assets/44870300/562e895c-31b2-4ada-b53d-968518d7a4e4">

To develop a new reality, you should create a new reality package that follows the established project structure. For step-by-step instructions on creating a new reality, please refer to the [How To Create A New Reality](#how-to-create-a-new-reality) section.

## Native Plugins

### Customized Plugins
There are some native plugins for this app. 

* [HoloKitAppIOSNative] compiles to
`Packages/com.holoi.library.holokit-app/Runtime/Plugins/HoloKitAppIOSNative`

* [HoloKitAppWatchConnectivityNativePlugin] compiles to `Packages/com.holoi.library.holokit-app/Runtime/Plugins/HoloKitAppWatchConnectivity/iOS/libHoloKitAppWatchConnectivityNativePlugin.a`

* [HoloiLibraryPermissionsNativePlugin] compiles to `Packages/com.holoi.library.permissions`

* [MultipeerConnectivityTransportForNetcodeForGameObjectsNativePlugin] compiles to `Packages/com.holoi.netcode.transport.mpc`



[HoloKitAppIOSNative]: https://github.com/holoi/HoloKitAppIOSNative
[HoloKitAppWatchConnectivityNativePlugin]: https://github.com/holoi/HoloKitAppWatchConnectivityNativePlugin
[HoloiLibraryPermissionsNativePlugin]: https://github.com/holoi/HoloiLibraryPermissionsNativePlugin
[MultipeerConnectivityTransportForNetcodeForGameObjectsNativePlugin]: https://github.com/holoi/MultipeerConnectivityTransportForNetcodeForGameObjectsNativePlugin/
### Apple Plugins

We use [Apple Unity Plugins] for CoreHaptics and PHASE. 

[Apple Unity Plugins]:  https://github.com/apple/unityplugins

First we replace the Unity to the latest Unity.

```
cp /Users/amber/Projects/HoloKit/HoloKit2/holokit-app/ProjectSettings/ProjectVersion.txt plug-ins/Apple.GameController/Apple.GameController_Unity/ProjectSettings/ProjectVersion.txt
cp /Users/amber/Projects/HoloKit/HoloKit2/holokit-app/ProjectSettings/ProjectVersion.txt plug-ins/Apple.CoreHaptics/Apple.CoreHaptics_Unity/ProjectSettings/ProjectVersion.txt
cp /Users/amber/Projects/HoloKit/HoloKit2/holokit-app/ProjectSettings/ProjectVersion.txt plug-ins/Apple.Accessibility/Apple.Accessibility_Unity/ProjectSettings/ProjectVersion.txt
cp /Users/amber/Projects/HoloKit/HoloKit2/holokit-app/ProjectSettings/ProjectVersion.txt plug-ins/Apple.GameKit/Apple.GameKit_Unity/ProjectSettings/ProjectVersion.txt
cp /Users/amber/Projects/HoloKit/HoloKit2/holokit-app/ProjectSettings/ProjectVersion.txt plug-ins/Apple.PHASE/Apple.PHASE_Unity/ProjectSettings/ProjectVersion.txt
cp /Users/amber/Projects/HoloKit/HoloKit2/holokit-app/ProjectSettings/ProjectVersion.txt plug-ins/Apple.Core/Apple.Core_Unity/ProjectSettings/ProjectVersion.txt
```

Then we build the plugins for iOS and macOS.

```
python3 build.py -m iOS macOS -b
```


### Troubleshooting

1. You need move Build Phase "Unity Process symbols for Unity-iPhone" to the Last before "Embed Watch Content"
https://forum.unity.com/threads/xcode-15-cycle-inside-unity-iphone-building-could-produce-unreliable-results.1496747/

![Alt text](Documentation/Media/image.png)

2. CopyAndEmbed Error for .framework

![Alt text](Documentation/Media/image-1.png)

Change code according to this PR
https://github.com/apple/unityplugins/pull/6/commits/9514c390ed9e06c27cb58bfa6a995689791f2ca7



3. Missing 
https://github.com/apple/unityplugins/pull/20

Change `Packages/com.apple.unityplugin.core/Editor/AppleFrameworkUtility.cs``
Line 35
```
            string libraryNameWithoutExtension = Path.GetFileNameWithoutExtension(libraryName);
            string[] results = AssetDatabase.FindAssets(libraryNameWithoutExtension);
```

## How To Create A New Reality
