using MalbersAnimations.Events;
using MalbersAnimations.Scriptables;
using MalbersAnimations.Utilities;
using UnityEngine;
using System;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MalbersAnimations.Controller
{
    [AddComponentMenu("Malbers/Interaction/Pick Up - Drop")]
    public class MPickUp : MonoBehaviour, IAnimatorListener
    {
        [RequiredField, Tooltip("Trigger used to find Items that can be picked Up")]
        public Collider PickUpArea;
        [SerializeField, Tooltip("When an Item is Picked and Hold, the Pick Trigger area will be disabled")]
        private BoolReference m_HidePickArea = new BoolReference(true);
        //public bool AutoPick { get => m_AutoPick.Value; set => m_AutoPick.Value = value; }

        [Tooltip("Bone to Parent the Picked Item")]
        [RequiredField]  public Transform Holder;
        public Vector3 PosOffset;
        public Vector3 RotOffset;
        [Tooltip("Check for tags on the Pickable items")]
        public Tag[] Tags;


        [Tooltip("Layer for the Interact with colliders")]
        [SerializeField] private LayerReference Layer = new LayerReference(-1);
        [SerializeField] private QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore;




        // [Header("Events")]
        public BoolEvent CanPickUp = new BoolEvent();
        [FormerlySerializedAs("OnItem")]
        public GameObjectEvent OnItemPicked = new GameObjectEvent();
        public GameObjectEvent OnItemDrop = new GameObjectEvent();
        public GameObjectEvent OnFocusedItem = new GameObjectEvent();
        public IntEvent OnPicking = new IntEvent();
        public IntEvent OnDropping = new IntEvent();

        public bool debug;
        public float DebugRadius = 0.02f;
        public Color DebugColor = Color.yellow;


        private ICharacterAction character;

        [SerializeField] private TriggerProxy Proxy;

        /// <summary>Does the Animal is holding an Item</summary>

        public bool Has_Item => Item != null;

        [SerializeField] private Pickable item;
        public Pickable Item
        {
            get => item;
            set
            {
                item = value;
             //   OnItem.Invoke(item != null ? item.gameObject : null);
              //  Debug.Log("item: " + item);
            }
        }

        [SerializeField] private Pickable focusedItem;
        public Pickable FocusedItem
        {
            get => focusedItem;
            set
            {
                focusedItem = value;
                OnFocusedItem.Invoke(focusedItem != null ? focusedItem.gameObject : null);
                CanPickUp.Invoke(focusedItem != null);
            }
        }

        private void Awake()
        {
            character = gameObject.FindInterface<ICharacterAction>();

            CheckTriggerProxy();
        }

        private void CheckTriggerProxy()
        {
            if (PickUpArea)
            {
                Proxy = TriggerProxy.CheckTriggerProxy(PickUpArea, Layer, triggerInteraction, transform.root);
            }
            else
            {
                Debug.LogWarning("Please set a Pick up Area");
            }
        }

        private void OnEnable()
        {
            Proxy.OnTrigger_Enter.AddListener(OnGameObjectEnter);
            Proxy.OnTrigger_Exit.AddListener(OnGameObjectExit);
             
            if (Has_Item)  PickUpItem();         //If the animal has an item at start then make all the stuff to pick it up
        }

        private void OnDisable()
        {
            Proxy.OnTrigger_Enter.RemoveListener(OnGameObjectEnter);
            Proxy.OnTrigger_Exit.RemoveListener(OnGameObjectExit);
        }

        void OnGameObjectEnter(Collider col)
        {
            var newItem = col.FindComponent<Pickable>();

            if (newItem && newItem.enabled)
            {
                if (newItem != FocusedItem && FocusedItem != null) //If we are choosing another focused Item then unfocus the one that we had.
                {
                    FocusedItem.Focused = false;
                }

                FocusedItem = newItem;
                FocusedItem.Focused = true;
                Debugging("Focused Item - " + FocusedItem.name);

                if (FocusedItem.AutoPick) TryPickUp();
            }
        }

        void OnGameObjectExit(Collider col)
        {
            if (FocusedItem != null) //Means there's a New Focused Item
            {
                var newItem = col.FindComponent<Pickable>();

                if (newItem == FocusedItem)
                {
                    Debugging("Unfocused Item - " + FocusedItem.name);
                    FocusedItem.Focused = false;
                    FocusedItem = null;
                }
                else
                {
                    //Was another one that is not focused anumore (Make sure is stays unfocused)
                  if (newItem)  newItem.Focused = false;
                }
            }
        }


        public virtual void TryPickUpDrop()
        {
            if (character != null && character.IsPlayingAction) return; //Do not try if the Character is doing an action

            if (!Has_Item)  TryPickUp();
            else            TryDrop();
        }


        public virtual void TryDrop()
        {
            if (!enabled) return; //Do nothing if this script is disabled

            if (item && !item.InCoolDown)
            {
                if (character != null && !character.IsPlayingAction /*&& Item.DropReaction != null*/)
                {
                    Item.OnPreDropped.Invoke(gameObject);
                }

                Debugging("Item Try Drop - " + Item.name);

                if (!item.ByAnimation)
                Invoke(nameof(DropItem), Item.DropDelay.Value);
            }
        }

        

        /// <summary>  Tries the pickup logic checking all the correct conditions if the character does not have an item.  </summary>
        public virtual void TryPickUp()
        {
            if (!isActiveAndEnabled) return; //Do nothing if this script is disabled

            if (FocusedItem && !FocusedItem.InCoolDown)
            {
                if (character != null && !character.IsPlayingAction) //Try Picking UP WHEN THE CHARACTER IS NOT MAKING ANY ANIMATION
                {
                    if (FocusedItem.Align)
                    {
                        StartCoroutine(MTools.AlignLookAtTransform(transform.root, FocusedItem.transform, FocusedItem.AlignTime));
                        StartCoroutine(MTools.AlignTransformRadius(transform.root, FocusedItem.transform.position, FocusedItem.AlignTime, FocusedItem.AlignDistance));
                    }
                   
                    FocusedItem.OnPrePicked.Invoke(gameObject); //Do the On Picked First  
                }
                Debugging("Try Pick Up");  

                if (!FocusedItem.ByAnimation)
                    Invoke(nameof(PickUpItem), FocusedItem.PickDelay.Value);
            }
        }

        /// <summary>Pick Up Logic. It can be called by the ANimator</summary>
        public void PickUpItem()
        {
            if (!isActiveAndEnabled) return; //Do nothing if this script is disabled

            if (Item == null) Item = FocusedItem; //Check for the Picked Item

            if (Item)
            {
                Debugging("Item Picked - " + Item.name);

                if (Holder)
                {
                    var localScale = Item.transform.localScale;
                    Item.transform.parent = Holder;                 //Parent it to the Holder
                    Item.transform.localPosition = PosOffset;       //Offset the Position
                    Item.transform.localEulerAngles = RotOffset;    //Offset the Rotation
                    Item.transform.localScale = localScale;         //Offset the Rotation
                }

                Item.Picker = gameObject;                      //Set on the Item who did the Picking
                Item.Pick();                                    //Tell the Item that it was picked
                OnItemPicked.Invoke(Item.gameObject);           //Invoke the Event
                OnPicking.Invoke(Item.ID);                      //Invoke the Event
                FocusedItem = null;                             //Remove the Focused Item
                
                var item = Item; //Store before collectable

                //Check if the item is a collectable so Pick it and remove it from the 
                if (Item.Collectable)
                { 
                    Item = null;

                    //Enable Disable to find new collectables in the same area
                    PickUpArea.enabled = false;
                    this.Delay_Action(() => PickUpArea.enabled = true);      
                }
                else
                {
                    if (m_HidePickArea.Value)
                        PickUpArea.enabled = false;        //Disable the Pick Up Area
                }
               
                
                if (item.DestroyOnPick)
                {
                    PickUpArea.gameObject.SetActive(true);   //Enable the Pick up Area
                    PickUpArea.enabled = true; //Enable the Collider just in case.
                    Destroy(item.gameObject);
                    Item = null; //Clear the 
                }
                Proxy.ResetTrigger();
            }
        }


        /// <summary> Drops the item logic</summary>
        public virtual void DropItem()
        {
            if (!enabled) return; //Do nothing if this script is disabled
            if (Has_Item)
            {
                Debugging("Item Dropped - " + Item.name);

                Item.Drop();                                    //Tell the item is being droped
                OnItemDrop.Invoke(Item.gameObject);
                OnDropping.Invoke(Item.ID);                     //Invoke the method
                
               // OnItemPicked.Invoke(null);
                Item = null;                                    //Remove the Item

                if (m_HidePickArea.Value)
                    PickUpArea.enabled = (true);         //Enable the Pick up Area

                if (FocusedItem != null && !FocusedItem.AutoPick) Proxy.ResetTrigger();
            }
        }


        private void Debugging(string msg)
        {
#if UNITY_EDITOR
            if (debug) Debug.Log($"[{transform.root.name}] - [{msg}]",this);
#endif
        }

        public virtual bool OnAnimatorBehaviourMessage(string message, object value) => this.InvokeWithParams(message, value);

        [SerializeField] private int Editor_Tabs1;
        private void OnDrawGizmos()
        {
            if (Holder)
            {
                Gizmos.color = DebugColor;
                Gizmos.DrawWireSphere(Holder.TransformPoint(PosOffset), 0.02f);
                Gizmos.DrawSphere(Holder.TransformPoint(PosOffset), 0.02f);

            }
        }
    }

    #region INSPECTOR
#if UNITY_EDITOR
    [CustomEditor(typeof(MPickUp)), CanEditMultipleObjects]
    public class MPickUpEditor : Editor
    {
       
        private SerializedProperty
            PickUpArea, FocusedItem, Editor_Tabs1,  Holder, RotOffset, item, m_HidePickArea, OnFocusedItem, Layer, triggerInteraction, OnItemDrop,
            PosOffset, CanPickUp,  OnDropping, OnPicking,  DebugRadius, OnItem, DebugColor, debug, Tags;

        protected string[] Tabs1 = new string[] { "General", "Events" };


        private void OnEnable()
        { 
            PickUpArea = serializedObject.FindProperty("PickUpArea");
            Layer = serializedObject.FindProperty("Layer");
            triggerInteraction = serializedObject.FindProperty("triggerInteraction");
            m_HidePickArea = serializedObject.FindProperty("m_HidePickArea");

            Holder = serializedObject.FindProperty("Holder");
            PosOffset = serializedObject.FindProperty("PosOffset");
            RotOffset = serializedObject.FindProperty("RotOffset");
            Tags = serializedObject.FindProperty("Tags");

            FocusedItem = serializedObject.FindProperty("focusedItem");
            item = serializedObject.FindProperty("item");

            CanPickUp = serializedObject.FindProperty("CanPickUp");
            //CanDrop = serializedObject.FindProperty("CanDrop");


            OnPicking = serializedObject.FindProperty("OnPicking");
            OnPicking = serializedObject.FindProperty("OnPicking");
            OnItem = serializedObject.FindProperty("OnItemPicked");
            OnItemDrop = serializedObject.FindProperty("OnItemDrop");
            OnDropping = serializedObject.FindProperty("OnDropping");
            OnFocusedItem = serializedObject.FindProperty("OnFocusedItem");


            Editor_Tabs1 = serializedObject.FindProperty("Editor_Tabs1");
            DebugColor = serializedObject.FindProperty("DebugColor");
            DebugRadius = serializedObject.FindProperty("DebugRadius");
            debug = serializedObject.FindProperty("debug");
        }

        public override void OnInspectorGUI()
        {
                serializedObject.Update();
            MalbersEditor.DrawDescription("Pick Up Logic for Pickable Items");
            
            //EditorGUILayout.BeginVertical(MTools.StyleGray);
            {
                Editor_Tabs1.intValue = GUILayout.Toolbar(Editor_Tabs1.intValue, Tabs1);
                if (Editor_Tabs1.intValue == 0) DrawGeneral();
                else DrawEvents();

                if (debug.boolValue)
                {
                    EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                    {
                        EditorGUILayout.PropertyField(DebugRadius);
                        EditorGUILayout.PropertyField(DebugColor, GUIContent.none, GUILayout.MaxWidth(40));
                    }
                    EditorGUILayout.EndHorizontal();
                }

                serializedObject.ApplyModifiedProperties();
            }
               // EditorGUILayout.EndVertical();
        }

        private void DrawGeneral()
        {
            //MalbersEditor.DrawScript(script);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(PickUpArea, new GUIContent("Pick Up Trigger"));
            MalbersEditor.DrawDebugIcon(debug);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(Layer);
            EditorGUILayout.PropertyField(triggerInteraction);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(Tags);
            EditorGUI.indentLevel--;
            EditorGUILayout.PropertyField(m_HidePickArea, new GUIContent("Hide Trigger"));
            EditorGUILayout.EndVertical();


            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.PropertyField(Holder);
                if (Holder.objectReferenceValue)
                {
                    EditorGUILayout.LabelField("Offsets", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(PosOffset, new GUIContent("Position", "Position Local Offset to parent the item to the holder"));
                    EditorGUILayout.PropertyField(RotOffset, new GUIContent("Rotation", "Rotation Local Offset to parent the item to the holder"));
                }
            }
            EditorGUILayout.EndVertical();


            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.PropertyField(item);
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(FocusedItem);
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawEvents()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.PropertyField(CanPickUp, new GUIContent("On Can Pick Item"));
                EditorGUILayout.PropertyField(OnFocusedItem, new GUIContent("On Item Focused"));
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(OnItem, new GUIContent("On Item Picked"));
                EditorGUILayout.PropertyField(OnItemDrop, new GUIContent("On Item Dropped"));
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(OnPicking);
                EditorGUILayout.PropertyField(OnDropping);
            }
            EditorGUILayout.EndVertical();  
        }
    }
#endif
    #endregion
}