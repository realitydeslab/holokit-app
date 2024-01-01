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

Before creating your new reality, first clone the project to your local disk and open it in Unity Editor. We strongly recommend building it on your iPhone initially to ensure compatibility. Once confirmed, you can proceed to add new realities to the project.

In this project's architecture, each reality is a package dependent on the `Holoi.Library.HoloKitApp` base package. To create a new reality, you need to create a new reality package. We offers two ways to create a new reality package. You can either use the Reality Template Package Generator tool or do it manually.

<img width="169" alt="image" src="https://github.com/holoi/holokit-app/assets/44870300/cbfd048b-ed4d-4cb4-8932-80a4a6cce40d">

For manual creation, we provide a template package to facilitate this. Duplicate the folder `Packages/com.holoi.reality.reality-template`, renaming it to `com.yuchen.reality.my-first-reality`. Of course you can rename this folder to whatever you like as long as it follows the naming rule.

<img width="730" alt="image" src="https://github.com/holoi/holokit-app/assets/44870300/58b9b0e1-0943-4c0e-a637-9018b1bac26e">
<img width="729" alt="image" src="https://github.com/holoi/holokit-app/assets/44870300/85ba078a-18fc-4373-b739-7291d6182c72">

Next, modify the `package.json` file in your new reality package to reflect the necessary changes, as illustrated below.

<img width="464" alt="image" src="https://github.com/holoi/holokit-app/assets/44870300/cdf532dc-14ee-48a9-8383-9836a816a8c1">

Return to Unity Editor, and you will see the new reality package displayed.

<img width="378" alt="image" src="https://github.com/holoi/holokit-app/assets/44870300/9405ebbd-f60c-401d-b3b1-e45bdf781629">

Next, navigate to the reality package folder. In the Runtime folder, locate the assembly definition file and rename it to `Yuchen.Reality.MyFirstReality`.

<img width="411" alt="image" src="https://github.com/holoi/holokit-app/assets/44870300/21ebfa84-2e2a-4cb3-ae4f-82bc12af9bd4">

This should resolve any errors in the project."

In your reaity package, navigate to `Assets/Scenes` and locate the reality template scene. Rename this scene and open it. Inside, you'll find the following game objects as shown in the original scene.

<img width="406" alt="image" src="https://github.com/holoi/holokit-app/assets/44870300/1d34799f-3a15-48b6-91e0-d7999910afda">

Notice the `TemplateRealityManager` prefab in the scene. The `RealityManager` script serves as the core component for a reality. To create your own `RealityManager`, simply write a new script that inherits from the `RealityManager` base script. For this example, I will continue using the `TemplateRealityManager` script.

Please notice that all realities in this project are network-based by default. This requires network programming skills, using Unity's Netcode for GameObjects package. for those unfamiliar with this package, please refer to the [official documentation](https://docs-multiplayer.unity3d.com/netcode/current/about/).

The `RealityManager` class is a `NetworkBehaviour`, and the project handlees the network setup, leaving you to focus on your game logic. Your game logic should be implemented within the Netcode's network context. For instance, when spawning an object, you need to make sure it is spawned across all connected devices.

A key benefit of this network architecture is the support for the spectator view mode in all realities, greatly facilitating the sharing of recordings on social media.

<img width="497" alt="image" src="https://github.com/holoi/holokit-app/assets/44870300/ecb0dbc8-5fca-4e66-8e37-ab978b1bb67f">

Under the `TemplateRealityManager` prefab, you'll find the `RealityConfiguration` script. The project automatically handles setup tasks, including network, UI, and URP setups. However, as a reality developer, you're required to provide necessary assets for these setups:

- `Player Prefab`: Your reality's Netcode player prefab, if you have one.
- `Network Prefabs`: Netcode network prefabs in your reality.
- `UI Panel Prefabs`: Any additional UI panels in your reality, beyond the default UI.
- `UI Reality Setting Tab Prefabs`: Additional components for the reality's settings UI.
- `URP Assets`: Your reality's URP (Universal Render Pipeline) asset. Leave empty to use the default URP asset.
- `Sync Player Pose By Default`: Enable this to synchronize player poses across the network by default.

You can now run and test your reality in Unity Editor. However, it's hard to test all functionalities of AR projecs within Unity Editor. We need to build it onto your iPhone.

Locate the scriptable object of type `Reality` in your reality package's `Assets` folder. This object should contain important information about your reality. Although it has several fields, only fill in the essential ones.

<img width="547" alt="image" src="https://github.com/holoi/holokit-app/assets/44870300/8b0c6f42-d895-48f1-b922-e18a68d2a1c0">

- `Bundle Id`: The unique identifier for your reality, typically the name of your reality package folder.
- `Display Name`: The name of your reality as it appears in the app.
- `Description`: A detailed description of your reality.
- `Author`: The creator's name.
- `Version`: The current version of your reality.
- `Thumbnail Prefab`: Represents your reality visually in the reality list menu.
- `Preview Videos`: Preview videos for your reality in the menu.
- `Tutorial Videos`: Instructional videos for your reality in the menu.
- `Reality Tags`: Tags for your reality in the menu. These can be found in the `Holoi.Library.AssetFoundation package` under `Assets/ScriptableObjects/RealityTags`. This can be left empty. If you want your scene to support spectator view mode, drag the `SpectatorView` tag into this field.
- `Reality Entrance Options`: Entrance buttons for your reality. Typically includes 'Enter Reality' and 'Join As Spectator' buttons. More complex realities may have additional options.
- `Scene`: The main scene of your reality.

Fill in all necessary fields as explained above and leave the rest empty.

In the project's `Assets/Constants` folder, you'll find a scriptable object named `AvailableRealityList`. This object includes the `Reality` scriptable objects for all realities present in the app.

<img width="547" alt="image" src="https://github.com/holoi/holokit-app/assets/44870300/d9f8ab2e-4e86-4918-bd53-3044c766d584">

To add your reality to the app, drag your `Reality` scriptable object into this list. Also, add your reality scene to the build scene list. Then, build and deploy the app to your iPhone.

<img width="692" alt="image" src="https://github.com/holoi/holokit-app/assets/44870300/6d3f3895-b2b1-4f2a-92b9-cebc02dbfe27">

Upon completion, your reality will be accessible within the app.

<img width="283" alt="image" src="https://github.com/holoi/holokit-app/assets/44870300/ef9918c0-0870-4603-bd83-ad9b85e26293">
