using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System;

namespace MalbersAnimations
{
    [CustomEditor(typeof(MInput))]
    public class MInputEditor : Editor
    {
        protected ReorderableList list;
        protected SerializedProperty
            inputs, showInputEvents, IgnoreOnPause, OnInputEnabled, 
            
            ActiveMapIndex, actionMaps, ActiveMap, DefaultMap,


            OnInputDisableds, OnInputDisabled, ResetOnFocusLost;
        private MInput _M;

        private Dictionary<string, ReorderableList> innerListDict = new Dictionary<string, ReorderableList>();
        string[] ActionMapsNames;


        public bool showOnPlayMode = false;

        protected virtual void OnEnable()
        {
            _M = ((MInput)target);

            ResetOnFocusLost = serializedObject.FindProperty("ResetOnFocusLost");
            inputs = serializedObject.FindProperty("inputs");
            OnInputEnabled = serializedObject.FindProperty("OnInputEnabled");
            OnInputDisabled = serializedObject.FindProperty("OnInputDisabled");
            showInputEvents = serializedObject.FindProperty("showInputEvents");

            IgnoreOnPause = serializedObject.FindProperty("IgnoreOnPause");
            ActiveMap = serializedObject.FindProperty("ActiveMap");
             
            ActiveMapIndex = serializedObject.FindProperty("ActiveMapIndex");
            actionMaps = serializedObject.FindProperty("actionMaps");
            DefaultMap = serializedObject.FindProperty("DefaultMap");


            list = new ReorderableList(serializedObject, inputs, true, true, true, true)
            {
                drawElementCallback = DrawElementCallback,
                drawHeaderCallback = HeaderCallbackDelegate,
                onAddCallback = OnAddCallBack
            };

            ActionMapsNames = new string[1]; // Set the first one as One 
            ActionMapsNames[0] = "<Default>";


            showOnPlayMode = false;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            MalbersEditor.DrawDescription("Inputs to connect to components via UnityEvents");

            if (Application.isPlaying)
            {
                showOnPlayMode =
                    GUILayout.Toggle(showOnPlayMode, 
                    new GUIContent("Show Buttons on Play Mode", "This makes the Inspector bit faster"), EditorStyles.miniButton);
            }

            if (!Application.isPlaying || (showOnPlayMode && Application.isPlaying))
            {
                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    EditorGUILayout.PropertyField(IgnoreOnPause);
                    EditorGUILayout.PropertyField(ResetOnFocusLost);
                    DrawRewired();
                }

                DrawListAnEvents();
            }
            serializedObject.ApplyModifiedProperties();
        }

