using MalbersAnimations.Scriptables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MalbersAnimations.Events;
using System;

#if UNITY_EDITOR
using UnityEditorInternal;
using UnityEditor;
#endif

namespace MalbersAnimations.Controller
{
    [AddComponentMenu("Malbers/Animal Controller/Combo Manager")]
    public class ComboManager : MonoBehaviour
    {
        [RequiredField] public MAnimal animal;

        public int Branch = 0;
        public List<Combo> combos = new List<Combo>();

        [Tooltip("Current Active Combo Index")]
        public IntReference ActiveComboIndex = new IntReference(0);

        /// <summary>Current Active Combo</summary>
        public Combo ActiveCombo { get; internal set; }

        [Tooltip("Disable Combo Manager if the animal is Sleep.")]
        public bool DisableOnSleep = true;

        public int ActiveComboSequenceIndex { get; internal set; }

        public ComboSequence ActiveComboSequence => ActiveCombo.CurrentSequence;

        /// <summary> Is the manager playing a combo? </summary>
        public bool PlayingCombo { get; internal set; }

        public bool debug;

        private void OnValidate()
        {
            ActiveComboIndex = Mathf.Clamp(ActiveComboIndex.Value,-1, combos.Count - 1);
        }

        private void OnEnable()
        {
            if (!animal) animal = this.FindComponent<MAnimal>();

            ActiveCombo = null; //Reset the Active Combo

            if (!animal)
            {
                Debug.LogWarning("The Combo Manager needs an Animal Component", gameObject);
            }
            else
            {
                animal.OnModeEnd.AddListener(OnModeEnd);

                //Save the Modes in the Combo itself
                for (int i = 0; i < combos.Count; i++)
                {
                    var combo = combos[i];
                    combo.ComboIndex = i;
                    combo.FinishTime = -combo.CoolDown; 
                    combo.CachedMode = animal.Mode_Get(combo.Mode);

                    if (combo.CachedMode == null)
                        Debug.LogError($"Animal {animal.name} does not have the mode {combo.Mode.name}. Please Add it to your animal",this);
                }

                if (ActiveComboIndex >= 0)
                {
                    ActiveCombo = combos[ActiveComboIndex];
                    Restart();
                }
            }
        }
        private void OnDisable() { animal.OnModeEnd.RemoveListener(OnModeEnd); }

        private void OnModeEnd(int modeID, int CurrentExitAbility)
        { 
            if (PlayingCombo)
            {
                if (ActiveComboSequence == null) { Restart(); return; } //Weird bug

                if (ActiveComboSequence.Finisher)
                {
                    ActiveCombo.OnComboFinished.Invoke(ActiveComboSequenceIndex);
                    MDebug($"Combo Finished. <b>[{ActiveComboSequenceIndex}]</b> Branch:<b>[{Branch}]</b>. [Restarting]");
                    Restart();
                    ActiveCombo.FinishTime = Time.time; //cache the time the combo has finished.
                }
                //Are we exiting the Current Secuence or just the Old one??? A new Secuence is playing
                else if (CurrentExitAbility == ActiveComboSequence.Ability)
                {
                    if (!animal.IsPlayingMode) // if is no longer playing a Mode then means it was interruptedd
                    {
                        MDebug($"Incomplete <b>[{ActiveComboSequenceIndex}]</b> Branch: <b>[{Branch}]</b>. [Restarting*]");
                        ActiveCombo.OnComboInterrupted.Invoke(ActiveComboSequenceIndex);
                        Restart();//meaning it got to the end of the combo
                    }
                }
            }
        }

        

        /// <summary> Changes the Active Combo on the Manager  </summary>
        public virtual void SetActiveCombo(int index)
        {
            ActiveComboIndex = index;

            if (ActiveComboIndex < 0) //minus 1 means ignore playing combos
            {
                MDebug($"Combo Manager Disabled. No combo set for activation.-1" );
                selectedComboEditor = -1;
                ActiveCombo = null;
                return; 
            }

            ActiveCombo = combos[ActiveComboIndex];

            MDebug($"Set Active Combo [{ActiveCombo.Name},{index}]");
            
            selectedComboEditor = ActiveComboIndex;
        }

