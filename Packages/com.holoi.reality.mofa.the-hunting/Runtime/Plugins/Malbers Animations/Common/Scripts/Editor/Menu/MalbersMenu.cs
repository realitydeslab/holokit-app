using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MalbersAnimations
{
    public class MalbersMenu : EditorWindow
    {
        const string URPL_Shader_Path2020 = "Assets/Malbers Animations/Common/Shaders/URP_Malbers Shaders 2020.unitypackage";
        const string URPL_Shader_Path2021 = "Assets/Malbers Animations/Common/Shaders/URP_Malbers Shaders 2021.unitypackage";
        const string HRPL20_Shader_Path = "Assets/Malbers Animations/Common/Shaders/HDRP_MalbersShaders 2020.unitypackage";


        [MenuItem("Tools/Malbers Animations/Malbers URP Shaders [Unity 2020]", false, 1)]
        public static void UpgradeMaterialsURPL2020() => AssetDatabase.ImportPackage(URPL_Shader_Path2020, true);



        [MenuItem("Tools/Malbers Animations/Malbers URP Shaders [Unity 2021]", false, 1)]
        public static void UpgradeMaterialsURPL2021() => AssetDatabase.ImportPackage(URPL_Shader_Path2021, true);



        [MenuItem("Tools/Malbers Animations/Malbers HDRP Shaders", false, 1)]
        public static void UpgradeMaterialsHRPL2020() => AssetDatabase.ImportPackage(HRPL20_Shader_Path, true);



        [MenuItem("Tools/Malbers Animations/Create Test Scene", false, 100)]
        public static void CreateTestPlane()
        {
            var all = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects().ToList();

            var mainCam = all.Find(x => x.name == "Main Camera");
            if (mainCam)
            { DestroyImmediate(mainCam); }


            var TestPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            TestPlane.transform.localScale = new Vector3(20, 1, 20);
            TestPlane.GetComponent<MeshRenderer>().sharedMaterial = AssetDatabase.LoadAssetAtPath("Assets/Malbers Animations/Common/Shaders/Ground_20.mat", typeof(Material)) as Material;
            TestPlane.isStatic = true;

            var BrainCam = AssetDatabase.LoadAssetAtPath("Assets/Malbers Animations/Common/Cinemachine/CM Brain.prefab", typeof(GameObject)) as GameObject;
            var CMFreeLook = AssetDatabase.LoadAssetAtPath("Assets/Malbers Animations/Common/Cinemachine/CM FreeLook Main.prefab", typeof(GameObject)) as GameObject;
            var WolfLite = AssetDatabase.LoadAssetAtPath("Assets/Malbers Animations/Animal Controller/Wolf Lite/Wolf Lite.prefab", typeof(GameObject)) as GameObject;
            var Steve = AssetDatabase.LoadAssetAtPath("Assets/Malbers Animations/Animal Controller/Human/Steve.prefab", typeof(GameObject)) as GameObject;

            if (BrainCam) PrefabUtility.InstantiatePrefab(BrainCam);
            if (CMFreeLook) PrefabUtility.InstantiatePrefab(CMFreeLook);
            if (WolfLite) PrefabUtility.InstantiatePrefab(WolfLite);
            if (Steve) PrefabUtility.InstantiatePrefab(Steve);
        }



        [MenuItem("Tools/Malbers Animations/Integrations", false, 10)]
        public static void OpenIntegrations() => Application.OpenURL("https://malbersanimations.gitbook.io/animal-controller/annex/integrations");


        [MenuItem("Tools/Malbers Animations/Tools/Remove All MonoBehaviours from Selected", false, 100)]
        public static void RemoveMono()
        {
            var allGo = Selection.gameObjects;

            if (allGo != null)
            {
                foreach (var selected in allGo)
                {
                    var AllComponents = selected.GetComponentsInChildren<MonoBehaviour>(true);

                    Debug.Log($"Removed {AllComponents.Length} from {selected}", selected);

                    foreach (var comp in AllComponents)
                    {
                        var t = comp.gameObject;
                        DestroyImmediate(comp);
                        EditorUtility.SetDirty(t);
                    }
                }
            }
        }
    }
}
