using MalbersAnimations.Events;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif



namespace MalbersAnimations
{
    [AddComponentMenu("Malbers/AI/Waypoint")]
    [HelpURL("https://malbersanimations.gitbook.io/animal-controller/main-components/ai/mwaypoint")]
    public class MWayPoint : MonoBehaviour, IWayPoint
    {
        public static List<MWayPoint> WayPoints;

        public WayPointType pointType = WayPointType.Ground;
        [Tooltip("Distance for AI driven animals to stop when arriving to this gameobject. When is set as the AI Target.")]
        [Min(0)] public float stoppingDistance = 1f;
       
        
        [Tooltip("Distance for AI driven animals to start slowing its speed when arriving to this gameobject. If its set to zero or lesser than the Stopping distance, the Slowing Movement Logic will be ignored")]
        [Min(0)] public float slowingDistance = 0;


        [Tooltip(" When the AI animal arrives to the target, do we Rotate the Animal so it looks at the center of the waypoint?")]
        [SerializeField] private bool m_arriveLookAt = false;

        [Tooltip("Default Height for the Waypoints")]
        [Min(0)] [SerializeField] private float m_height = 0.5f;
        public float Height => m_height;

        [MinMaxRange(0, 60), Tooltip("Waytime range to go to the next destination")]
        public RangedFloat m_WaitTime = new RangedFloat(1, 5);

        public Color DebugColor = Color.red;
        public float WaitTime => m_WaitTime.RandomValue;

        public WayPointType TargetType => pointType;

        public virtual Vector3 GetPosition() => transform.position;
        public Vector3 GetCenter() => transform.position + transform.up * Height;
        public virtual float StopDistance() => stoppingDistance * transform.localScale.y; //IMPORTANT For Scaled objects like the ball
        public virtual float SlowDistance() => slowingDistance * transform.localScale.y; //IMPORTANT For Scaled objects like the ball

        public Transform WPTransform => base.transform;

        [SerializeField] protected List<Transform> nextWayPoints;
        public List<Transform> NextTargets { get => nextWayPoints; set => nextWayPoints = value; }

        public bool ArriveLookAt => m_arriveLookAt;

        [Space]
        public GameObjectEvent OnTargetArrived = new GameObjectEvent();



       protected virtual void OnEnable()
        {
            if (WayPoints == null) WayPoints = new List<MWayPoint>();
            WayPoints.Add(this);
        }

        protected virtual void OnDisable()
        {
            WayPoints.Remove(this);
        }

        public virtual void TargetArrived(GameObject target) => OnTargetArrived.Invoke(target);

        public virtual Transform NextTarget()
        {
            var next = NextTargets.Count > 0 ? NextTargets[UnityEngine.Random.Range(0, NextTargets.Count)] : null;

            if (next != null && !next.gameObject.activeInHierarchy) next = null; //Do not get the Next target if its desactive in herarchy

            return next;
        }

        /// <summary>Returns a Random Waypoint from the Global WaypointList</summary>
        public static Transform GetWaypoint()
        {
            return (WayPoints != null && WayPoints.Count > 1) ? WayPoints[UnityEngine.Random.Range(0, WayPoints.Count)].WPTransform : null;
        }

        /// <summary>Returns a Random Waypoint from the Global WaypointList by its type (Ground, Air, Water)</summary>
        public static Transform GetWaypoint(WayPointType pointType)
        {
            if (WayPoints != null && WayPoints.Count > 1)
            {
                var MWayPoint = WayPoints.Find(item => item.pointType == pointType);

                return MWayPoint ? MWayPoint.WPTransform : null;
            }
            return null;
        }

#if UNITY_EDITOR
        /// <summary>DebugOptions</summary>

        void OnDrawGizmos()
        {
            Gizmos.color = DebugColor;

            if (pointType == WayPointType.Air)
            {
                Gizmos.DrawWireSphere(base.transform.position, stoppingDistance);
                if (stoppingDistance < slowingDistance)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawWireSphere(base.transform.position, slowingDistance);
                }
            }
            else
            {
                MTools.DrawCircle(base.transform.position, Quaternion.FromToRotation( transform.forward,transform.up), stoppingDistance, DebugColor);
               
                if (stoppingDistance < slowingDistance)
                {
                    MTools.DrawCircle(base.transform.position, Quaternion.FromToRotation(transform.forward, transform.up), slowingDistance, Color.cyan);
                }
            }

            Gizmos.color = DebugColor;
           
            Gizmos.DrawRay(transform.position, transform.up * Height);
            Gizmos.DrawWireSphere(transform.position,   Height * 0.1f);
            Gizmos.DrawWireSphere(transform.position + transform.up * Height, Height * 0.1f);
          
            var col = DebugColor;
            col.a = 0.333f;
            Gizmos.color = col;
            Gizmos.DrawSphere(transform.position,  Height * 0.1f);
            Gizmos.DrawSphere(transform.position + transform.up * Height, Height * 0.1f);