        /// <summary> Changes the Active Combo on the Manager using the Mode ID </summary>
        public virtual void SetActiveCombo(ModeID ComboMode)
        {
            if (ComboMode == null)
            {
                SetActiveCombo(-1);  //meaning that we are disabling all combos
            }
            else
            {
                int index = combos.FindIndex(x => x.Mode == ComboMode);
                SetActiveCombo(index);
            }
        }

        public virtual void SetActiveCombo(IntVar index) => SetActiveCombo(index.Value);

        public virtual void SetActiveCombo(string ComboName)
        {
            int index = combos.FindIndex(x => x.Name == ComboName);
            SetActiveCombo(index);
        }

        public virtual void Play() => TryPlay(Branch);

        public virtual bool TryPlay() => TryPlay(Branch);
        public virtual void Play(int branch) => TryPlay(Branch);
        public virtual bool TryPlay(int branch)
        {
            if ((DisableOnSleep && animal.Sleep) ||  //if animal is sleep or
                !enabled ||     //  the component disabled or
                ActiveComboIndex < 0)
                return false; //Active combo is minus 1 ... ignore playing combos

            if (!animal.IsPlayingMode/* && !animal.IsPreparingMode*/) Restart();   //Means is not Playing any mode so Restart


            Branch = branch;
            if (ActiveCombo != null)
            {
                MDebug($"{ActiveCombo.Name} [Try Play]");
                if (ActiveCombo.InCoolDown)
                {
                    MDebug($"{ActiveCombo.Name} In CoolDown");
                    return false; //Do not Play a Combo if its in cooldown
                }
                return ActiveCombo.Play(this);
            }
            return false;
        }
      
        public virtual void SetBranch(int branch)
        {
            Branch = branch;
        }

        public virtual void Restart()
        {
            ActiveComboSequenceIndex = 0;
            PlayingCombo = false;

            if (ActiveCombo != null)
            {
                ActiveCombo.CurrentSequence = null;  //Clean the current combo secuence
                ActiveCombo.ActiveSequenceIndex = -1;  //Clean the current combo secuence
                foreach (var seq in ActiveCombo.Sequence) seq.Used = false; //Set that the secuenced is used to 
            }
            MDebug("Restart");
        }



        internal void MDebug(string value)
        {
#if UNITY_EDITOR
            if (debug) Debug.Log($"<b><color=orange>[{animal.name}] - [Combo - {(ActiveCombo != null ? ActiveCombo.Name : "NULL" )}] - {value}</color></b>", this);
#endif
        }

        [HideInInspector] public int selectedComboEditor = -1;

        internal Combo GetCombo(ModeID weaponID) => combos.Find(x => x.Mode == weaponID);
    }

    [System.Serializable]
    public class Combo
    {
        public ModeID Mode;
        public string Name = "Combo1";
        public Mode CachedMode;

        [Tooltip("After the Combo is Finished, With a finisher it cannot play again until the cooldown has passed")]
        public FloatReference CoolDown = new FloatReference();

        public float FinishTime;

        public bool InCoolDown => CoolDown > 0 && (Time.time - FinishTime) < CoolDown;

        public List<ComboSequence> Sequence = new List<ComboSequence>();
        public ComboSequence CurrentSequence { get; internal set; }

        /// <summary> Current Index on the list to search combos. This is used to avoid searching already used Sequences on the list</summary>
        public int ActiveSequenceIndex  { get; internal set; }
        //{
        //    get => atvs;
        //    internal set
        //    {
        //        atvs = value;
        //         Debug.Log($"[{Name}] - ActiveSequenceIndex [{value} ]");
        //    }
        //}
        //int atvs;


        public int ComboIndex { get; internal set; }

        public IntEvent OnComboFinished = new IntEvent();
        public IntEvent OnComboInterrupted = new IntEvent();

