using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditorInternal;
using UnityEditor;
#endif


namespace MalbersAnimations.Controller.AI
{
    [CreateAssetMenu(menuName = "Malbers Animations/Pluggable AI/Decision/AND Decision", order = 100)]
    public class ANDDecision : MAIDecision
    {
        public override string DisplayName => "General/AND";
       
        [HideInInspector,Tooltip("Selected Index of the list in the inspector")] public int list_index;

        public List<MAIDecision> decisions = new List<MAIDecision>();
        public List<bool> invert = new List<bool>();

        public bool debug;

        public override void PrepareDecision(MAnimalBrain brain, int Index)
        {
            if (invert.Count != decisions.Count) invert.Resize(decisions.Count);

            foreach (var d in decisions) d.PrepareDecision(brain, Index);
        }

        public override bool Decide(MAnimalBrain brain, int Index)
        {
            for (int i = 0; i < decisions.Count; i++)
            {
                bool Decision = decisions[i].Decide(brain, Index);

                if (invert[i]) Decision = !Decision;
                if (debug) Debug.Log($"[{brain.Animal.name}] -> [{(invert[i] ? "NOT " : " " )}{decisions[i].name}] -> [{Decision}]",this);
                if (!Decision) return false;
            }
            return true;
        }
        public override void FinishDecision(MAnimalBrain brain, int Index)
        {
            foreach (var d in decisions) d?.FinishDecision(brain, Index);
        }

        public override void DrawGizmos(MAnimalBrain brain)
        {
            if (decisions != null)
                foreach (var d in decisions) d?.DrawGizmos(brain);
        } 

