// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Botao Amber Hu <botao@holoi.com>
// SPDX-License-Identifier: MIT

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Holoi.Library.HoloKitApp.Editor
{
    public class RealityPackageTemplateGenerator: EditorWindow
    {
        string _realityName = "TypedTheDream";
        string _displayName = "Typed: The Dream";
        string _bundleId = "com.holoi.reality.typed.the-dream";
        string _namespaceName = "Holoi.Reality.Typed.TheDream";
        string _author = "Holo Interactive";

        [MenuItem("Tools/Reality Template Package Generator")]
        public static void LaunchRealityPackageGeneraetorWindow()
        {
            GetWindow<RealityPackageTemplateGenerator>();
        }

        private void Clear()
        {
            EditorUtility.UnloadUnusedAssetsImmediate();
        }

        private void OnGUI()
        {
            //GUIUtilities.HorizontalLine();

            EditorGUILayout.BeginVertical();
            
            GUILayout.Width(300.0f);
            
            _realityName = EditorGUILayout.TextField("Reality Name", _realityName);
            _bundleId = EditorGUILayout.TextField("BundleId", _bundleId);
            _namespaceName = EditorGUILayout.TextField("Namespace Name", _namespaceName);
            _author = EditorGUILayout.TextField("Author", _author);
            _displayName = EditorGUILayout.TextField("Display Name", _displayName);  
            
            var prevColor = GUI.color;
            GUI.color = Color.green;
            
            if (GUILayout.Button("Generate Package", GUILayout.Width(300f)))
            {
                GenerateNewRealityPackage();
            }
            
            GUI.color = prevColor;
            
            GUILayout.FlexibleSpace();
            
            EditorGUILayout.EndVertical();
        }
        
        private void OnDestroy()
        {
            Clear();
        }
        private void GenerateNewRealityPackage()
        {
            // Validate input
            if (string.IsNullOrEmpty(_realityName))
            {
                EditorUtility.DisplayDialog("Error", "Please enter a package name.", "OK");
                return;
            }
            if (string.IsNullOrEmpty(_bundleId))
            {
                EditorUtility.DisplayDialog("Error", "Please enter a bundle identifier.", "OK");
                return;
            }
            if (string.IsNullOrEmpty(_namespaceName))
            {
                EditorUtility.DisplayDialog("Error", "Please enter a namespace.", "OK");
                return;
            }
            if (string.IsNullOrEmpty(_author))
            {
                EditorUtility.DisplayDialog("Error", "Please enter an author.", "OK");
                return;
            }

            if (string.IsNullOrEmpty(_displayName))
            {
                EditorUtility.DisplayDialog("Error", "Please enter a display name.", "OK");
                return;
            }

            // Create package folder
            string packagePath = Path.Combine("Packages", _bundleId);
            Directory.CreateDirectory(packagePath);
            string runtimePath = Path.Combine(packagePath, "Runtime");
            Directory.CreateDirectory(runtimePath);
            string assetsPath = Path.Combine(packagePath, "Assets");
            Directory.CreateDirectory(assetsPath);
            string artResourcesPath = Path.Combine(assetsPath, "ArtResources");
            Directory.CreateDirectory(artResourcesPath);
            string prefabsPath = Path.Combine(assetsPath, "Prefabs");
            Directory.CreateDirectory(prefabsPath);
            string scenesPath = Path.Combine(assetsPath, "Scenes");
            Directory.CreateDirectory(scenesPath);
            string videoPath = Path.Combine(assetsPath, "Videos");
            Directory.CreateDirectory(videoPath);
            string thumbnailsPath = Path.Combine(assetsPath, "Thumbnails");
            Directory.CreateDirectory(thumbnailsPath);
            string docPath = Path.Combine(packagePath, "Documentation~");
            Directory.CreateDirectory(docPath);
            
					  // Generate RealityManager script
            string realityManagerContent = @"// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Botao Amber Hu <botao@holoi.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Unity.Netcode;
using Holoi.Library.HoloKitApp;
using HoloKit;

namespace {{_namespaceName}}
{
    public class {{_realityName}}RealityManager : RealityManager 
    {
          
    }
}
"
              .Replace("{{_namespaceName}}", _namespaceName)
              .Replace("{{_realityName}}", _realityName);
            File.WriteAllText(Path.Combine(runtimePath, $"{_realityName}RealityManager.cs"), realityManagerContent);
            string realityManagerGUID = System.Guid.NewGuid().ToString("N");
            string realityManagerMeta = @"fileFormatVersion: 2
guid: {{realityManagerGUID}}
MonoImporter:
  externalObjects: {}
  serializedVersion: 2
  defaultReferences: []
  executionOrder: 0
  icon: {instanceID: 0}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"
              .Replace("{{realityManagerGUID}}", realityManagerGUID);
            File.WriteAllText(Path.Combine(runtimePath, $"{_realityName}RealityManager.cs.meta"), realityManagerMeta);

            // Generate scene asset
            string sceneAssetContent = @"%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!29 &1
OcclusionCullingSettings:
  m_ObjectHideFlags: 0
  serializedVersion: 2
  m_OcclusionBakeSettings:
    smallestOccluder: 5
    smallestHole: 0.25
    backfaceThreshold: 100
  m_SceneGUID: 00000000000000000000000000000000
  m_OcclusionCullingData: {fileID: 0}
--- !u!104 &2
RenderSettings:
  m_ObjectHideFlags: 0
  serializedVersion: 10
  m_Fog: 0
  m_FogColor: {r: 0.5, g: 0.5, b: 0.5, a: 1}
  m_FogMode: 3
  m_FogDensity: 0.01
  m_LinearFogStart: 0
  m_LinearFogEnd: 300
  m_AmbientSkyColor: {r: 0.212, g: 0.227, b: 0.259, a: 1}
  m_AmbientEquatorColor: {r: 0.114, g: 0.125, b: 0.133, a: 1}
  m_AmbientGroundColor: {r: 0.047, g: 0.043, b: 0.035, a: 1}
  m_AmbientIntensity: 2
  m_AmbientMode: 0
  m_SubtractiveShadowColor: {r: 0.42, g: 0.478, b: 0.627, a: 1}
  m_SkyboxMaterial: {fileID: 10304, guid: 0000000000000000f000000000000000, type: 0}
  m_HaloStrength: 0.5
  m_FlareStrength: 1
  m_FlareFadeSpeed: 3
  m_HaloTexture: {fileID: 0}
  m_SpotCookie: {fileID: 10001, guid: 0000000000000000e000000000000000, type: 0}
  m_DefaultReflectionMode: 0
  m_DefaultReflectionResolution: 128
  m_ReflectionBounces: 1
  m_ReflectionIntensity: 1
  m_CustomReflection: {fileID: 0}
  m_Sun: {fileID: 0}
  m_IndirectSpecularColor: {r: 0, g: 0, b: 0, a: 1}
  m_UseRadianceAmbientProbe: 0
--- !u!157 &3
LightmapSettings:
  m_ObjectHideFlags: 0
  serializedVersion: 12
  m_GISettings:
    serializedVersion: 2
    m_BounceScale: 1
    m_IndirectOutputScale: 1
    m_AlbedoBoost: 1
    m_EnvironmentLightingMode: 0
    m_EnableBakedLightmaps: 1
    m_EnableRealtimeLightmaps: 0
  m_LightmapEditorSettings:
    serializedVersion: 12
    m_Resolution: 2
    m_BakeResolution: 40
    m_AtlasSize: 1024
    m_AO: 0
    m_AOMaxDistance: 1
    m_CompAOExponent: 1
    m_CompAOExponentDirect: 0
    m_ExtractAmbientOcclusion: 0
    m_Padding: 2
    m_LightmapParameters: {fileID: 0}
    m_LightmapsBakeMode: 1
    m_TextureCompression: 1
    m_ReflectionCompression: 2
    m_MixedBakeMode: 2
    m_BakeBackend: 1
    m_PVRSampling: 1
    m_PVRDirectSampleCount: 32
    m_PVRSampleCount: 512
    m_PVRBounces: 2
    m_PVREnvironmentSampleCount: 256
    m_PVREnvironmentReferencePointCount: 2048
    m_PVRFilteringMode: 1
    m_PVRDenoiserTypeDirect: 1
    m_PVRDenoiserTypeIndirect: 1
    m_PVRDenoiserTypeAO: 1
    m_PVRFilterTypeDirect: 0
    m_PVRFilterTypeIndirect: 0
    m_PVRFilterTypeAO: 0
    m_PVREnvironmentMIS: 1
    m_PVRCulling: 1
    m_PVRFilteringGaussRadiusDirect: 1
    m_PVRFilteringGaussRadiusIndirect: 5
    m_PVRFilteringGaussRadiusAO: 2
    m_PVRFilteringAtrousPositionSigmaDirect: 0.5
    m_PVRFilteringAtrousPositionSigmaIndirect: 2
    m_PVRFilteringAtrousPositionSigmaAO: 1
    m_ExportTrainingData: 0
    m_TrainingDataDestination: TrainingData
    m_LightProbeSampleCountMultiplier: 4
  m_LightingDataAsset: {fileID: 0}
  m_LightingSettings: {fileID: 0}
--- !u!196 &4
NavMeshSettings:
  serializedVersion: 2
  m_ObjectHideFlags: 0
  m_BuildSettings:
    serializedVersion: 3
    agentTypeID: 0
    agentRadius: 0.5
    agentHeight: 2
    agentSlope: 45
    agentClimb: 0.4
    ledgeDropHeight: 0
    maxJumpAcrossDistance: 0
    minRegionArea: 2
    manualCellSize: 0
    cellSize: 0.16666667
    manualTileSize: 0
    tileSize: 256
    buildHeightMesh: 0
    maxJobWorkers: 0
    preserveTilesOutsideBounds: 0
    debug:
      m_Flags: 0
  m_NavMeshData: {fileID: 0}
--- !u!1001 &275482710
PrefabInstance:
  m_ObjectHideFlags: 0
  serializedVersion: 2
  m_Modification:
    serializedVersion: 3
    m_TransformParent: {fileID: 933475194}
    m_Modifications:
    - target: {fileID: 5626992625800642032, guid: 8c3baeda8ec2e42179a64fba10a812b1, type: 3}
      propertyPath: m_LocalPosition.x
      value: 0
      objectReference: {fileID: 0}
    - target: {fileID: 5626992625800642032, guid: 8c3baeda8ec2e42179a64fba10a812b1, type: 3}
      propertyPath: m_LocalPosition.y
      value: 0
      objectReference: {fileID: 0}
    - target: {fileID: 5626992625800642032, guid: 8c3baeda8ec2e42179a64fba10a812b1, type: 3}
      propertyPath: m_LocalPosition.z
      value: 0
      objectReference: {fileID: 0}
    - target: {fileID: 5626992625800642032, guid: 8c3baeda8ec2e42179a64fba10a812b1, type: 3}
      propertyPath: m_LocalRotation.w
      value: 1
      objectReference: {fileID: 0}
    - target: {fileID: 5626992625800642032, guid: 8c3baeda8ec2e42179a64fba10a812b1, type: 3}
      propertyPath: m_LocalRotation.x
      value: 0
      objectReference: {fileID: 0}
    - target: {fileID: 5626992625800642032, guid: 8c3baeda8ec2e42179a64fba10a812b1, type: 3}
      propertyPath: m_LocalRotation.y
      value: 0
      objectReference: {fileID: 0}
    - target: {fileID: 5626992625800642032, guid: 8c3baeda8ec2e42179a64fba10a812b1, type: 3}
      propertyPath: m_LocalRotation.z
      value: 0
      objectReference: {fileID: 0}
    - target: {fileID: 5626992625800642032, guid: 8c3baeda8ec2e42179a64fba10a812b1, type: 3}
      propertyPath: m_LocalEulerAnglesHint.x
      value: 0
      objectReference: {fileID: 0}
    - target: {fileID: 5626992625800642032, guid: 8c3baeda8ec2e42179a64fba10a812b1, type: 3}
      propertyPath: m_LocalEulerAnglesHint.y
      value: 0
      objectReference: {fileID: 0}
    - target: {fileID: 5626992625800642032, guid: 8c3baeda8ec2e42179a64fba10a812b1, type: 3}
      propertyPath: m_LocalEulerAnglesHint.z
      value: 0
      objectReference: {fileID: 0}
    - target: {fileID: 5626992625800642033, guid: 8c3baeda8ec2e42179a64fba10a812b1, type: 3}
      propertyPath: m_Name
      value: HoloKit Camera
      objectReference: {fileID: 0}
    m_RemovedComponents: []
    m_RemovedGameObjects: []
    m_AddedGameObjects: []
    m_AddedComponents: []
  m_SourcePrefab: {fileID: 100100000, guid: 8c3baeda8ec2e42179a64fba10a812b1, type: 3}
--- !u!4 &275482711 stripped
Transform:
  m_CorrespondingSourceObject: {fileID: 5626992625800642032, guid: 8c3baeda8ec2e42179a64fba10a812b1, type: 3}
  m_PrefabInstance: {fileID: 275482710}
  m_PrefabAsset: {fileID: 0}
--- !u!20 &275482712 stripped
Camera:
  m_CorrespondingSourceObject: {fileID: 5626992625800642046, guid: 8c3baeda8ec2e42179a64fba10a812b1, type: 3}
  m_PrefabInstance: {fileID: 275482710}
  m_PrefabAsset: {fileID: 0}
--- !u!1 &745928622
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  serializedVersion: 6
  m_Component:
  - component: {fileID: 745928623}
  - component: {fileID: 745928624}
  m_Layer: 0
  m_Name: Meshing
  m_TagString: Untagged
  m_Icon: {fileID: 0}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!4 &745928623
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 745928622}
  serializedVersion: 2
  m_LocalRotation: {x: 0, y: 0, z: 0, w: 1}
  m_LocalPosition: {x: 0, y: 0, z: 0}
  m_LocalScale: {x: 10, y: 10, z: 10}
  m_ConstrainProportionsScale: 0
  m_Children: []
  m_Father: {fileID: 933475194}
  m_LocalEulerAnglesHint: {x: 0, y: 0, z: 0}
--- !u!114 &745928624
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 745928622}
  m_Enabled: 0
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: 968053edfd89749c48f4ea5d444abf64, type: 3}
  m_Name: 
  m_EditorClassIdentifier: 
  m_MeshPrefab: {fileID: 2190579803620544122, guid: 456e8a3460ea5488fbfe94c6b91b0f61, type: 3}
  m_Density: 0.5
  m_Normals: 1
  m_Tangents: 0
  m_TextureCoordinates: 1
  m_Colors: 0
  m_ConcurrentQueueSize: 4