        protected void DrawRewired()
        {
#if REWIRED
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("PlayerID"), new GUIContent("Player ID", "Rewired Player ID"));
                EditorGUILayout.EndVertical();
#endif
        }


        private void CheckActionMaps()
        {
            if (_M.actionMaps != null)
            {
                var count = _M.actionMaps.Count;

                ActionMapsNames = new string[count + 1]; // Set the first one as NONE  
                ActionMapsNames[0] = "Default";

                for (int i = 0; i < count; i++)
                {
                    var nme = _M.actionMaps[i].name;
                    ActionMapsNames[i + 1] = nme;
                }
            }
            else
            {
                ActionMapsNames = new string[1]; // Set the first one as One 
                ActionMapsNames[0] = "Default";
            }
        }

        protected void DrawListAnEvents()
        {
            CheckActionMaps();

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {

                if (_M.actionMaps.Count>0)
                {
                    EditorGUILayout.LabelField("Use the method SetMap(name) to switch Input Maps",MalbersEditor.DescriptionStyle);
                  //  EditorGUILayout.HelpBox("Use the Method SetMap(name) to switch Input Maps", MessageType.Info);
                }


                using (new GUILayout.HorizontalScope())
                {
                    using (var cc = new EditorGUI.ChangeCheckScope())
                    {
                        ActiveMapIndex.intValue = EditorGUILayout.Popup(
                            new GUIContent("Active Input Map", "Map that will be connected to the Animal Controller"),
                            ActiveMapIndex.intValue, ActionMapsNames);

                        if (cc.changed)
                        {
                            serializedObject.ApplyModifiedProperties();

                            if (Application.isPlaying)
                            {
                                if (ActiveMapIndex.intValue == 0)
                                {
                                    _M.ActiveMap = _M.DefaultMap;
                                }
                                else
                                {
                                    _M.ActiveMap = _M.actionMaps[ActiveMapIndex.intValue - 1];
                                }
                            }
                        }
                    }

                    if (!Application.isPlaying)
                    {
                        if (GUILayout.Button(MalbersEditor.Icon_Add, GUILayout.Width(30), GUILayout.Height(18)))
                        {
                            var NewMap = new MInputMap() 
                            { 
                                name = new Scriptables.StringReference($"New Action Map {_M.actionMaps.Count}"), 
                                inputs = new List<InputRow>() 
                            };
                            _M.actionMaps.Add(NewMap);
                            _M.ActiveMapIndex = _M.actionMaps.Count;
                            serializedObject.ApplyModifiedProperties();
                            EditorUtility.SetDirty(_M);

                          //  ActiveMapIndex.intValue++;
                            //serializedObject.ApplyModifiedProperties();
                            CheckActionMaps();

                            Debug.Log("<B>New Action Map Created</B>");
                        }
                    }
                }
                if (ActiveMapIndex.intValue == 0) //Draw Default Input Map
                {
                    list.DoLayoutList();

                    var Index = list.index;

                    if (Index != -1 && Index < inputs.arraySize)
                    {
                        var Element = inputs.GetArrayElementAtIndex(Index);

                        DrawInputEvents(Element);
                    }

                }
                else if (ActionMapsNames.Length > 1 && actionMaps.arraySize > ActiveMapIndex.intValue - 1)
                {
                    var index = ActiveMapIndex.intValue - 1;
                    var selectedMap = actionMaps.GetArrayElementAtIndex(index);

                    if (selectedMap != null)
                    {
                        using (new GUILayout.HorizontalScope())
                        {
                            if (!Application.isPlaying)
                            {
                                var name = selectedMap.FindPropertyRelative("name");
                                EditorGUILayout.PropertyField(name);
                                if (GUILayout.Button(MalbersEditor.Icon_Delete, GUILayout.Width(30)))
                                {
                                    if (EditorUtility.DisplayDialog(
                                    "Remove ActionMap", "Are you sure you want to remove this action map?", "Yes", "Cancel"))
                                    {
                                        actionMaps.DeleteArrayElementAtIndex(ActiveMapIndex.intValue - 1);
                                        ActiveMapIndex.intValue--;
                                        //   CheckActionMaps();
                                        serializedObject.ApplyModifiedProperties();
                                        GUIUtility.ExitGUI();
                                    }
                                }
                            }
                        }
                    }


                    DrawActionMapList(selectedMap, index);
                }
            }

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                if (MalbersEditor.Foldout(showInputEvents, "[Enable/Disable] Events"))
                {
                    EditorGUILayout.PropertyField(OnInputEnabled);
                    EditorGUILayout.PropertyField(OnInputDisabled);
                }
            }
        }

        private void DrawActionMapList(SerializedProperty selectedMap, int inputIndex)
        {
            ReorderableList Reo_AbilityList;

            var inputs = selectedMap.FindPropertyRelative("inputs");
            var element = _M.actionMaps[inputIndex].inputs;

            string listKey = inputs.propertyPath;

            if (innerListDict.ContainsKey(listKey))
            {
                // fetch the reorderable list in dict
                Reo_AbilityList = innerListDict[listKey];
            }
            else
            {
                Reo_AbilityList = new ReorderableList(inputs.serializedObject, inputs, true, true, true, true)
                {
                    drawHeaderCallback = HeaderCallbackDelegate,
                    drawElementCallback = (rect, index, isActive, isFocused) =>
                    {
                        var elementSer = inputs.GetArrayElementAtIndex(index);
                        rect.y += 2;

                        var activeRect = new Rect(rect.x, rect.y, 20, EditorGUIUtility.singleLineHeight);
                        element[index].active.Value = EditorGUI.Toggle(activeRect, element[index].active.Value);
                        DrawRow(rect, elementSer);
                    },

                    onAddCallback = (list) =>
                    {
                      //  Debug.Log("inputIndex = " + inputIndex);
                        Undo.RecordObject(target, "Add New Input");

                        if (_M.actionMaps[inputIndex].inputs == null)
                            _M.actionMaps[inputIndex].inputs = new List<InputRow>();

                        element = _M.actionMaps[inputIndex].inputs;

                        var newInput = new InputRow("New", "InputValue", KeyCode.Alpha0, InputButton.Press, InputType.Input);
                        element.Add(newInput);

                        selectedMap.serializedObject.ApplyModifiedProperties();
                        EditorUtility.SetDirty(target);
                    }
                };

                innerListDict.Add(listKey, Reo_AbilityList);
            }

            Reo_AbilityList.DoLayoutList();

            int SelectedAbility = Reo_AbilityList.index;


            if (SelectedAbility != -1 && SelectedAbility < inputs.arraySize)
            {
                DrawInputEvents(inputs.GetArrayElementAtIndex(SelectedAbility));
            }
        }

        protected void DrawInputEvents(SerializedProperty Element)
        {
            if (Element == null) return;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(Element, new GUIContent($"[{Element.displayName}] Input"), false);
                EditorGUI.indentLevel--;

              //  var inputname =  Element.FindPropertyRelative("name").stringValue;
              //  Element.isExpanded = MalbersEditor.Foldout(Element.isExpanded, $" [{inputname}] Input");
                
                if (Element.isExpanded)
                {

                    var active = Element.FindPropertyRelative("active");
                    var OnInputChanged = Element.FindPropertyRelative("OnInputChanged");
                    var OnInputPressed = Element.FindPropertyRelative("OnInputPressed");
                    var OnInputDown = Element.FindPropertyRelative("OnInputDown");
                    var OnInputUp = Element.FindPropertyRelative("OnInputUp");
                    var OnInputEnable = Element.FindPropertyRelative("OnInputEnable");
                    var OnInputDisable = Element.FindPropertyRelative("OnInputDisable");
                    var ResetOnDisable = Element.FindPropertyRelative("ResetOnDisable");

                    EditorGUILayout.PropertyField(active);
                    EditorGUILayout.PropertyField(ResetOnDisable);
                    EditorGUILayout.Space();
                   // EditorGUILayout.LabelField("Events", EditorStyles.boldLabel);

                    InputButton GetPressed = (InputButton)Element.FindPropertyRelative("GetPressed").enumValueIndex;


                    switch (GetPressed)
                    {
                        case InputButton.Press:
                            EditorGUILayout.PropertyField(OnInputChanged);
                            EditorGUILayout.PropertyField(OnInputPressed);
                            EditorGUILayout.PropertyField(OnInputDown);
                            EditorGUILayout.PropertyField(OnInputUp);
                            break;
                        case InputButton.Down:
                            EditorGUILayout.PropertyField(OnInputDown);
                            EditorGUILayout.PropertyField(OnInputChanged);
                            break;
                        case InputButton.Up:
                            EditorGUILayout.PropertyField(OnInputUp);
                            EditorGUILayout.PropertyField(OnInputChanged);
                            break;
                        case InputButton.LongPress:
                            EditorGUILayout.PropertyField(Element.FindPropertyRelative("LongPressTime"));
                            EditorGUILayout.PropertyField(Element.FindPropertyRelative("SmoothDecrease"));
                            EditorGUILayout.Space();
                            EditorGUILayout.PropertyField(Element.FindPropertyRelative("OnLongPress"), new GUIContent("On Long Press Completed"));
                            EditorGUILayout.PropertyField(Element.FindPropertyRelative("OnPressedNormalized"), new GUIContent("On Pressed Time Normalized"));
                            EditorGUILayout.PropertyField(OnInputDown, new GUIContent("On Input Down"));
                            EditorGUILayout.PropertyField(OnInputUp, new GUIContent("On Pressed Interrupted"));
                            EditorGUILayout.PropertyField(OnInputChanged);
                            break;
                        case InputButton.DoubleTap:
                            EditorGUILayout.PropertyField(Element.FindPropertyRelative("DoubleTapTime"));
                            EditorGUILayout.Space();
                            EditorGUILayout.PropertyField(OnInputDown, new GUIContent("On First Tap"));
                            EditorGUILayout.PropertyField(Element.FindPropertyRelative("OnDoubleTap"));
                            EditorGUILayout.PropertyField(OnInputChanged);
                            break;
                        case InputButton.Toggle:
                            EditorGUILayout.PropertyField(OnInputChanged,  new GUIContent("On Input Toggle"));
                            EditorGUILayout.PropertyField(OnInputDown,  new GUIContent("On Toggle On"));
                            EditorGUILayout.PropertyField(OnInputUp,  new GUIContent("On Toggle Off"));
                            break;  
                        case InputButton.Axis:
                            EditorGUILayout.PropertyField(OnInputChanged);
                            EditorGUILayout.PropertyField(OnInputPressed);
                            EditorGUILayout.PropertyField(OnInputDown);
                            EditorGUILayout.PropertyField(OnInputUp);
                            EditorGUILayout.PropertyField(Element.FindPropertyRelative("OnPressedNormalized"), new GUIContent("On Axis Value Changed"));
                            break;
                        default:
                            break;
                    }

                    OnInputEnable.isExpanded = MalbersEditor.Foldout(OnInputEnable.isExpanded, $"[{Element.displayName}] Enable/Disable");

                    if (OnInputEnable.isExpanded)
                    {
                        EditorGUILayout.PropertyField(OnInputEnable, new GUIContent("On [" + Element.displayName + "] Enabled"));
                        EditorGUILayout.PropertyField(OnInputDisable, new GUIContent("On [" + Element.displayName + "] Disabled"));
                    }
                }
            }
            EditorGUILayout.EndVertical();
        }

        protected void HeaderCallbackDelegate(Rect rect)
        {

            Rect R_1 = new Rect(rect.x + 20, rect.y, (rect.width - 20) / 4 + 12, EditorGUIUtility.singleLineHeight);
            Rect R_2 = new Rect(rect.x + (rect.width - 20) / 4 + 35, rect.y, (rect.width - 20) / 4 - 20, EditorGUIUtility.singleLineHeight);
            Rect R_3 = new Rect(rect.x + ((rect.width - 20) / 4) * 2 + 18, rect.y, ((rect.width - 30) / 4) + 11, EditorGUIUtility.singleLineHeight);
            Rect R_4 = new Rect(rect.x + ((rect.width) / 4) * 3 + 15, rect.y, ((rect.width) / 4) - 15, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(R_1, "   Name", EditorStyles.boldLabel);
            EditorGUI.LabelField(R_2, "   Type", EditorStyles.boldLabel);
            EditorGUI.LabelField(R_3, "  Value", EditorStyles.boldLabel);
            EditorGUI.LabelField(R_4, "Button", EditorStyles.boldLabel);
        }

        protected void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = _M.inputs[index];

            var elementSer = inputs.GetArrayElementAtIndex(index);

            rect.y += 2;
            element.active.Value = EditorGUI.Toggle(new Rect(rect.x, rect.y, 20, EditorGUIUtility.singleLineHeight), element.active.Value);

            DrawRow(rect, elementSer);
        }

        private static void DrawRow(Rect rect, SerializedProperty elementSer)
        {
            Rect R_1 = new Rect(rect.x + 20, rect.y, (rect.width - 20) / 4 + 12, EditorGUIUtility.singleLineHeight);
            Rect R_2 = new Rect(rect.x + (rect.width - 20) / 4 + 35, rect.y, (rect.width - 20) / 4 - 20, EditorGUIUtility.singleLineHeight);
            Rect R_3 = new Rect(rect.x + ((rect.width - 20) / 4) * 2 + 18, rect.y, ((rect.width - 30) / 4) + 11, EditorGUIUtility.singleLineHeight);
            Rect R_4 = new Rect(rect.x + ((rect.width) / 4) * 3 + 15, rect.y, ((rect.width) / 4) - 15, EditorGUIUtility.singleLineHeight);


          //  Rect R_5 = new Rect(rect.width, rect.y,15, EditorGUIUtility.singleLineHeight);



            var name = elementSer.FindPropertyRelative("name");
            var type = elementSer.FindPropertyRelative("type");
            var input = elementSer.FindPropertyRelative("input");
            var key = elementSer.FindPropertyRelative("key");
            var GetPressed = elementSer.FindPropertyRelative("GetPressed");

          //  EditorGUI.PropertyField(R_5, elementSer);

            EditorGUI.PropertyField(R_1, name, GUIContent.none);
            //name.stringValue = EditorGUI.TextField(R_1, name.stringValue, EditorStyles.textField);

            var lockType = false;

            if (GetPressed.enumValueIndex == (int)InputButton.Axis)
            {
                type.enumValueIndex = (int)InputType.Input;
                type.serializedObject.ApplyModifiedProperties();
                lockType = true;
            }


            using (new EditorGUI.DisabledGroupScope(lockType))
            {
                EditorGUI.PropertyField(R_2, type, GUIContent.none);
            }

            if (type.intValue != 1)
                EditorGUI.PropertyField(R_3, input, GUIContent.none);
            else
                EditorGUI.PropertyField(R_3, key, GUIContent.none);

            EditorGUI.PropertyField(R_4, GetPressed, GUIContent.none);
        }

        protected void OnAddCallBack(ReorderableList list)
        {
            Undo.RecordObject(target, "Add New Input");
            if (_M.inputs == null)
                _M.inputs = new System.Collections.Generic.List<InputRow>();
            
            _M.inputs.Add(new InputRow("New", "InputValue", KeyCode.Alpha0, InputButton.Press, InputType.Input));

            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }
    }
}