        void Reset() { Description = "All Decisions on the list  must be TRUE in order to sent a True Decision"; }
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(ANDDecision))]
    public class ANDDecisionEd : Editor
    {
        private SerializedProperty Description, MessageID, send, interval, decisions, invert, list_index, debug;
        private ReorderableList list;
        private List<Type> DecisionType;

        ANDDecision ANDD;

        private GUIContent plus;

        private void OnEnable()
        {
            FindTarget();
            if (plus == null) plus = UnityEditor.EditorGUIUtility.IconContent("d_Toolbar Plus");

            Description = serializedObject.FindProperty("Description");
            debug = serializedObject.FindProperty("debug");
            MessageID = serializedObject.FindProperty("DecisionID");
            send = serializedObject.FindProperty("send");
            interval = serializedObject.FindProperty("interval");
            decisions = serializedObject.FindProperty("decisions");
            invert = serializedObject.FindProperty("invert");
            list_index = serializedObject.FindProperty("list_index");
            DecisionType = MTools.GetAllTypes<MAIDecision>();

            AndDecisions();

            ResizeInvert();

            EditorUtility.SetDirty(target);
        }

        protected virtual void ResizeInvert() => ANDD.invert.Resize(ANDD.decisions.Count);
        //protected virtual void ResizeDecisionList() => ANDD.decisions.Resize(ANDD.decisions.Count + 1);

        protected virtual void FindTarget() => ANDD = (ANDDecision)target;
        protected virtual string ListLabel => "AND";

        private void AndDecisions()
        {
            list = new ReorderableList(serializedObject, decisions)
            {
                drawHeaderCallback = rect =>
                 EditorGUI.LabelField(rect, new GUIContent($"   Invert   Decisions [{ListLabel}] ")),

                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    var element = decisions.GetArrayElementAtIndex(index);
                    var inv = invert.GetArrayElementAtIndex(index);


                    var NotW = 39f;
                    var r = new Rect(rect) { y = rect.y + 2, height = EditorGUIUtility.singleLineHeight, width = rect.width - NotW - 4 };

                    var space = element.objectReferenceValue == null;

                    if (space)
                        r.width -= 22;

                    var InRect = new Rect(r);
                    InRect.width = NotW;

                    r.x += NotW + 4;

                    var defaultColor = GUI.backgroundColor;

                    if (list_index.intValue == index)GUI.backgroundColor = Color.yellow;
                    EditorGUI.PropertyField(r, element, GUIContent.none);
                    GUI.backgroundColor = defaultColor;


                    var S = new GUIStyle(EditorStyles.miniButton);

                    S.fontStyle = inv.boolValue ? FontStyle.Bold : S.fontStyle;

                    var currentColor = GUI.color;
                    GUI.color = inv.boolValue ? ((GUI.color * 0.6f) + (Color.red * 0.4f)) : currentColor;
                    inv.boolValue = GUI.Toggle(InRect, inv.boolValue, new GUIContent("NOT", "Invert decision value"), S);
                    GUI.color = currentColor;
                    if (space)
                    {
                        var AddButtonRect = new Rect(rect)
                        {
                            x = rect.width + 22,
                            width = 22,
                            y = rect.y + 2,
                            height = EditorGUIUtility.singleLineHeight
                        };

                        // if (!internalData.boolValue)
                        if (GUI.Button(AddButtonRect, plus, EditorStyles.helpBox))
                        {
                            MTools.AddScriptableAssetContextMenu(element, typeof(MAIDecision),
                                MTools.GetSelectedPathOrFallback());
                        }
                    }
                },
                onAddCallback = AddDecision, 
                onSelectCallback =  SelectedItem,
                onRemoveCallback = list =>
                {
                    var decision = decisions.GetArrayElementAtIndex(list.index).objectReferenceValue;

                    if (decision != null)
                    {
                        if (EditorUtility.DisplayDialog("Remove Decision", "Deleting a Decision cannot be undone. Are you sure you want to delete it?", "Yes", "No"))
                        {
                            string Path = AssetDatabase.GetAssetPath(decision);

                            if (Path == AssetDatabase.GetAssetPath(target)) //mean it was created inside the AI STATE
                            {
                                decisions.GetArrayElementAtIndex(list.index).objectReferenceValue = null;
                                DestroyImmediate(decision, true); //Delete the internal asset!
                                decisions.DeleteArrayElementAtIndex(list.index);
                                decisions.serializedObject.ApplyModifiedProperties();
                                AssetDatabase.SaveAssets();
                            }
                            else
                            {
                                decisions.DeleteArrayElementAtIndex(list.index);
                            }
                        }
                    }
                    else
                    {
                        decisions.DeleteArrayElementAtIndex(list.index);
                        decisions.serializedObject.ApplyModifiedProperties();
                    }
                    
                    MTools.CheckListIndex(list);
                    ResizeInvert();
                    GUIUtility.ExitGUI();
                    EditorUtility.SetDirty(target); 
                }
            };
        }

        private void SelectedItem(ReorderableList list)
        {
            list_index.intValue = list.index;
        }

        private void AddDecision(ReorderableList list)
        {
            var addMenu = new GenericMenu();

            decisions.InsertArrayElementAtIndex(decisions.arraySize);
            invert.InsertArrayElementAtIndex(invert.arraySize);


            serializedObject.ApplyModifiedProperties();

            EditorUtility.SetDirty(target);


            for (int i = 0; i < DecisionType.Count; i++)
            {
                Type st = DecisionType[i];


                //Fast Ugly get the name of the Asset thing
                MAIDecision t = (MAIDecision)CreateInstance(st);
                var Rname = t.DisplayName;
                DestroyImmediate(t);

                //var Rname = Regex.Replace(st.Name, @"([a-z])([A-Z])", "$1 $2");

                addMenu.AddItem(new GUIContent(Rname), false, () => AddTDecision(st));
            }


            addMenu.AddSeparator("");
            addMenu.AddItem(new GUIContent("Empty"), false, () => 
            {
                var index = decisions.arraySize - 1;
                decisions.GetArrayElementAtIndex(index).objectReferenceValue = null;
            }
            );
            addMenu.ShowAsContext();

        }


        private void AddTDecision(Type desicion)
        {
            MAIDecision des = (MAIDecision)CreateInstance(desicion);
            des.hideFlags = HideFlags.None;
            des.name = "D_" + desicion.Name;
            AssetDatabase.AddObjectToAsset(des, AssetDatabase.GetAssetPath(target));
            AssetDatabase.SaveAssets();

            //decisions.InsertArrayElementAtIndex(decisions.arraySize);
            //invert.InsertArrayElementAtIndex(invert.arraySize);

            var index = decisions.arraySize - 1;
            decisions.GetArrayElementAtIndex(index).objectReferenceValue = des;

            EditorUtility.SetDirty(des);
            EditorUtility.SetDirty(target);
            serializedObject.ApplyModifiedProperties();


            ResizeInvert();

            list.index = index;
        }


        //private void CheckListIndex(ReorderableList list)
        //{
        //    list.index -= 1;
        //    if (list.index == -1 && list.serializedProperty.arraySize > 0) //In Case you remove the first one
        //        list.index = 0;
        //}

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(Description);
            EditorGUILayout.PropertyField(MessageID);
            EditorGUILayout.PropertyField(send);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(interval);
            MalbersEditor.DrawDebugIcon(debug);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();


            for (int i = 0; i < decisions.arraySize; i++)
            {
                if (decisions.GetArrayElementAtIndex(i).objectReferenceValue == null)
                {
                    EditorGUILayout.HelpBox("The Brain cannot contain empty Tasks. Set the missing tasks", MessageType.Error);
                    break;
                }
            }

            list.DoLayoutList();
            list.index = list_index.intValue;

            if (list.index != -1 && list.count > list.index)
            {
                var element = decisions.GetArrayElementAtIndex(list.index);

                if (element != null && element.objectReferenceValue != null)
                {
                    var asset = element.objectReferenceValue;

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                    EditorGUILayout.LabelField("Decision: " + asset.name, EditorStyles.boldLabel);

                    EditorGUILayout.BeginHorizontal();
                    {
                        asset.name = EditorGUILayout.TextField("Name", asset.name);
                        element.serializedObject.ApplyModifiedProperties();
                        EditorUtility.SetDirty(asset);

                        if (GUILayout.Button(new GUIContent("R", "Update the Asset name"), GUILayout.Width(20)))
                        {
                            string taskPath = AssetDatabase.GetAssetPath(asset);
                            string targetPath = AssetDatabase.GetAssetPath(target);

                            // Check if the asset itself is external or internal to the target
                            if (taskPath != targetPath)
                                AssetDatabase.RenameAsset(taskPath, asset.name);

                            AssetDatabase.SaveAssets();
                            EditorGUIUtility
                                .PingObject(
                                    asset); //Final way of changing the name of the asset... dirty but it works
                        }


                        if (GUILayout.Button(new GUIContent("E", "Extract the task into its own file"),  GUILayout.Width(20)))
                        {
                            ExtractDecisionFromList(asset, element, list.index);
                        }
                    }
                    EditorGUILayout.EndHorizontal();



                    MTools.DrawObjectReferenceInspector(element);
                    EditorGUILayout.EndVertical();
                }
            }

            serializedObject.ApplyModifiedProperties();
        }


        private void ExtractDecisionFromList(UnityEngine.Object asset, SerializedProperty element, int index)
        {
            string taskPath = AssetDatabase.GetAssetPath(asset);
            string targetPath = AssetDatabase.GetAssetPath(target);
            if (taskPath == targetPath)
            {
                UnityEngine.Object clone = MTools.ExtractObject(asset, index);
                if (!clone)
                    return;

                // Remove from list
                DestroyImmediate(asset, true);

                // Add as external decision
                SerializedProperty decision = element.FindPropertyRelative("decision");
                decision.objectReferenceValue = clone;
                list.index = -1;  
                EditorUtility.SetDirty(target);
                serializedObject.ApplyModifiedProperties();
                AssetDatabase.SaveAssets();

                EditorGUIUtility.PingObject(clone);
            }
            else
            {
                // Checking this beforehand is quite in-efficient as the AssetDatabase.GetAssetPath() is slow
                // If there is a better way to check whether it's an internal or external asset then that could be used
                Debug.LogWarning("Cannot extract already extracted decision");
            }
        }
    }
#endif
}