--- !u!1001 &782621718
PrefabInstance:
  m_ObjectHideFlags: 0
  serializedVersion: 2
  m_Modification:
    serializedVersion: 3
    m_TransformParent: {fileID: 933475194}
    m_Modifications:
    - target: {fileID: 2148306426442201805, guid: 33703e991f67b48bc95296635ea2bc41, type: 3}
      propertyPath: m_IsActive
      value: 1
      objectReference: {fileID: 0}
    - target: {fileID: 3231388504386293999, guid: 33703e991f67b48bc95296635ea2bc41, type: 3}
      propertyPath: m_IsActive
      value: 1
      objectReference: {fileID: 0}
    - target: {fileID: 4879128648065969852, guid: 33703e991f67b48bc95296635ea2bc41, type: 3}
      propertyPath: m_Name
      value: HoloKit Hand Tracker
      objectReference: {fileID: 0}
    - target: {fileID: 4879128648065969853, guid: 33703e991f67b48bc95296635ea2bc41, type: 3}
      propertyPath: m_LocalPosition.x
      value: 0
      objectReference: {fileID: 0}
    - target: {fileID: 4879128648065969853, guid: 33703e991f67b48bc95296635ea2bc41, type: 3}
      propertyPath: m_LocalPosition.y
      value: 0
      objectReference: {fileID: 0}
    - target: {fileID: 4879128648065969853, guid: 33703e991f67b48bc95296635ea2bc41, type: 3}
      propertyPath: m_LocalPosition.z
      value: 0
      objectReference: {fileID: 0}
    - target: {fileID: 4879128648065969853, guid: 33703e991f67b48bc95296635ea2bc41, type: 3}
      propertyPath: m_LocalRotation.w
      value: 1
      objectReference: {fileID: 0}
    - target: {fileID: 4879128648065969853, guid: 33703e991f67b48bc95296635ea2bc41, type: 3}
      propertyPath: m_LocalRotation.x
      value: -0
      objectReference: {fileID: 0}
    - target: {fileID: 4879128648065969853, guid: 33703e991f67b48bc95296635ea2bc41, type: 3}
      propertyPath: m_LocalRotation.y
      value: -0
      objectReference: {fileID: 0}
    - target: {fileID: 4879128648065969853, guid: 33703e991f67b48bc95296635ea2bc41, type: 3}
      propertyPath: m_LocalRotation.z
      value: -0
      objectReference: {fileID: 0}
    - target: {fileID: 4879128648065969853, guid: 33703e991f67b48bc95296635ea2bc41, type: 3}
      propertyPath: m_LocalEulerAnglesHint.x
      value: 0
      objectReference: {fileID: 0}
    - target: {fileID: 4879128648065969853, guid: 33703e991f67b48bc95296635ea2bc41, type: 3}
      propertyPath: m_LocalEulerAnglesHint.y
      value: 0
      objectReference: {fileID: 0}
    - target: {fileID: 4879128648065969853, guid: 33703e991f67b48bc95296635ea2bc41, type: 3}
      propertyPath: m_LocalEulerAnglesHint.z
      value: 0
      objectReference: {fileID: 0}
    m_RemovedComponents: []
    m_RemovedGameObjects: []
    m_AddedGameObjects: []
    m_AddedComponents: []
  m_SourcePrefab: {fileID: 100100000, guid: 33703e991f67b48bc95296635ea2bc41, type: 3}