        public bool Play(ComboManager M)
        {
            var animal = M.animal;

           //If the Animal is not Playing a Mode
            if (!animal.IsPlayingMode 
                || (animal.ActiveMode != CachedMode)) //OR the Mode currently playing is different.... try to activate the Combo in the normal way
            {
                for (int i = 0; i < Sequence.Count; i++)
                {
                    var Starter = Sequence[i];

                    if (!Starter.Used && Starter.Branch == M.Branch && Starter.PreviewsAbility == 0) //Only Start with Started Abilities
                    {
                        // Debug.Log($"CachedMode {CachedMode.Name} + Starter.Ability [{Starter.Ability}]");

                        if (CachedMode.TryActivate(Starter.Ability))
                        {
                            // Debug.Log("TryActivate true!!  COMBO");

                            M.PlayingCombo = true;
                            PlaySequence(M, Starter);
                            ActiveSequenceIndex = i; //Finding which is the active secuence index;
                            return true;
                        }
                        else
                        {
                            M.MDebug($"Try Activate First Sequence ({CachedMode.Name}) Failed ");

                            return false;
                        }
                    }
                }
            }
                //Check if we are playing the same mode or if the current mode has lower priority
            else //if (animal.ActiveMode == CachedMode) 
            {   
                //If we are on a Finisher Secuence Ignore!! This will allow to finish the combo
                if (Sequence[ActiveSequenceIndex].Finisher)
                {
                      //Debug.Log("Finishing Finisher"  );
                    return true; 
                }
                 
                for (int i = ActiveSequenceIndex + 1; i < Sequence.Count; i++) //Search from the next one
                {
                    var s = Sequence[i];

                    if (!s.Used && s.Branch == M.Branch && s.PreviewsAbility != 0 && s.PreviewsAbility == CachedMode.AbilityIndex )
                    {
                        if (s.Activation.IsInRange(animal.ModeTime) && animal.Mode_ForceActivate(Mode, s.Ability)) //Play the nex animation on the sequence
                        {
                            PlaySequence(M, s);
                            ActiveSequenceIndex = i; //Finding which is the active secuence index;
                        }
                        else
                        { 
                            M.MDebug($"Sequence [{ActiveSequenceIndex}]: <b>[{M.ActiveComboSequenceIndex}]</b> - Branch:<b>[{M.Branch}]</b>. NOT IN TIME RANGE YET [{animal.ModeTime}]"); 
                        }
                        return true;
                    }
                } 
            }

          // Debug.Log("FAILED PLAY COMBO MODE!");
           return false;
        }

        private void PlaySequence(ComboManager M, ComboSequence sequence)
        {
            CurrentSequence = sequence; //Store the current sequence
            CurrentSequence.Used = true;


            M.ActiveComboSequenceIndex = Mode.ID * 1000 + sequence.Ability;
            CurrentSequence.OnSequencePlay.Invoke(M.ActiveComboSequenceIndex);

            M.MDebug($"Sequence [{ActiveSequenceIndex}]: <b>[{M.ActiveComboSequenceIndex}]</b> - Branch:<b>[{M.Branch}]</b>. Time: {M.animal.ModeTime:F2}");
        }
    }


    [System.Serializable]
    public class ComboSequence
    {
        [MinMaxRange(0, 1)]
        public RangedFloat Activation = new RangedFloat(0.3f, 0.6f);
        public int PreviewsAbility = 0;
        /// <summary> Ability needed to activate</summary>
        public int Ability = 0;
        /// <summary> Branch used on the combo sequence</summary>
        public int Branch = 0;
        public bool Used;
        /// <summary> Is this Secuence a Finisher Combo? </summary>
        public bool Finisher;
        public IntEvent OnSequencePlay = new IntEvent();
    }



#if UNITY_EDITOR
    [CustomEditor(typeof(ComboManager))]

    public class ComboEditor : Editor
    {
        public static GUIStyle StyleGray => MTools.Style(new Color(0.5f, 0.5f, 0.5f, 0.3f));
        public static GUIStyle StyleBlue => MTools.Style(new Color(0, 0.5f, 1f, 0.3f));
        public static GUIStyle StyleGreen => MTools.Style(new Color(0f, 1f, 0.5f, 0.3f));
        /// <summary> Cached style to use to draw the popup button. </summary>
        private GUIStyle popupStyle;


