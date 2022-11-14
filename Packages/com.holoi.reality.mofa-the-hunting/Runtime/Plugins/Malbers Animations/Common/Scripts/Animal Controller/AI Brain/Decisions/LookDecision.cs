using MalbersAnimations.Scriptables;
using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace MalbersAnimations.Controller.AI
{
    public enum LookFor { MainAnimalPlayer, MalbersTag, UnityTag, Zones, GameObject, ClosestWayPoint, CurrentTarget, TransformVar, GameObjectVar, RuntimeGameobjectSet }


    [CreateAssetMenu(menuName = "Malbers Animations/Pluggable AI/Decision/Look", order = -101)]
    public class LookDecision : MAIDecision
    {
        public override string DisplayName => "General/Look";

        [Range(0, 1)]
        /// <summary>Angle of Vision of the Animal</summary>
        [Tooltip("Shorten the Look Ray to not found the ground by mistake")]
        public float LookMultiplier = 0.9f;

        /// <summary>Range for Looking forward and Finding something</summary>
        [Space, Tooltip("Range for Looking forward and Finding something")]
        public FloatReference LookRange = new FloatReference(15);
        [Range(0, 360)]
        /// <summary>Angle of Vision of the Animal</summary>
        [Tooltip("Angle of Vision of the Animal")]
        public float LookAngle = 120;

        /// <summary>What to look for?? </summary>
        [Space, Tooltip("What to look for??")]
        public LookFor lookFor = LookFor.MainAnimalPlayer;
        [Tooltip("Layers that can block the Animal Eyes")]
        public LayerReference ObstacleLayer = new LayerReference(1);


        [Space(20), Tooltip("If the what we are looking for is found then Assign it as a new Target")]
        public bool AssignTarget = true;
        [Tooltip("If the what we are looking for is found then also start moving")]
        public bool MoveToTarget = true;
        [Tooltip("Remove Target when loose sight:\nIf the Target No longer on the Field of View: Set the Target from the AIControl as NULL")]
        public bool RemoveTarget = false;
        [Tooltip("Select randomly one of the potential targets, not the first one found")]
        public bool ChooseRandomly = false;

        [Space]
        [Tooltip("Look for this Unity Tag on an Object")]
        public string UnityTag = string.Empty;
        [Tooltip("Look for an Specific GameObject by its name")]
        public string GameObjectName = string.Empty;


        [RequiredField, Tooltip("Transform Reference value. This value should be set by a Transform Hook Component")]
        public TransformVar transform;
        [RequiredField, Tooltip("GameObject Reference value. This value should be set by a GameObject Hook Component")]
        public GameObjectVar gameObject;

        [RequiredField, Tooltip("GameObjectSet. Search for all  GameObjects Set in the Set")]
        public RuntimeGameObjects gameObjectSet;

        /// <summary>Custom Tags you want to find</summary>
        [Tooltip("Custom Tags you want to find")]
        public Tag[] tags;
        /// <summary>Type of Zone we want to find</summary>
        [Tooltip("Type of Zone we want to find")]
        // [Utilities.Flag]
        public ZoneType zoneType;

        [Tooltip("Search for all zones")]
        public bool AllZones = true;
        /// <summary>ID value of the Zone we want to find</summary>
        [Tooltip("ID value of the Zone we want to find")]
        [Min(-1)] public int ZoneID = -1;

        [Tooltip("Mode Zone Index")]
        [Min(-1)] public int ZoneModeAbility = -1;

        public Color debugColor = new Color(0, 0, 0.7f, 0.3f);

        void Reset() => Description = "The Animal will look for an Object using a cone view";


        public override bool Decide(MAnimalBrain brain, int index) => Look_For(brain, false, index);

        public override void FinishDecision(MAnimalBrain brain, int index)
        {
            Look_For(brain, true, index); //This will assign the Target in case its true
        }

        public override void PrepareDecision(MAnimalBrain brain, int index)
        {
            //brain.DecisionsVars[index].gameobjects = null;
            //brain.DecisionsVars[index].Components = null;

            switch (lookFor)
            {
                case LookFor.MalbersTag:

                    if (Tags.TagsHolders == null || tags == null || tags.Length == 0) return;

                    List<GameObject> gtags = new List<GameObject>();

                    foreach (var t in Tags.TagsHolders)
                    {
                        if (t.gameObject.HasMalbersTag(tags))
                            gtags.Add(t.gameObject);
                    }

                    if (gtags.Count > 0) brain.DecisionsVars[index].gameobjects = gtags.ToArray();

                    break;

                case LookFor.UnityTag:
                    if (string.IsNullOrEmpty(UnityTag)) return;
                    brain.DecisionsVars[index].gameobjects = GameObject.FindGameObjectsWithTag(UnityTag);
                    break;
                case LookFor.RuntimeGameobjectSet:
                    if (gameObjectSet == null || gameObjectSet.Count == 0) return;
                    brain.DecisionsVars[index].gameobjects = gameObjectSet.Items.ToArray();
                    break;

                default:
                    break;
            }

            StoreColliders(brain, index);
        }

        /// <summary> Store all renderers found on the GameObjects  </summary>
        private void StoreColliders(MAnimalBrain brain, int index)
        {
            if (brain.DecisionsVars[index].gameobjects != null && brain.DecisionsVars[index].gameobjects.Length > 0)
            {
                var colliders = new List<Collider>();

                for (int i = 0; i < brain.DecisionsVars[index].gameobjects.Length; i++)
                {
                    var AllColliders = brain.DecisionsVars[index].gameobjects[i].GetComponentsInChildren<Collider>();

                    foreach (var c in AllColliders)
                    {
                        if (!c.isTrigger && !MTools.Layer_in_LayerMask(c.gameObject.layer, ObstacleLayer.Value)) colliders.Add(c); //Save only good Colliders
                    }
                }
                brain.DecisionsVars[index].AddComponents(colliders.ToArray());
            }
        }


        /// <summary>  Looks for a gameobject acording to the Look For type.</summary>
        private bool Look_For(MAnimalBrain brain, bool assign, int index)
        {
            switch (lookFor)
            {
                case LookFor.MainAnimalPlayer: return LookForAnimalPlayer(brain, assign);
                case LookFor.MalbersTag: return LookForMalbersTags(brain, assign, index);
                case LookFor.UnityTag: return LookForUnityTags(brain, assign, index);
                case LookFor.Zones: return LookForZones(brain, assign);
                case LookFor.GameObject: return LookForGameObjectByName(brain, assign);
                case LookFor.ClosestWayPoint: return LookForClosestWaypoint(brain, assign);
                case LookFor.CurrentTarget: return LookForTarget(brain, assign);
                case LookFor.TransformVar: return LookForTransformVar(brain, assign);
                case LookFor.GameObjectVar: return LookForGoVar(brain, assign);
                case LookFor.RuntimeGameobjectSet: return LookForGoSet(brain, assign, index);
                default: return false;
            }
        }

        public bool LookForTarget(MAnimalBrain brain, bool assign)
        {
            if (brain.Target == null) return false;

            AssignMoveTarget(brain, brain.Target, assign);
            var Center = brain.TargetAnimal ? brain.TargetAnimal.Center : brain.Target.position;
            return IsInFieldOfView(brain, Center, out _);
        }

        public bool LookForTransformVar(MAnimalBrain brain, bool assign)
        {
            if (transform == null || transform.Value == null) return false;

            AssignMoveTarget(brain, transform.Value, assign);

            var Center =
                transform.Value == brain.Target && brain.AIControl.IsAITarget != null ?
                brain.AIControl.IsAITarget.GetCenter() :
                transform.Value.position;

            return IsInFieldOfView(brain, Center, out _);
        }

        public bool LookForGoVar(MAnimalBrain brain, bool assign)
        {
            if (gameObject == null && gameObject.Value && !gameObject.Value.IsPrefab()) return false;

            AssignMoveTarget(brain, gameObject.Value.transform, assign);

            var Center =
                gameObject.Value.transform == brain.Target && brain.AIControl.IsAITarget != null ?
                brain.AIControl.IsAITarget.GetCenter() :
                gameObject.Value.transform.position;

            return IsInFieldOfView(brain, Center, out _);
        }

        private bool IsInFieldOfView(MAnimalBrain brain, Vector3 Center, out float Distance)
        {
            var Direction_to_Target = (Center - brain.Eyes.position); //Put the Sight a bit higher

            //Important, otherwise it will find the ground for Objects to close to it. Also Apply the Scale
            Distance = Vector3.Distance(Center, brain.Eyes.position) * LookMultiplier;

            if (LookAngle == 0 || LookRange <= 0) return true; //Means the Field of view can be ignored

            if (Distance < LookRange.Value) //Check if whe are inside the Look Radius
            {
                Vector3 EyesForward = Vector3.ProjectOnPlane(brain.Eyes.forward, brain.Animal.UpVector);

                //if (brain.debug)
                //{ 
                //    Debug.Log($"Look Decision {lookFor.ToString()} - [{Distance:F3}]");
                //    Debug.DrawRay(brain.Eyes.position, Direction_to_Target * LookMultiplier, Color.cyan, interval);
                //}

                var angle = Vector3.Angle(Direction_to_Target, EyesForward);

                if (angle < LookAngle)
                {
                    //Need a RayCast to see if there's no obstacle in front of the Animal OBSTACLE LAYER
                    if (Physics.Raycast(brain.Eyes.position, Direction_to_Target, out RaycastHit hit, Distance, ObstacleLayer, QueryTriggerInteraction.Ignore))
                    {
                        if (brain.debug)
                        {
                            Debug.DrawRay(brain.Eyes.position, Direction_to_Target * LookMultiplier, Color.green, interval);
                            Debug.DrawLine(hit.point, Center, Color.red, interval);
                            MTools.DrawWireSphere(Center, Color.red, interval);
                        }

                        //if (brain.debug) Debug.Log($"Look Decision {lookFor.ToString()}: Found Obstacle: [{hit.transform.name}]. " +
                        //    $"Layer: [{LayerMask.LayerToName(hit.transform.gameObject.layer)}]",this);

                        return false; //Meaning there's something between the Eyes of the Animal and the Target
                    }
                    else
                    {
                        if (brain.debug)
                        {
                            Debug.DrawRay(brain.Eyes.position, Direction_to_Target, Color.green, interval);
                            MTools.DrawWireSphere(Center, Color.green, interval);
                        }
                        return true;
                    }
                }

                return false;
            }
            //  Debug.Log($"False (NOT IN Distanc{Distance} > RANGE) {LookRange.Value}" );
            return false;
        }

        private void AssignMoveTarget(MAnimalBrain brain, Transform target, bool assign)
        {
            if (assign)
            {
                if (AssignTarget) brain.AIControl.SetTarget(target, MoveToTarget);
                else if (RemoveTarget) brain.AIControl.ClearTarget();
            }
        }

        public bool LookForZones(MAnimalBrain brain, bool assign)
        {
            var zones = Zone.Zones;
            if (zones == null || zones.Count == 0) return false;  //There's no zone around here

            float minDistance = float.PositiveInfinity;

            Zone FoundZone = null;

            foreach (var zone in zones)
            {
                if (AllZones ||
                    (zone && zone.zoneType == zoneType) &&                      //Check the same Zone Types
                    ZoneID == -1 ||                                             //Check First if its Any Zone
                    zone.ZoneID == ZoneID ||                                    //Check Zone has the same ID
                    zone.zoneType != ZoneType.Mode ||                           //Check if its not a Zone Mode
                    zone.zoneType == ZoneType.Mode && ZoneModeAbility == -1 ||  //Check if it's a Zone Mode but the Ability its any
                    zone.ModeAbilityIndex == ZoneModeAbility)                   //Check if it's a Zone Mode AND Ability Match

                    if (IsInFieldOfView(brain, zone.ZoneCollider.bounds.center, out float Distance) && Distance < minDistance)
                    {
                        minDistance = Distance;
                        FoundZone = zone;
                    }
            }

            if (FoundZone)
            {
                AssignMoveTarget(brain, FoundZone.transform, assign);
                return true;
            }
            return false;
        }

        public bool LookForMalbersTags(MAnimalBrain brain, bool assign, int index)
        {
            if (Tags.TagsHolders == null || tags == null || tags.Length == 0) return false;

            float minDistance = float.MaxValue;
            Transform Closest = null;

            var filtredTags = Tags.GambeObjectbyTag(tags);
            if (filtredTags == null)
                return false;

            if (ChooseRandomly)
            {
                while(filtredTags.Count != 0)
                {
                    int newIndex = Random.Range(0, filtredTags.Count);
                    var go = filtredTags[newIndex].transform;

                    if(go != null)
                    {
                        if (IsInFieldOfView(brain, go.position, out float Distance))
                        {
                            AssignMoveTarget(brain, go, assign);
                            return true;
                        }
                    }
                    filtredTags.RemoveAt(newIndex);
                }
            }
            else
            {
                for (int i = 0; i < filtredTags.Count; i++)
                {
                    var go = filtredTags[i].transform;

                    if (go != null)
                    {
                        if (IsInFieldOfView(brain, go.position, out float Distance))
                        {
                            if (Distance < minDistance)
                            {
                                minDistance = Distance;
                                Closest = go;
                            }
                        }
                    }
                }
            }

            if (Closest)
            {
                AssignMoveTarget(brain, Closest.transform, assign);
                return true;
            }
            return false;
        }

        public bool LookForUnityTags(MAnimalBrain brain, bool assign, int index)
        {
            if (string.IsNullOrEmpty(UnityTag)) return false;

            if (ChooseRandomly)
                return ChooseRandomObject(brain, assign, index);
            return ClosestGameObject(brain, assign, index);
        }

        public bool LookForGoSet(MAnimalBrain brain, bool assign, int index)
        {
            if (gameObjectSet == null || gameObjectSet.Count == 0) return false;

            if (ChooseRandomly)
                return ChooseRandomObject(brain, assign, index);
            return ClosestGameObject(brain, assign, index);
        }



        private bool ClosestGameObject(MAnimalBrain brain, bool assign, int index)
        {
            var All = brain.DecisionsVars[index].gameobjects; //catch all the saved gameobjects

            if (All == null || All.Length == 0) return false;

            float minDistance = float.MaxValue;

            GameObject ClosestGameObject = null;

            for (int i = 0; i < All.Length; i++)
            {
                var go = All[i];

                if (go != null)
                {
                    var Center = go.transform.position;// + new Vector3(0, brain.Animal.Height, 0); //In case there's no height use the animal Default

                    if (brain.DecisionsVars[index].Components != null && brain.DecisionsVars[index].Components.Length > 0)
                    {
                        var bounds = Vector3.zero;
                        int total = 0;
                        foreach (var c in brain.DecisionsVars[index].Components)
                        {
                            if (c != null && c is Collider && c.transform.IsGrandchild(go.transform))
                            {
                                bounds += (c as Collider).bounds.center;
                                total++;
                            }
                        }
                        bounds /= total;

                        if (bounds != Vector3.zero) Center = bounds;
                    }


                    if (IsInFieldOfView(brain, Center, out float Distance))
                    {
                        if (Distance < minDistance)
                        {
                            minDistance = Distance;
                            ClosestGameObject = go;
                        }
                    }
                }
            }

            if (ClosestGameObject)
            {
                AssignMoveTarget(brain, ClosestGameObject.transform, assign);
                return true;
            }
            return false;
        }

        public bool ChooseRandomObject(MAnimalBrain brain, bool assign, int index)
        {
            var All = new List<GameObject>();
            if (brain.DecisionsVars[index].gameobjects != null)
                All.AddRange(brain.DecisionsVars[index].gameobjects); //catch all the saved gameobjects with a tag
            if (All.Count == 0) return false;

            while (All.Count != 0)
            {
                int newIndex = Random.Range(0, All.Count);
                if (All[newIndex] != null)
                {
                    var Center = All[newIndex].transform.position + new Vector3(0, brain.Animal.Height, 0);

                    var renderer = brain.DecisionsVars[index].Components[newIndex];

                    if (renderer != null) Center = (renderer as Renderer).bounds.center;

                    if (IsInFieldOfView(brain, Center, out float Distance))
                    {
                        AssignMoveTarget(brain, All[newIndex].transform, assign);
                        return true;
                    }
                }
                All.RemoveAt(newIndex);
            }

            return false;
        }


        public bool LookForGameObjectByName(MAnimalBrain brain, bool assign)
        {
            if (string.IsNullOrEmpty(GameObjectName)) return false;

            var gameObject = GameObject.Find(GameObjectName);

            if (gameObject)
            {
                AssignMoveTarget(brain, gameObject.transform, assign);
                return IsInFieldOfView(brain, gameObject.transform.position, out _);   //Find if is inside the Field of view
            }
            return false;
        }

        public bool LookForClosestWaypoint(MAnimalBrain brain, bool assign)
        {
            var allWaypoints = MWayPoint.WayPoints;
            if (allWaypoints == null || allWaypoints.Count == 0) return false;  //There's no waypoints  around here

            float minDistance = float.MaxValue;

            MWayPoint closestWayPoint = null;

            foreach (var way in allWaypoints)
            {
                var center = way.GetCenter();
                if (IsInFieldOfView(brain, center, out float Distance))
                {
                    if (Distance < minDistance)
                    {
                        minDistance = Distance;
                        closestWayPoint = way;
                    }
                }
            }

            if (closestWayPoint)
            {
                AssignMoveTarget(brain, closestWayPoint.transform, assign);
                return true; //Find if is inside the Field of view
            }
            return false;
        }

        private bool LookForAnimalPlayer(MAnimalBrain brain, bool assign)
        {
            if (MAnimal.MainAnimal == null || MAnimal.MainAnimal.ActiveStateID == StateEnum.Death) return false; //Means the animal is death or Disable

            AssignMoveTarget(brain, MAnimal.MainAnimal.transform, assign);
            return IsInFieldOfView(brain, MAnimal.MainAnimal.Center, out _);
        }


#if UNITY_EDITOR
        public override void DrawGizmos(MAnimalBrain brain)
        {
            var Eyes = brain.Eyes;

            var scale = brain.Animal ? brain.Animal.ScaleFactor : brain.transform.root.localScale.y;

            if (Eyes != null)
            {
                Color c = debugColor;
                c.a = 1f;

                Vector3 EyesForward = Vector3.ProjectOnPlane(brain.Eyes.forward, Vector3.up);

                Vector3 rotatedForward = Quaternion.Euler(0, -LookAngle * 0.5f, 0) * EyesForward;
                UnityEditor.Handles.color = c;
                UnityEditor.Handles.DrawWireArc(Eyes.position, Vector3.up, rotatedForward, LookAngle, LookRange * scale);
                UnityEditor.Handles.color = debugColor;
                UnityEditor.Handles.DrawSolidArc(Eyes.position, Vector3.up, rotatedForward, LookAngle, LookRange * scale);
            }
        }
#endif
    }



    /// <summary>  Inspector!!!  </summary>

#if UNITY_EDITOR

    [CustomEditor(typeof(LookDecision))]
    [CanEditMultipleObjects]
    public class LookDecisionEditor : Editor
    {
        public static GUIStyle StyleBlue => MTools.Style(new Color(0, 0.5f, 1f, 0.3f));

        SerializedProperty
            Description, UnityTag, debugColor, zoneType, ZoneID, tags, LookRange, LookAngle, lookFor, transform, gameobject, gameObjectSet, AllZones, WaitForTasks, WaitForTask, LookMultiplier,
            MessageID, send, interval, ObstacleLayer, MoveToTarget, AssignTarget, GameObjectName, RemoveTarget, ZoneModeIndex, ChooseRandomly;

        //MonoScript script;
        private void OnEnable()
        {
            // script = MonoScript.FromScriptableObject((ScriptableObject)target);

            Description = serializedObject.FindProperty("Description");
            tags = serializedObject.FindProperty("tags");
            RemoveTarget = serializedObject.FindProperty("RemoveTarget");
            ChooseRandomly = serializedObject.FindProperty("ChooseRandomly");
            GameObjectName = serializedObject.FindProperty("GameObjectName");
            UnityTag = serializedObject.FindProperty("UnityTag");
            LookRange = serializedObject.FindProperty("LookRange");
            zoneType = serializedObject.FindProperty("zoneType");
            lookFor = serializedObject.FindProperty("lookFor");
            MessageID = serializedObject.FindProperty("DecisionID");
            send = serializedObject.FindProperty("send");
            interval = serializedObject.FindProperty("interval");
            LookAngle = serializedObject.FindProperty("LookAngle");
            ObstacleLayer = serializedObject.FindProperty("ObstacleLayer");
            AssignTarget = serializedObject.FindProperty("AssignTarget");
            MoveToTarget = serializedObject.FindProperty("MoveToTarget");
            debugColor = serializedObject.FindProperty("debugColor");
            ZoneID = serializedObject.FindProperty("ZoneID");
            ZoneModeIndex = serializedObject.FindProperty("ZoneModeAbility");
            transform = serializedObject.FindProperty("transform");
            gameobject = serializedObject.FindProperty("gameObject");
            gameObjectSet = serializedObject.FindProperty("gameObjectSet");
            AllZones = serializedObject.FindProperty("AllZones");
            WaitForTasks = serializedObject.FindProperty("WaitForAllTasks");
            WaitForTask = serializedObject.FindProperty("waitForTask");
            LookMultiplier = serializedObject.FindProperty("LookMultiplier");
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            {
                EditorGUILayout.PropertyField(Description);
                EditorGUILayout.PropertyField(MessageID);
                EditorGUILayout.PropertyField(send);
                EditorGUILayout.PropertyField(interval);
                EditorGUILayout.PropertyField(WaitForTasks);
                EditorGUILayout.PropertyField(WaitForTask);

                EditorGUILayout.PropertyField(LookRange);
                EditorGUILayout.PropertyField(LookAngle);
                EditorGUILayout.PropertyField(LookMultiplier);

                EditorGUILayout.PropertyField(lookFor);
                EditorGUILayout.PropertyField(ObstacleLayer);

                LookFor lookforval = (LookFor)lookFor.intValue;

                switch (lookforval)
                {
                    case LookFor.MainAnimalPlayer:
                        break;
                    case LookFor.MalbersTag:
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(tags, true);
                        EditorGUI.indentLevel--;
                        EditorGUILayout.PropertyField(ChooseRandomly);
                        break;
                    case LookFor.UnityTag:
                        EditorGUILayout.PropertyField(UnityTag);
                        EditorGUILayout.PropertyField(ChooseRandomly);
                        break;
                    case LookFor.Zones:
                        var ZoneName = ((ZoneType)zoneType.intValue).ToString();

                        EditorGUILayout.PropertyField(AllZones);

                        if (!AllZones.boolValue)
                        {
                            EditorGUILayout.PropertyField(zoneType, new GUIContent("Zone Type", "Choose between a Mode or a State for the Zone"));
                            EditorGUILayout.PropertyField(ZoneID, new GUIContent(ZoneName + " ID", "ID of the Zone.\n" +
                                "For States is the StateID value\n" +
                                "For Stances is the StanceID value\n" +
                                "For Modes is the ModeID value\n"));

                            if (zoneType.intValue == 0)
                            {
                                EditorGUILayout.PropertyField(ZoneModeIndex, new GUIContent("Ability Index"));

                                if (ZoneModeIndex.intValue == -1)
                                    EditorGUILayout.HelpBox("Ability Index is (-1), it will search for any Ability on the Mode Zone", MessageType.None);
                            }

                            if (ZoneID.intValue < 0)
                                EditorGUILayout.HelpBox(ZoneName + " ID is (-1). It will search for any " + ((ZoneType)zoneType.intValue).ToString() + " zone.", MessageType.None);
                        }
                        break;
                    case LookFor.GameObject:
                        EditorGUILayout.PropertyField(GameObjectName, new GUIContent("GameObject"));
                        break;
                    case LookFor.ClosestWayPoint:
                        break;
                    case LookFor.CurrentTarget:
                        break;
                    case LookFor.TransformVar:
                        EditorGUILayout.PropertyField(transform);
                        break;
                    case LookFor.GameObjectVar:
                        EditorGUILayout.PropertyField(gameobject);
                        break;
                    case LookFor.RuntimeGameobjectSet:
                        EditorGUILayout.PropertyField(gameObjectSet);
                        EditorGUILayout.PropertyField(ChooseRandomly);
                        break;
                    default:
                        break;
                }

                EditorGUILayout.PropertyField(AssignTarget);
                EditorGUILayout.PropertyField(MoveToTarget);
               

                if (!AssignTarget.boolValue)
                {
                    EditorGUILayout.PropertyField(RemoveTarget);
                }
                else
                {
                    RemoveTarget.boolValue = false;
                }


                EditorGUILayout.PropertyField(debugColor);

            }
            // EditorGUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Look Decision Inspector");
                EditorUtility.SetDirty(target);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