--- !u!4 &782621719 stripped
Transform:
  m_CorrespondingSourceObject: {fileID: 4879128648065969853, guid: 33703e991f67b48bc95296635ea2bc41, type: 3}
  m_PrefabInstance: {fileID: 782621718}
  m_PrefabAsset: {fileID: 0}
--- !u!1 &933475193
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  serializedVersion: 6
  m_Component:
  - component: {fileID: 933475194}
  m_Layer: 0
  m_Name: Camera Offset
  m_TagString: Untagged
  m_Icon: {fileID: 0}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!4 &933475194
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 933475193}
  serializedVersion: 2
  m_LocalRotation: {x: 0, y: 0, z: 0, w: 1}
  m_LocalPosition: {x: 0, y: 0, z: 0}
  m_LocalScale: {x: 1, y: 1, z: 1}
  m_ConstrainProportionsScale: 0
  m_Children:
  - {fileID: 275482711}
  - {fileID: 782621719}
  - {fileID: 745928623}
  m_Father: {fileID: 2068233601}
  m_LocalEulerAnglesHint: {x: 0, y: 0, z: 0}
--- !u!1 &1513027496
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  serializedVersion: 6
  m_Component:
  - component: {fileID: 1513027498}
  - component: {fileID: 1513027497}
  - component: {fileID: 1513027499}
  m_Layer: 0
  m_Name: Directional Light
  m_TagString: Untagged
  m_Icon: {fileID: 0}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!108 &1513027497