        private int branch, prev, current;

        SerializedProperty Branch, combos, animal, selectedComboEditor, debug, DisableOnSleep,
            ActiveComboIndex;
        private Dictionary<string, ReorderableList> SequenceReordable = new Dictionary<string, ReorderableList>();
        private ReorderableList CombosReor;

        private ComboManager M;
        private int SelectedAbilityIndex,IndexAbility;

        private void OnEnable()
        {
            M = (ComboManager)target;

            animal = serializedObject.FindProperty("animal");
            combos = serializedObject.FindProperty("combos");

            Branch = serializedObject.FindProperty("Branch");
            DisableOnSleep = serializedObject.FindProperty("DisableOnSleep");

            selectedComboEditor = serializedObject.FindProperty("selectedComboEditor");
            debug = serializedObject.FindProperty("debug");
            ActiveComboIndex = serializedObject.FindProperty("ActiveComboIndex");
            DrawComboList();
        }

        private void DrawComboList()
        {
            CombosReor = new ReorderableList(serializedObject, combos, true, true, true, true)
            {
                drawHeaderCallback = (rect) => 
                {
                    float half = rect.width / 2;
                    var IDIndex = new Rect(rect.x, rect.y, 45, EditorGUIUtility.singleLineHeight);
                    var IDName = new Rect(rect.x + 45, rect.y, half - 15 - 45, EditorGUIUtility.singleLineHeight);
                    var IDRect = new Rect(rect.x + half + 10, rect.y, half - 10, EditorGUIUtility.singleLineHeight);

                    EditorGUI.LabelField(IDIndex, "Index");
                    EditorGUI.LabelField(IDName, " Name");
                    EditorGUI.LabelField(IDRect, "  Mode");
                },

                drawElementCallback = (rect, index, isActive, isFocused) => 
                {
                    var element = combos.GetArrayElementAtIndex(index);
                    var Mode = element.FindPropertyRelative("Mode");
                    var Name = element.FindPropertyRelative("Name");
                    rect.y += 2;

                    float half = rect.width / 2;

                    var IDIndex = new Rect(rect.x, rect.y, 25, EditorGUIUtility.singleLineHeight);
                    var IDName = new Rect(rect.x + 25, rect.y, half - 15 - 25, EditorGUIUtility.singleLineHeight);
                    var IDRect = new Rect(rect.x + half + 10, rect.y, half - 10, EditorGUIUtility.singleLineHeight);

                    var oldColor = GUI.contentColor;

                    if (index == M.ActiveComboIndex)
                    {
                        GUI.contentColor = Color.yellow;
                    }


                    EditorGUI.LabelField(IDIndex, "(" + index.ToString() + ")");
                    EditorGUI.PropertyField(IDName, Name, GUIContent.none);
                    EditorGUI.PropertyField(IDRect, Mode, GUIContent.none);

                    GUI.contentColor = oldColor;
                },

               
                onSelectCallback = (list) =>  selectedComboEditor.intValue = list.index ,

                onRemoveCallback = (list) =>
                {  
                    // The reference value must be null in order for the element to be removed from the SerializedProperty array.
                    combos.DeleteArrayElementAtIndex(list.index);
                    list.index -= 1;

                    if (list.index == -1 && combos.arraySize > 0) list.index = 0;   //In Case you remove the first one

                    selectedComboEditor.intValue--;

                    list.index = Mathf.Clamp(list.index, 0, list.index - 1);

                    EditorUtility.SetDirty(target);
                } 
            };
        }
       