            col = Color.white;
            col.a = 0.2f;
            Gizmos.color = col;
            if (nextWayPoints != null)
            {
                foreach (var nw in nextWayPoints)
                {
                    if (nw)  MTools.DrawLine(transform.position, nw.position, 1);
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color =  Color.yellow;
            Gizmos.DrawRay(transform.position, transform.up * Height);
            Gizmos.DrawWireSphere(transform.position, Height * 0.1f);
            Gizmos.DrawWireSphere(transform.position + transform.up * Height, Height * 0.1f);


            if (pointType == WayPointType.Air)
            {
                Gizmos.DrawWireSphere(base.transform.position, stoppingDistance);
            }
            else
            {
                MTools.DrawCircle(base.transform.position, Quaternion.FromToRotation(transform.forward, transform.up), stoppingDistance, Color.yellow);
            }



            Gizmos.color = DebugColor;

            if (nextWayPoints != null)
            {
                foreach (var nw in nextWayPoints)
                {
                    if (nw)
                        MTools.DrawLine(transform.position, nw.position, 3);
                }
            }
        }
#endif
    }


#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(MWayPoint)),CanEditMultipleObjects]
    public class MWayPointEditor : UnityEditor.Editor
    {
        UnityEditor.SerializedProperty
            pointType, stoppingDistance, slowingDistance, WaitTime, nextWayPoints, DebugColor, OnTargetArrived, m_height, m_arriveLookAt;

        MWayPoint M;

        private string[] uniquenames;


        private void OnEnable()
        {
            M = (MWayPoint)target;


            //Get all WP Names
            var allWP = UnityEngine.Object.FindObjectsOfType<MWayPoint>();
            uniquenames = new string[allWP.Length];
            for (int i = 0; i < allWP.Length; i++) uniquenames[i] = allWP[i].name;
            

            pointType = serializedObject.FindProperty("pointType");
            stoppingDistance = serializedObject.FindProperty("stoppingDistance");
            slowingDistance = serializedObject.FindProperty("slowingDistance");
            WaitTime = serializedObject.FindProperty("m_WaitTime");
            nextWayPoints = serializedObject.FindProperty("nextWayPoints");
            DebugColor = serializedObject.FindProperty("DebugColor");
            OnTargetArrived = serializedObject.FindProperty("OnTargetArrived");
            m_height = serializedObject.FindProperty("m_height");
            m_arriveLookAt = serializedObject.FindProperty("m_arriveLookAt");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            MalbersEditor.DrawDescription("Uses this Transform position as the destination point for AI Driven characters");
          //  EditorGUILayout.BeginVertical(MTools.StyleGray);
            {
                UnityEditor.EditorGUILayout.BeginVertical(UnityEditor.EditorStyles.helpBox);
                {
                    UnityEditor.EditorGUILayout.BeginHorizontal();
                    UnityEditor.EditorGUILayout.PropertyField(pointType);
                    UnityEditor.EditorGUILayout.PropertyField(DebugColor, GUIContent.none, GUILayout.Width(40));
                    UnityEditor.EditorGUILayout.EndHorizontal();
                    UnityEditor.EditorGUILayout.PropertyField(m_height);
                    UnityEditor.EditorGUILayout.PropertyField(m_arriveLookAt);
                    UnityEditor.EditorGUILayout.PropertyField(stoppingDistance);
                    UnityEditor.EditorGUILayout.PropertyField(slowingDistance);
                    UnityEditor.EditorGUILayout.PropertyField(WaitTime);
                }
                UnityEditor.EditorGUILayout.EndVertical();

                UnityEditor.EditorGUILayout.BeginVertical(UnityEditor.EditorStyles.helpBox);
                {
                    UnityEditor.EditorGUI.indentLevel++;
                    if (GUILayout.Button("Create Next Waypoint"))
                    {
                        var nextWayP =  UnityEngine.Object.Instantiate(M.gameObject);
                        nextWayP.transform.position += M.gameObject.transform.forward*2;
                        nextWayP.name = UnityEditor.ObjectNames.GetUniqueName(uniquenames, M.name);

                        System.Array.Resize(ref uniquenames, uniquenames.Length + 1);

                        uniquenames[uniquenames.Length - 1] = nextWayP.name;

                        if (M.NextTargets == null) M.NextTargets = new List<Transform>();

                        M.NextTargets.Add(nextWayP.transform);
                        nextWayPoints.serializedObject.ApplyModifiedProperties();

                        nextWayP.GetComponent<MWayPoint>().NextTargets = new List<Transform>(); //Clear the copied nextTargets
                        Selection.activeGameObject = nextWayP.gameObject;
                    }

                    UnityEditor.EditorGUILayout.PropertyField(nextWayPoints, true);
                    UnityEditor.EditorGUI.indentLevel--;
                }
                UnityEditor.EditorGUILayout.EndVertical();
                UnityEditor.EditorGUILayout.PropertyField(OnTargetArrived);
            }
           // UnityEditor.EditorGUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