Light:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 1513027496}
  m_Enabled: 1
  serializedVersion: 11
  m_Type: 1
  m_Color: {r: 1, g: 0.95686275, b: 0.8392157, a: 1}
  m_Intensity: 2
  m_Range: 10
  m_SpotAngle: 30
  m_InnerSpotAngle: 21.80208
  m_CookieSize: 10
  m_Shadows:
    m_Type: 2
    m_Resolution: -1
    m_CustomResolution: -1
    m_Strength: 1
    m_Bias: 0.05
    m_NormalBias: 0.4
    m_NearPlane: 0.2
    m_CullingMatrixOverride:
      e00: 1
      e01: 0
      e02: 0
      e03: 0
      e10: 0
      e11: 1
      e12: 0
      e13: 0
      e20: 0
      e21: 0
      e22: 1
      e23: 0
      e30: 0
      e31: 0
      e32: 0
      e33: 1
    m_UseCullingMatrixOverride: 0
  m_Cookie: {fileID: 0}
  m_DrawHalo: 0
  m_Flare: {fileID: 0}
  m_RenderMode: 0
  m_CullingMask:
    serializedVersion: 2
    m_Bits: 4294967295
  m_RenderingLayerMask: 1
  m_Lightmapping: 4
  m_LightShadowCasterMode: 0
  m_AreaSize: {x: 1, y: 1}
  m_BounceIntensity: 1
  m_ColorTemperature: 6570
  m_UseColorTemperature: 0
  m_BoundingSphereOverride: {x: 0, y: 0, z: 0, w: 0}
  m_UseBoundingSphereOverride: 0
  m_UseViewFrustumForShadowCasterCull: 1
  m_ShadowRadius: 0
  m_ShadowAngle: 0
--- !u!4 &1513027498
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 1513027496}
  serializedVersion: 2
  m_LocalRotation: {x: 0.40821788, y: -0.23456968, z: 0.10938163, w: 0.8754261}
  m_LocalPosition: {x: 0, y: 0, z: 0}
  m_LocalScale: {x: 1, y: 1, z: 1}
  m_ConstrainProportionsScale: 0
  m_Children: []
  m_Father: {fileID: 0}
  m_LocalEulerAnglesHint: {x: 50, y: -30, z: 0}
--- !u!114 &1513027499
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 1513027496}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: 474bcb49853aa07438625e644c072ee6, type: 3}
  m_Name: 
  m_EditorClassIdentifier: 
  m_Version: 3
  m_UsePipelineSettings: 1
  m_AdditionalLightsShadowResolutionTier: 2
  m_LightLayerMask: 1
  m_RenderingLayers: 1
  m_CustomShadowLayers: 0
  m_ShadowLayerMask: 1
  m_ShadowRenderingLayers: 1
  m_LightCookieSize: {x: 1, y: 1}
  m_LightCookieOffset: {x: 0, y: 0}
  m_SoftShadowQuality: 1
