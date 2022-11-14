using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;

namespace MalbersAnimations.Utilities
{
    [CustomEditor(typeof(BlendShape))/*,CanEditMultipleObjects*/]
    public class BlendShapeEditor : Editor
    {
        BlendShape M;
       // private MonoScript script;
        protected int index = 0;
        SerializedProperty blendShapes, preset, LODs, mesh, random, LoadPresetOnStart;

        private void OnEnable()
        {
            M = (BlendShape)target;
           // script = MonoScript.FromMonoBehaviour(M);
            blendShapes = serializedObject.FindProperty("blendShapes");
            preset = serializedObject.FindProperty("preset");
            LODs = serializedObject.FindProperty("LODs");
            mesh = serializedObject.FindProperty("mesh");
            random = serializedObject.FindProperty("random");
            LoadPresetOnStart = serializedObject.FindProperty("LoadPresetOnStart");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            MalbersEditor.DrawDescription("Adjust the Blend Shapes on the Mesh");

            EditorGUI.BeginChangeCheck();
            {
              //  using (new GUILayout.VerticalScope(MalbersEditor.StyleGray))
                {
                    using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                    {

                        using (var cc = new EditorGUI.ChangeCheckScope())
                        {
                            EditorGUILayout.PropertyField(mesh);
                            if (cc.changed)
                            {
                                serializedObject.ApplyModifiedProperties();
                                M.SetShapesCount();
                                EditorUtility.SetDirty(target);
                            }
                        }

                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(LODs, new GUIContent("LODs", "Other meshes with Blend Shapes to change"));
                        EditorGUI.indentLevel--;
                    }

                    int Length = 0;
                    if (mesh.objectReferenceValue != null)
                        Length = blendShapes.arraySize;

                    using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                        {
                            if (Length > 0)
                            {
                                int pin = serializedObject.FindProperty("PinnedShape").intValue;
                                EditorGUILayout.LabelField(new GUIContent("Pin Shape:              (" + pin + ") |" + M.mesh.sharedMesh.GetBlendShapeName(pin) + "|", "Current Shape Store to modigy When accesing public methods from other scripts"));
                            }
                        }
                        
                         
                        if (Length > 0)
                        {
                            if (M.blendShapes == null)
                            {
                                M.blendShapes = M.GetBlendShapeValues();
                                serializedObject.ApplyModifiedProperties();
                            }

                            for (int i = 0; i < Length; i++)
                            {
                                if (i >= M.mesh.sharedMesh.blendShapeCount) continue;

                                var bs = blendShapes.GetArrayElementAtIndex(i);
                                if (bs != null && M.mesh.sharedMesh != null)
                                {

                                    bs.floatValue = EditorGUILayout.Slider("(" + i.ToString("D2") + ") " + M.mesh.sharedMesh.GetBlendShapeName(i), bs.floatValue, 0, 100);
                                }
                                //EditorUtility.SetDirty(M.mesh);
                            }

                            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                            {
                                EditorGUILayout.PropertyField(preset, new GUIContent("Preset", "Saves the Blend Shapes values to a scriptable Asset"));
                            }


                            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                            {
                                EditorGUILayout.LabelField("On Start", EditorStyles.boldLabel);
                                EditorGUI.BeginDisabledGroup(preset.objectReferenceValue == null);
                                EditorGUILayout.PropertyField(LoadPresetOnStart, new GUIContent("Load Preset", "Load a  Blend Shape Preset on Start"));
                                EditorGUI.EndDisabledGroup();

                                EditorGUI.BeginDisabledGroup(preset.objectReferenceValue != null && LoadPresetOnStart.boolValue);
                                EditorGUILayout.PropertyField(random, new GUIContent("Random", "Make Randoms Blend Shapes at start"));
                                EditorGUI.EndDisabledGroup();

                            }


                            using (new GUILayout.HorizontalScope())
                            {
                                if (GUILayout.Button("Reset"))
                                {
                                     for (int i = 0; i < Length; i++)
                                    {
                                        blendShapes.GetArrayElementAtIndex(i).floatValue = 0; 
                                    }
                                
                                }
                                if (GUILayout.Button("Randomize"))
                                { 
                                    for (int i = 0; i < Length; i++)
                                    {
                                        blendShapes.GetArrayElementAtIndex(i).floatValue = Random.Range(0, 100);
                                    } 
                                }
                                if (GUILayout.Button("Save"))
                                {
                                    if (preset.objectReferenceValue == null)
                                    {
                                        string newBonePath = EditorUtility.SaveFilePanelInProject("Create New Blend Preset", "BlendShape preset", "asset", "Message");

                                        BlendShapePreset bsPreset = CreateInstance<BlendShapePreset>();

                                        AssetDatabase.CreateAsset(bsPreset, newBonePath);

                                        preset.objectReferenceValue = bsPreset;
                                        serializedObject.ApplyModifiedProperties();

                                        Debug.Log("New Blend Shape Preset Created");
                                        M.SavePreset();

                                    }
                                    else
                                    {
                                        if (EditorUtility.DisplayDialog("Overwrite Blend Shape Preset", "Are you sure to overwrite the preset?", "Yes", "No"))
                                        {
                                            M.SavePreset();
                                            GUIUtility.ExitGUI();
                                        }
                                    }
                                }

                                using (new EditorGUI.DisabledGroupScope(preset.objectReferenceValue == null))
                                {
                                    if (GUILayout.Button("Load"))
                                    {
                                        if (preset.objectReferenceValue != null)
                                        {
                                            if (M.preset.blendShapes == null || M.preset.blendShapes.Length == 0)
                                                Debug.LogWarning("The preset " + M.preset.name + " is empty, Please use a Valid Preset");
                                            else
                                            {
                                                M.LoadPreset();
                                                EditorUtility.SetDirty(target);
                                            }
                                        }
                                    } 
                                }
                            }
                        }
                    }
                } 
            }
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Blend Shapes Changed");
                if (M.mesh)  Undo.RecordObject(M.mesh, "Blend Shapes Changed");

                M.UpdateBlendShapes();
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