        private void DrawSequence(int ComboIndex, SerializedProperty combo, SerializedProperty sequence)
        {
            ReorderableList Reo_AbilityList;
            string listKey = combo.propertyPath;

            if (popupStyle == null)
            {
                popupStyle = new GUIStyle(GUI.skin.GetStyle("PaneOptions"));
                popupStyle.imagePosition = ImagePosition.ImageOnly;
            }

            if (SequenceReordable.ContainsKey(listKey))
            {
                Reo_AbilityList = SequenceReordable[listKey]; // fetch the reorderable list in dict
            }
            else
            {
                Reo_AbilityList = new ReorderableList(combo.serializedObject, sequence, true, true, true, true)
                {
                    drawElementCallback = (rect, index, isActive, isFocused) =>
                    {
                        rect.y += 2;

                        var Height = EditorGUIUtility.singleLineHeight;
                        var element = sequence.GetArrayElementAtIndex(index);

                        //var Activation = element.FindPropertyRelative("Activation");
                        var PreviewsAbility = element.FindPropertyRelative("PreviewsAbility");
                        var Ability = element.FindPropertyRelative("Ability");
                        var Branch = element.FindPropertyRelative("Branch");
                        var useD = element.FindPropertyRelative("Used");
                        var finisher = element.FindPropertyRelative("Finisher");
                        var Activation = element.FindPropertyRelative("Activation");


                        var IDRect = new Rect(rect) { height = Height };

                        var ActivationRect = new Rect(rect) { height = Height, width = rect.width - 17 };
                        ActivationRect.y += Height + 2;

                        float wid = rect.width / 2;

                        var IRWidth = 30f;
                        var Sep = -10f;
                        var Offset = 40f;

                        float xx = IRWidth + Offset;

                        var IndexRect = new Rect(IDRect) { width = IRWidth };
                        var BranchRect = new Rect(IDRect) { x = xx, width = 45 };
                        var PrevARect = new Rect(IDRect) { x = 75 + xx + Sep + 5, width = wid - 15 - Sep - 20 - 45 };
                        
                        var AbilityRect = new Rect(IDRect) { x = wid + xx + Sep + 35, width = wid - 15 - Sep - 60};
                        var FinisherRect = new Rect(IDRect) { x = IDRect.width + 35, width = 20 };

                        var style = new GUIStyle(EditorStyles.label);

                        if (!useD.boolValue && Application.isPlaying) style.normal.textColor = Color.green; //If the Combo is not used turn the combos to Green

                       
                        EditorGUI.LabelField(IndexRect, "(" + index.ToString() + ")", style);
                        var oldCColor = GUI.contentColor;
                        var oldColor = GUI.color;

                        if (PreviewsAbility.intValue <= 0)
                        {
                            GUI.contentColor = Color.green;
                            finisher.boolValue = false; //FINISHER COMBOS CANNOT BE STARTERS
                        }

                        if (finisher.boolValue) GUI.contentColor = Color.cyan;

                        if (Application.isPlaying && M.ActiveComboIndex == ComboIndex)
                        {
                            if (M.ActiveCombo != null)
                            {
                                var Index = M.ActiveCombo.ActiveSequenceIndex;

                                if (Index == index) //Paint Active Index
                                {
                                    GUI.contentColor =
                                    GUI.color = Color.yellow;

                                    if (M.ActiveComboSequence != null && M.ActiveComboSequence.Finisher) //Paint finisher
                                    {
                                        GUI.contentColor =
                                        GUI.color = (Color.red + Color.yellow) / 2;
                                    }
                                }
                                else if (Index > index)  //Paint Used Index
                                {
                                    GUI.contentColor =
                                    GUI.color = Color.gray;
                                }

                            }
                        }

                        EditorGUI.PropertyField(BranchRect, Branch, GUIContent.none);

                        // Calculate rect for configuration first button
                        Rect PrevbuttonRect = new Rect(PrevARect);
                        PrevbuttonRect.yMin += popupStyle.margin.top;
                        PrevbuttonRect.width = popupStyle.fixedWidth + popupStyle.margin.right;
                        PrevbuttonRect.x -= 20;
                        PrevbuttonRect.height = EditorGUIUtility.singleLineHeight;


                        // Calculate rect for configuration first button
                        Rect NextbuttonRect = new Rect(AbilityRect);
                        NextbuttonRect.yMin += popupStyle.margin.top;
                        NextbuttonRect.width = popupStyle.fixedWidth + popupStyle.margin.right;
                        NextbuttonRect.x -= 20;
                        NextbuttonRect.height = EditorGUIUtility.singleLineHeight;


                        int result = -1;
                        var popupOptions = new List<string>();
                        var AbilitiesIndex = new List<int>();

                        popupOptions.Add("[0] Combo Starter");
                        AbilitiesIndex.Add(0);


                        if (IndexAbility == index)
                        {
                            rect.height = Height * 2;
                        }

                        if (M.animal != null)
                        {
                            var Mode = M.animal.Mode_Get(M.combos[ComboIndex].Mode);

                            if (Mode != null && Mode.Abilities != null)
                            {
                                foreach (var item in Mode.Abilities)
                                {
                                    popupOptions.Add("[" + item.Index.Value + "] " + item.Name);
                                    AbilitiesIndex.Add(item.Index.Value);
                                }

                                result = EditorGUI.Popup(PrevbuttonRect, result, popupOptions.ToArray(), popupStyle);

                                if (result != -1) PreviewsAbility.intValue = AbilitiesIndex[result];
                                result = -1;

                                result = EditorGUI.Popup(NextbuttonRect, result, popupOptions.ToArray(), popupStyle);
                                if (result != -1) Ability.intValue = AbilitiesIndex[result];
                            }
                        }
                        EditorGUI.PropertyField(PrevARect, PreviewsAbility, GUIContent.none);
                        EditorGUI.PropertyField(AbilityRect, Ability, GUIContent.none);
                        if (PreviewsAbility.intValue > 0) EditorGUI.PropertyField(FinisherRect, finisher, GUIContent.none);

                        
                       if (PreviewsAbility.intValue == 0)
                            EditorGUI.LabelField(ActivationRect,"Sequence starter. It doesn't require an Activation Time",EditorStyles.whiteLabel);
                        else
                            EditorGUI.PropertyField(ActivationRect, Activation,new GUIContent($"Activaton Time [Ab:{PreviewsAbility.intValue}]"));

                        GUI.contentColor = oldCColor;
                        GUI.color = oldColor;


                        var r = new Rect(ActivationRect);
                        r.y += Height+2;
                        r.x -= 20;
                        r.width += 43;
                        r.height = 1f;

                        EditorGUI.DrawRect(r, new Color(0,0,0,0.25f));


                        if (index == SelectedAbilityIndex)
                        {
                            branch = Branch.intValue;
                            prev = PreviewsAbility.intValue;
                            current = Ability.intValue;
                        }
                    },

                    drawHeaderCallback = rect =>
                    {
                        var Height = EditorGUIUtility.singleLineHeight;
                        var IDRect = new Rect(rect) { height = Height };

                        float wid = rect.width / 2;
                        var IRWidth = 30f;
                        var Sep = -10f;
                        var Offset = 40f;

                        float xx = IRWidth + Offset;

                        var IndexRect = new Rect(IDRect) { width = IRWidth + 5 };
                        var BranchRect = new Rect(IDRect) { x = xx, width = 45 };
                        var PrevARect = new Rect(IDRect) { x = 75 + xx + Sep + 5, width = wid - 15 - Sep - 20 - 45 };
                        var AbilityRect = new Rect(IDRect) { x = wid + xx + Sep + 25, width = wid - 15 - Sep - 90 };
                        var FinisherRect = new Rect(IDRect) { x = IDRect.width - 10, width = 50 };

                        EditorGUI.LabelField(IndexRect, "Index");
                        EditorGUI.LabelField(BranchRect, " Branch");
                        EditorGUI.LabelField(PrevARect,
                            new GUIContent("Required Ability", "Mode Ability [Index] on the Animal required to activate a sequence"));
                        EditorGUI.LabelField(AbilityRect,
                            new GUIContent("Play Ability", "Next Mode Ability [Index] to Play on the Animal if the Active Mode Animation is withing the Activation Range limit "));
                        EditorGUI.LabelField(FinisherRect, new GUIContent("Finisher", "Combo Finisher"));
                    },

                    elementHeightCallback = (int index) =>
                    {
                        return EditorGUIUtility.singleLineHeight * 2 +6;
                    }
                   
                };

                SequenceReordable.Add(listKey, Reo_AbilityList);  //Store it on the Editor
            }

            Reo_AbilityList.DoLayoutList();

            SelectedAbilityIndex = Reo_AbilityList.index;

            Reo_AbilityList.elementHeightCallback(SelectedAbilityIndex);

            if (SelectedAbilityIndex != -1)
            {
                var element = sequence.GetArrayElementAtIndex(SelectedAbilityIndex);

                var Activation = element.FindPropertyRelative("Activation");
                var OnSequencePlay = element.FindPropertyRelative("OnSequencePlay");

                var lbl = "Branch:[" + branch + "] Required Ab:[" + prev + "] Next Ab:[" + current + "]";

                //EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                //{

                //    EditorGUILayout.LabelField("Sequence Properties - " + lbl);
                //    EditorGUILayout.PropertyField(Activation, new GUIContent("Activation", "Range of the Preview Animation the Sequence can be activate"));
                //}
               // EditorGUILayout.EndVertical();
                EditorGUILayout.PropertyField(OnSequencePlay, new GUIContent("Sequence Play - " + lbl));
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            MalbersEditor.DrawDescription
                ("Use Modes to create combo sequences. Active Combos using ComboManager.Play(int Branch)\nBranches are the different Inputs Values");

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.PropertyField(animal);

                if (animal.objectReferenceValue != null)
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        using (var cc = new EditorGUI.ChangeCheckScope())
                        {
                            int oldActiveCIndex = M.ActiveComboIndex.Value;

                            EditorGUILayout.PropertyField(ActiveComboIndex, new GUIContent("Active Combo Index", "Active Combo"));
                            if (cc.changed && Application.isPlaying)
                            {
                                ActiveComboIndex.serializedObject.ApplyModifiedProperties();

                                M.SetActiveCombo(M.ActiveComboIndex.Value);
                            }
                        }


                        MalbersEditor.DrawDebugIcon(debug);
                        //  debug.boolValue = GUILayout.Toggle(debug.boolValue,new GUIContent("D","Debug"), EditorStyles.miniButton, GUILayout.Width(23));
                    }

                    EditorGUILayout.PropertyField(Branch, new GUIContent("Branch",
                        "Current Branch ID for the Combo Sequence, if this value change then the combo will play different sequences"));

                EditorGUILayout.PropertyField(DisableOnSleep);
                    CombosReor.DoLayoutList();

                    CombosReor.index = selectedComboEditor.intValue;
                    int IndexCombo = CombosReor.index;

                    if (IndexCombo != -1)
                    {

                        var combo = combos.GetArrayElementAtIndex(IndexCombo);

                        if (combo != null)
                        {
                            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                            {
                                var name = combo.FindPropertyRelative("Name");

                                EditorGUI.indentLevel++;    
                                EditorGUILayout.PropertyField(combo, new GUIContent($"[{name.stringValue}] Combo"), false);
                                EditorGUI.indentLevel--;    
                           

                                if (combo.isExpanded)
                                {
                                    var Mode = combo.FindPropertyRelative("Mode");
                                    var OnComboFinished = combo.FindPropertyRelative("OnComboFinished");
                                    var OnComboInterrupted = combo.FindPropertyRelative("OnComboInterrupted");
                                    var CoolDown = combo.FindPropertyRelative("CoolDown");

                                    using (new GUILayout.VerticalScope(StyleGreen))
                                    {
                                        EditorGUILayout.LabelField("Green Sequences are combo starters", EditorStyles.boldLabel);
                                    }


                                    EditorGUILayout.PropertyField(CoolDown);
                                    EditorGUILayout.LabelField("Combo Sequence List", EditorStyles.boldLabel);
                                    var sequence = combo.FindPropertyRelative("Sequence");

                                    if (Mode.objectReferenceValue)
                                        DrawSequence(IndexCombo, combo, sequence);

                                    EditorGUILayout.PropertyField(OnComboFinished);
                                    EditorGUILayout.PropertyField(OnComboInterrupted);
                                }
                            }
                        }
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}