--- !u!1001 &1642450503
PrefabInstance:
  m_ObjectHideFlags: 0
  serializedVersion: 2
  m_Modification:
    serializedVersion: 3
    m_TransformParent: {fileID: 0}
    m_Modifications:
    - target: {fileID: 2088026539129220745, guid: 3fd8ed4d40dfd45f59a9d40cc6463940, type: 3}
      propertyPath: m_LocalPosition.x
      value: 0
      objectReference: {fileID: 0}
    - target: {fileID: 2088026539129220745, guid: 3fd8ed4d40dfd45f59a9d40cc6463940, type: 3}
      propertyPath: m_LocalPosition.y
      value: 0
      objectReference: {fileID: 0}
    - target: {fileID: 2088026539129220745, guid: 3fd8ed4d40dfd45f59a9d40cc6463940, type: 3}
      propertyPath: m_LocalPosition.z
      value: 0
      objectReference: {fileID: 0}
    - target: {fileID: 2088026539129220745, guid: 3fd8ed4d40dfd45f59a9d40cc6463940, type: 3}
      propertyPath: m_LocalRotation.w
      value: 1
      objectReference: {fileID: 0}
    - target: {fileID: 2088026539129220745, guid: 3fd8ed4d40dfd45f59a9d40cc6463940, type: 3}
      propertyPath: m_LocalRotation.x
      value: 0
      objectReference: {fileID: 0}
    - target: {fileID: 2088026539129220745, guid: 3fd8ed4d40dfd45f59a9d40cc6463940, type: 3}
      propertyPath: m_LocalRotation.y
      value: 0
      objectReference: {fileID: 0}
    - target: {fileID: 2088026539129220745, guid: 3fd8ed4d40dfd45f59a9d40cc6463940, type: 3}
      propertyPath: m_LocalRotation.z
      value: 0
      objectReference: {fileID: 0}
    - target: {fileID: 2088026539129220745, guid: 3fd8ed4d40dfd45f59a9d40cc6463940, type: 3}
      propertyPath: m_LocalEulerAnglesHint.x
      value: 0
      objectReference: {fileID: 0}
    - target: {fileID: 2088026539129220745, guid: 3fd8ed4d40dfd45f59a9d40cc6463940, type: 3}
      propertyPath: m_LocalEulerAnglesHint.y
      value: 0
      objectReference: {fileID: 0}
    - target: {fileID: 2088026539129220745, guid: 3fd8ed4d40dfd45f59a9d40cc6463940, type: 3}
      propertyPath: m_LocalEulerAnglesHint.z
      value: 0
      objectReference: {fileID: 0}
    - target: {fileID: 5943210170912577281, guid: 3fd8ed4d40dfd45f59a9d40cc6463940, type: 3}
      propertyPath: m_Name
      value: HoloKit App
      objectReference: {fileID: 0}
    m_RemovedComponents: []
    m_RemovedGameObjects: []
    m_AddedGameObjects: []
    m_AddedComponents: []
  m_SourcePrefab: {fileID: 100100000, guid: 3fd8ed4d40dfd45f59a9d40cc6463940, type: 3}
--- !u!1 &1868868440
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  serializedVersion: 6
  m_Component:
  - component: {fileID: 1868868443}
  - component: {fileID: 1868868442}
  - component: {fileID: 1868868441}
  m_Layer: 0
  m_Name: AR Session
  m_TagString: Untagged
  m_Icon: {fileID: 0}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!114 &1868868441
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 1868868440}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: fa850fbd5b8aded44846f96e35f1a9f5, type: 3}
  m_Name: 
  m_EditorClassIdentifier: 
--- !u!114 &1868868442
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 1868868440}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: 3859a92a05d4f5d418cb6ca605290e74, type: 3}
  m_Name: 
  m_EditorClassIdentifier: 
  m_AttemptUpdate: 1
  m_MatchFrameRate: 1
  m_TrackingMode: 2
--- !u!4 &1868868443
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 1868868440}
  serializedVersion: 2
  m_LocalRotation: {x: -0, y: -0, z: -0, w: 1}
  m_LocalPosition: {x: 0, y: 0, z: 0}
  m_LocalScale: {x: 1, y: 1, z: 1}
  m_ConstrainProportionsScale: 0
  m_Children: []
  m_Father: {fileID: 0}
  m_LocalEulerAnglesHint: {x: 0, y: 0, z: 0}
--- !u!1 &2068233599
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  serializedVersion: 6
  m_Component:
  - component: {fileID: 2068233601}
  - component: {fileID: 2068233600}
  m_Layer: 0
  m_Name: XR Origin
  m_TagString: Untagged
  m_Icon: {fileID: 0}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!114 &2068233600
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 2068233599}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: e0cb9aa70a22847b5925ee5f067c10a9, type: 3}
  m_Name: 
  m_EditorClassIdentifier: 
  m_Camera: {fileID: 275482712}
  m_OriginBaseGameObject: {fileID: 2068233599}
  m_CameraFloorOffsetObject: {fileID: 933475193}
  m_RequestedTrackingOriginMode: 0
  m_CameraYOffset: 1.1176
--- !u!4 &2068233601
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 2068233599}
  serializedVersion: 2
  m_LocalRotation: {x: 0, y: 0, z: 0, w: 1}
  m_LocalPosition: {x: 0, y: 0, z: 0}
  m_LocalScale: {x: 1, y: 1, z: 1}
  m_ConstrainProportionsScale: 0
  m_Children:
  - {fileID: 933475194}
  m_Father: {fileID: 0}
  m_LocalEulerAnglesHint: {x: 0, y: 0, z: 0}
--- !u!1 &1072407931541854019
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  serializedVersion: 6
  m_Component:
  - component: {fileID: 1072407931541854021}
  - component: {fileID: 1072407931541854020}
  - component: {fileID: 8923555380272086202}
  - component: {fileID: 8923555380272086203}
  m_Layer: 0
  m_Name: {{_realityName}}RealityManager
  m_TagString: Untagged
  m_Icon: {fileID: 0}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!114 &1072407931541854020
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 1072407931541854019}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: d5a57f767e5e46a458fc5d3c628d0cbb, type: 3}
  m_Name: 
  m_EditorClassIdentifier: 
  GlobalObjectIdHash: 2597862321
  AlwaysReplicateAsRoot: 0
  SynchronizeTransform: 1
  ActiveSceneSynchronization: 0
  SceneMigrationSynchronization: 1
  SpawnWithObservers: 1
  DontDestroyWithOwner: 0
  AutoObjectParentSync: 1
--- !u!4 &1072407931541854021
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 1072407931541854019}
  serializedVersion: 2
  m_LocalRotation: {x: 0, y: 0, z: 0, w: 1}
  m_LocalPosition: {x: 0, y: 0, z: 0}
  m_LocalScale: {x: 1, y: 1, z: 1}
  m_ConstrainProportionsScale: 0
  m_Children: []
  m_Father: {fileID: 0}
  m_LocalEulerAnglesHint: {x: 0, y: 0, z: 0}
--- !u!1001 &3377202017777336383
PrefabInstance:
  m_ObjectHideFlags: 0
  serializedVersion: 2
  m_Modification:
    serializedVersion: 3
    m_TransformParent: {fileID: 0}
    m_Modifications:
    - target: {fileID: 1220708651965413441, guid: 070d93705e0c248449188c9d85d9d813, type: 3}
      propertyPath: m_Name
      value: Cube
      objectReference: {fileID: 0}
    - target: {fileID: 3737036489601911242, guid: 070d93705e0c248449188c9d85d9d813, type: 3}
      propertyPath: GlobalObjectIdHash
      value: 3001262315
      objectReference: {fileID: 0}
    - target: {fileID: 8754851722878583913, guid: 070d93705e0c248449188c9d85d9d813, type: 3}
      propertyPath: m_LocalPosition.x
      value: 0
      objectReference: {fileID: 0}
    - target: {fileID: 8754851722878583913, guid: 070d93705e0c248449188c9d85d9d813, type: 3}
      propertyPath: m_LocalPosition.y
      value: 0
      objectReference: {fileID: 0}
    - target: {fileID: 8754851722878583913, guid: 070d93705e0c248449188c9d85d9d813, type: 3}
      propertyPath: m_LocalPosition.z
      value: 0.5
      objectReference: {fileID: 0}
    - target: {fileID: 8754851722878583913, guid: 070d93705e0c248449188c9d85d9d813, type: 3}
      propertyPath: m_LocalRotation.w
      value: 1
      objectReference: {fileID: 0}
    - target: {fileID: 8754851722878583913, guid: 070d93705e0c248449188c9d85d9d813, type: 3}
      propertyPath: m_LocalRotation.x
      value: 0
      objectReference: {fileID: 0}
    - target: {fileID: 8754851722878583913, guid: 070d93705e0c248449188c9d85d9d813, type: 3}
      propertyPath: m_LocalRotation.y
      value: 0
      objectReference: {fileID: 0}
    - target: {fileID: 8754851722878583913, guid: 070d93705e0c248449188c9d85d9d813, type: 3}
      propertyPath: m_LocalRotation.z
      value: 0
      objectReference: {fileID: 0}
    - target: {fileID: 8754851722878583913, guid: 070d93705e0c248449188c9d85d9d813, type: 3}
      propertyPath: m_LocalEulerAnglesHint.x
      value: 0
      objectReference: {fileID: 0}
    - target: {fileID: 8754851722878583913, guid: 070d93705e0c248449188c9d85d9d813, type: 3}
      propertyPath: m_LocalEulerAnglesHint.y
      value: 0
      objectReference: {fileID: 0}
    - target: {fileID: 8754851722878583913, guid: 070d93705e0c248449188c9d85d9d813, type: 3}
      propertyPath: m_LocalEulerAnglesHint.z
      value: 0
      objectReference: {fileID: 0}
    m_RemovedComponents: []
    m_RemovedGameObjects: []
    m_AddedGameObjects: []
    m_AddedComponents: []
  m_SourcePrefab: {fileID: 100100000, guid: 070d93705e0c248449188c9d85d9d813, type: 3}
--- !u!114 &8923555380272086202
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 1072407931541854019}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: {{realityManagerGUID}}, type: 3}
  m_Name: 
  m_EditorClassIdentifier: 
--- !u!114 &8923555380272086203
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 1072407931541854019}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: b4ed510440403498f8353bc5bfc1ddd8, type: 3}
  m_Name: 
  m_EditorClassIdentifier: 
  PlayerPrefab: {fileID: 0}
  NetworkPrefabs: []
  UIPanelPrefabs: []
  UIRealitySettingTabPrefabs: []
  UrpAsset: {fileID: 0}
  SyncPlayerPoseByDefault: 0
--- !u!1660057539 &9223372036854775807
SceneRoots:
  m_ObjectHideFlags: 0
  m_Roots:
  - {fileID: 1642450503}
  - {fileID: 1868868443}
  - {fileID: 2068233601}
  - {fileID: 1513027498}
  - {fileID: 1072407931541854021}
  - {fileID: 3377202017777336383}
"
              .Replace("{{_realityName}}", _realityName)
              .Replace("{{realityManagerGUID}}", realityManagerGUID);
            File.WriteAllText(Path.Combine(scenesPath, $"{_realityName}.unity"), sceneAssetContent);
            string sceneAssetGUID = System.Guid.NewGuid().ToString("N");
            string sceneAssetMeta = @"fileFormatVersion: 2
guid: {{sceneAssetGUID}}
NativeFormatImporter:
  externalObjects: {}
  mainObjectFileID: 11400000
  userData: 
  assetBundleName: 
  assetBundleVariant: 
              ".Replace("{{sceneAssetGUID}}", sceneAssetGUID);
            File.WriteAllText(Path.Combine(scenesPath, $"{_realityName}.unity.meta"), sceneAssetMeta);
            
            // Generate Thumbnail prefab
            string thumbnailPrefabContent = @"%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!1 &4700645735896304704
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  serializedVersion: 6
  m_Component:
  - component: {fileID: 1768538410551860151}
  - component: {fileID: 6060410201828206963}
  - component: {fileID: 6359398273792790353}
  m_Layer: 0
  m_Name: {{_realityName}}Thumbnail
  m_TagString: Reality Thumbnail
  m_Icon: {fileID: 0}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!4 &1768538410551860151
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 4700645735896304704}
  serializedVersion: 2
  m_LocalRotation: {x: 0, y: 0, z: 0, w: 1}
  m_LocalPosition: {x: 0, y: 0, z: 0}
  m_LocalScale: {x: 1, y: 1, z: 1}
  m_ConstrainProportionsScale: 1
  m_Children:
  - {fileID: 4327932830547831877}
  m_Father: {fileID: 0}
  m_LocalEulerAnglesHint: {x: 0, y: 0, z: 0}
--- !u!33 &6060410201828206963
MeshFilter:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 4700645735896304704}
  m_Mesh: {fileID: -6564192206587231672, guid: 6973c53c3623b44b7bcb81d4944f6e61, type: 3}
--- !u!23 &6359398273792790353
MeshRenderer:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 4700645735896304704}
  m_Enabled: 1
  m_CastShadows: 0
  m_ReceiveShadows: 1
  m_DynamicOccludee: 1
  m_StaticShadowCaster: 0
  m_MotionVectors: 1
  m_LightProbeUsage: 1
  m_ReflectionProbeUsage: 1
  m_RayTracingMode: 2
  m_RayTraceProcedural: 0
  m_RayTracingAccelStructBuildFlagsOverride: 0
  m_RayTracingAccelStructBuildFlags: 1
  m_RenderingLayerMask: 1
  m_RendererPriority: 0
  m_Materials:
  - {fileID: 2100000, guid: b6b80fffc8463452cb036016b523f4e2, type: 2}
  m_StaticBatchInfo:
    firstSubMesh: 0
    subMeshCount: 0
  m_StaticBatchRoot: {fileID: 0}
  m_ProbeAnchor: {fileID: 0}
  m_LightProbeVolumeOverride: {fileID: 0}
  m_ScaleInLightmap: 1
  m_ReceiveGI: 1
  m_PreserveUVs: 0
  m_IgnoreNormalsForChartDetection: 0
  m_ImportantGI: 0
  m_StitchLightmapSeams: 1
  m_SelectedEditorRenderState: 3
  m_MinimumChartSize: 4
  m_AutoUVMaxDistance: 0.5
  m_AutoUVMaxAngle: 89
  m_LightmapParameters: {fileID: 0}
  m_SortingLayerID: 0
  m_SortingLayer: 0
  m_SortingOrder: 0
  m_AdditionalVertexStreams: {fileID: 0}
--- !u!1001 &8254395498397544366
PrefabInstance:
  m_ObjectHideFlags: 0
  serializedVersion: 2
  m_Modification:
    serializedVersion: 3
    m_TransformParent: {fileID: 1768538410551860151}
    m_Modifications:
    - target: {fileID: 5657197342317364202, guid: a2371720312654110b434e74f4f9435a, type: 3}
      propertyPath: m_Name
      value: Room Collider
      objectReference: {fileID: 0}
    - target: {fileID: 5657197342317364203, guid: a2371720312654110b434e74f4f9435a, type: 3}
      propertyPath: m_LocalPosition.x
      value: 0
      objectReference: {fileID: 0}
    - target: {fileID: 5657197342317364203, guid: a2371720312654110b434e74f4f9435a, type: 3}
      propertyPath: m_LocalPosition.y
      value: 0
      objectReference: {fileID: 0}
    - target: {fileID: 5657197342317364203, guid: a2371720312654110b434e74f4f9435a, type: 3}
      propertyPath: m_LocalPosition.z
      value: 0
      objectReference: {fileID: 0}
    - target: {fileID: 5657197342317364203, guid: a2371720312654110b434e74f4f9435a, type: 3}
      propertyPath: m_LocalRotation.w
      value: 1
      objectReference: {fileID: 0}
    - target: {fileID: 5657197342317364203, guid: a2371720312654110b434e74f4f9435a, type: 3}
      propertyPath: m_LocalRotation.x
      value: -0
      objectReference: {fileID: 0}
    - target: {fileID: 5657197342317364203, guid: a2371720312654110b434e74f4f9435a, type: 3}
      propertyPath: m_LocalRotation.y
      value: -0
      objectReference: {fileID: 0}
    - target: {fileID: 5657197342317364203, guid: a2371720312654110b434e74f4f9435a, type: 3}
      propertyPath: m_LocalRotation.z
      value: -0
      objectReference: {fileID: 0}
    - target: {fileID: 5657197342317364203, guid: a2371720312654110b434e74f4f9435a, type: 3}
      propertyPath: m_LocalEulerAnglesHint.x
      value: 0
      objectReference: {fileID: 0}
    - target: {fileID: 5657197342317364203, guid: a2371720312654110b434e74f4f9435a, type: 3}
      propertyPath: m_LocalEulerAnglesHint.y
      value: 0
      objectReference: {fileID: 0}
    - target: {fileID: 5657197342317364203, guid: a2371720312654110b434e74f4f9435a, type: 3}
      propertyPath: m_LocalEulerAnglesHint.z
      value: 0
      objectReference: {fileID: 0}
    m_RemovedComponents: []
    m_RemovedGameObjects: []
    m_AddedGameObjects: []
    m_AddedComponents: []
  m_SourcePrefab: {fileID: 100100000, guid: a2371720312654110b434e74f4f9435a, type: 3}
--- !u!4 &4327932830547831877 stripped
Transform:
  m_CorrespondingSourceObject: {fileID: 5657197342317364203, guid: a2371720312654110b434e74f4f9435a, type: 3}
  m_PrefabInstance: {fileID: 8254395498397544366}
  m_PrefabAsset: {fileID: 0}
"
              .Replace("{{_realityName}}", _realityName);
            File.WriteAllText(Path.Combine(thumbnailsPath, $"{_realityName}Thumbnail.prefab"), thumbnailPrefabContent);
            string thumbnailPrefabGUID = System.Guid.NewGuid().ToString("N");
            string thumbnailPrefabMeta = @"fileFormatVersion: 2
guid: {{thumbnailPrefabGUID}}
NativeFormatImporter:
  externalObjects: {}
  mainObjectFileID: 11400000
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"
              .Replace("{{thumbnailPrefabGUID}}", thumbnailPrefabGUID);
            File.WriteAllText(Path.Combine(thumbnailsPath, $"{_realityName}Thumbnail.prefab.meta"), thumbnailPrefabMeta);            

            // Generate package.json
            string packageJsonContent = @"{
    ""name"": ""{{_bundleId}}"",
    ""displayName"": ""{{_namespaceName}}"",
    ""author"": ""Holo Interactive"",
    ""version"": ""1.0.0"",
    ""unity"": ""2022.3"",
    ""dependencies"": {
        ""com.holoi.library.asset-foundation"": ""1.0.0"",
        ""com.holoi.library.holokit-app"": ""1.0.0""
    }
}"
              .Replace("{{_bundleId}}", _bundleId)
              .Replace("{{_namespaceName}}", _namespaceName);
            File.WriteAllText(Path.Combine(packagePath, "package.json"), packageJsonContent);
            string packageJsonGuid = System.Guid.NewGuid().ToString("N");
            string packageJsonMeta = @"fileFormatVersion: 2
guid: {{packageJsonGuid}}
PackageManifestImporter:
  externalObjects: {}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"
              .Replace("{{packageJsonGuid}}", packageJsonGuid);
            File.WriteAllText(Path.Combine(packagePath, "package.json.meta"), packageJsonMeta);

            // Generate Runtime/.asmdef
            string asmdefContent = @"{
    ""name"": ""{{_namespaceName}}"",
    ""rootNamespace"": ""{{_namespaceName}}"",
    ""references"": [
        ""GUID:f1b405ebc01864ce88de31f0e12f8726"",
        ""GUID:bc2dfb0673686496c8eeeb38339860fd"",
        ""GUID:a9420e37d7990b54abdef6688edbe313"",
        ""GUID:92703082f92b41ba80f0d6912de66115"",
        ""GUID:1491147abca9d7d4bb7105af628b223e"",
        ""GUID:dfd0ce20189d48e5b22bb4134b558f5a"",
        ""GUID:b89595f1fd66248258aa3879dcd09222"",
        ""GUID:f008255f36a214aac8c8aa21e586834b"",
        ""GUID:6055be8ebefd69e48b49212b09b47b2f"",
        ""GUID:d04eb9c554ad44ceab303cecf0c0cf82"",
        ""GUID:be03550c1e41142678b69e340a34bdfc"",
        ""GUID:2665a8d13d1b3f18800f46e256720795"",
        ""GUID:d8b63aba1907145bea998dd612889d6b"",
        ""GUID:39ed5662ff2b1b4c2a4578e668a4ac72"",
        ""GUID:63e8524e4a4684596b3143ae6829b702""
    ],
    ""includePlatforms"": [],
    ""excludePlatforms"": [],
    ""allowUnsafeCode"": false,
    ""overrideReferences"": false,
    ""precompiledReferences"": [],
    ""autoReferenced"": true,
    ""defineConstraints"": [],
    ""versionDefines"": [],
    ""noEngineReferences"": false
}
"
              .Replace("{{_namespaceName}}", _namespaceName);
            File.WriteAllText(Path.Combine(runtimePath, $"{_namespaceName}.asmdef"), asmdefContent);
            string asmdefGuid = System.Guid.NewGuid().ToString("N");
            string asmdefMeta = @"fileFormatVersion: 2
guid: {{asmdefGuid}}
AssemblyDefinitionImporter:
  externalObjects: {}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"
              .Replace("{{asmdefGuid}}", asmdefGuid);
            File.WriteAllText(Path.Combine(runtimePath, $"{_namespaceName}.asmdef.meta"), asmdefMeta);


            // Generate reality asset
            string realityAssetContent = @"%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!114 &11400000
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: 3f49b8680080947fea692620c42ea6fe, type: 3}
  m_Name: {{_realityName}}
  m_EditorClassIdentifier: 
  BundleId: {{_namespaceName}}
  DisplayName: '{{_displayName}}'
  DisplayName_Chinese: 
  Description: '{{_displayName}}'
  Description_Chinese: 
  Author: '{{_author}}'
  Author_Chinese: 
  Version: 1.0
  ThumbnailPrefab: {fileID: 11400000, guid: {{thumbnailPrefabGUID}}, type: 3}
  PreviewVideos: []
  TutorialVideos: []
  HardwareRequirement: 
  HardwareRequirement_Chinese: 
  RealityTags:
  - {fileID: 11400000, guid: aa98c375a35f64254adfc51a5dd08e0f, type: 2}
  - {fileID: 11400000, guid: 01efc34f179424c1db6f1f85355b56dd, type: 2}
  RealityEntranceOptions:
  - Text: Enter Reality
    Text_Chinese: 
    IsHost: 1
    PlayerType: 0
    PlayerTypeSubindex: 0
  - Text: Spectator
    Text_Chinese: 
    IsHost: 0
    PlayerType: 2
    PlayerTypeSubindex: 0
  MetaAvatarDescription: 
  MetaAvatarDescription_Chinese: 
  CompatibleMetaAvatarTags: []
  MetaObjectDescription: 
  MetaObjectDescription_Chinese: 
  CompatibleMetaObjectTags: []
  Scene:
    m_SceneAsset: {fileID: 102900000, guid: {{sceneAssetGUID}}, type: 3}
    m_SceneName: {{_realityName}}
"
              .Replace("{{_realityName}}", _realityName)
              .Replace("{{_displayName}}", _displayName)
              .Replace("{{_author}}", _author)
              .Replace("{{sceneAssetGUID}}", sceneAssetGUID)
              .Replace("{{_namespaceName}}", _namespaceName)
              .Replace("{{thumbnailPrefabGUID}}", thumbnailPrefabGUID);

            File.WriteAllText(Path.Combine(assetsPath, $"{_realityName}.asset"), realityAssetContent);
            string realityAssetGUID = System.Guid.NewGuid().ToString("N");
            string realityAssetMeta = @"fileFormatVersion: 2
guid: {{realityAssetGUID}}
NativeFormatImporter:
  externalObjects: {}
  mainObjectFileID: 11400000
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"
              .Replace("{{realityAssetGUID}}", realityAssetGUID);
            File.WriteAllText(Path.Combine(assetsPath, $"{_realityName}.asset.meta"), realityAssetMeta);

            // Generate README.md
            string readmeContent = $@"# {_realityName} 

This is a template reality package.
"
              .Replace("{{_realityName}}",  _realityName);
            File.WriteAllText(Path.Combine(packagePath, "README.md"), readmeContent);

            // Show confirmation message
            EditorUtility.DisplayDialog("Success", "Package created successfully!", "OK");
        }
    }
}
