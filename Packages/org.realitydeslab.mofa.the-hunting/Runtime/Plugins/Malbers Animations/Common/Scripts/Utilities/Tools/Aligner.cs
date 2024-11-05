using MalbersAnimations.Scriptables;
using System.Collections;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
namespace MalbersAnimations.Utilities
{
    [AddComponentMenu("Malbers/Utilities/Aling/Aligner")]
    public class Aligner : MonoBehaviour, IAlign
    {
      
        public TransformReference mainPoint = new TransformReference();
      
        public TransformReference secondPoint = new TransformReference();

        /// <summary>The Target will move close to the Aligner equals to the Radius</summary>
        [Min(0)] public float LookAtRadius;

        /// <summary>Time needed to do the alignment</summary>
        [Min(0)] public float AlignTime = 0.25f;

        [Tooltip("Add an offset to the rotation alignment")]
        public float AngleOffset = 0;

        //[Tooltip("Add an offset to the Position alignment")]
        //public float PosOffset = 0;
        /// <summary></summary>
        public AnimationCurve AlignCurve = new AnimationCurve(MTools.DefaultCurve);

        /// <summary></summary>
        public bool AlignPos = true;
        /// <summary></summary>
        public bool AlignRot = true;
        /// <summary>When Rotation is Enabled then It will find the closest Rotation</summary>
        public bool DoubleSided = true;
        /// <summary>Align a gameObject Looking at the Aligner</summary>
        public bool AlignLookAt = false;
      
        ///// <summary>Minimum Distance the animal will move if the Radius is greater than zero</summary>
        //public float LookAtDistance;
        public Color DebugColor = new Color(1, 0.23f, 0, 1f);

        public bool Active { get => enabled; set => enabled = value; }

        public Transform MainPoint => mainPoint.Value;
        public Transform SecondPoint => secondPoint.Value;

        public virtual void Align(GameObject Target) => Align(Target.transform);

        public virtual void Align(Collider Target) => Align(Target.transform.root);

        public virtual void Align(Component Target) => Align(Target.transform.root);
        public virtual void StopAling() { StopAllCoroutines(); }
        public virtual void Align_Self_To(GameObject Target) => Align_Self_To(Target.transform);

        public virtual void Align_Self_To(Collider Target) => Align_Self_To(Target.transform.root);

        public virtual void Align_Self_To(Component Target) => Align_Self_To(Target.transform.root);

        public virtual void Align_Self_To(Transform reference)
        {
            if (Active && MainPoint && reference != null)
            {
                if (AlignLookAt)
                {
                    StartCoroutine(AlignLookAtTransform(mainPoint, reference, AlignTime, AlignCurve));  //Align Look At the Zone
                    if (LookAtRadius > 0) StartCoroutine(MTools.AlignTransformRadius(reference, mainPoint.position, AlignTime, LookAtRadius, AlignCurve));  //Align Look At the Zone
                }
                else
                {
                    if (AlignPos)
                    {
                        Vector3 AlingPosition = reference.position;
                        StartCoroutine(MTools.AlignTransform_Position(MainPoint, AlingPosition, AlignTime, AlignCurve));
                    }
                    if (AlignRot)
                    {
                        Quaternion Side1 = reference.rotation;
                        Quaternion self = MainPoint.rotation;

                        if (DoubleSided)
                        {
                            Quaternion Side2 = reference.rotation * Quaternion.Euler(0, 180, 0);

                            var Side1Angle = Quaternion.Angle(self, Side1);
                            var Side2Angle = Quaternion.Angle(self, Side2);

                            StartCoroutine(MTools.AlignTransform_Rotation(MainPoint, Side1Angle < Side2Angle ? Side1 : Side2, AlignTime, AlignCurve));
                        }
                        else
                            StartCoroutine(MTools.AlignTransform_Rotation(MainPoint, Side1, AlignTime, AlignCurve));
                    }
                }
            }
        }

        public virtual void Align(Transform TargetToAlign)
        {
            if (Active && MainPoint && TargetToAlign != null)
            {
                if (AlignLookAt)
                {
                    StartCoroutine(AlignLookAtTransform(TargetToAlign, mainPoint, AlignTime, AlignCurve));  //Align Look At the Zone
                    
                    //Align Look At the Zone
                    if (LookAtRadius > 0)
                        StartCoroutine(MTools.AlignTransformRadius(TargetToAlign, mainPoint.position, AlignTime, LookAtRadius, AlignCurve)); 
                   
                }
                else
                {
                    var TargetPos = TargetToAlign.transform.position;
                    Vector3 AlingPosition = MainPoint.position;

                    if (SecondPoint)                //In case there's a line ... move to the closest point between the two transforms
                        AlingPosition = MTools.ClosestPointOnLine(MainPoint.position, SecondPoint.position, TargetPos);

                    Vector3 AlingPosOpposite = transform.InverseTransformPoint(AlingPosition);
                    AlingPosOpposite.z *= -1;
                    AlingPosOpposite = transform.TransformPoint(AlingPosOpposite);

                    var Distance1 = Vector3.Distance(TargetPos, AlingPosition);
                    var Distance2 = Vector3.Distance(TargetPos, AlingPosOpposite);


                    if (AlignPos)
                    {
                        if (DoubleSided)
                        {
                            AlingPosition = Distance2 < Distance1 ? AlingPosOpposite : AlingPosition;
                        }

                        StartCoroutine(MTools.AlignTransform_Position(TargetToAlign.transform, AlingPosition, AlignTime, AlignCurve));
                    }
                    if (AlignRot)
                    {
                        Quaternion Side1 = MainPoint.rotation;
                        var AnimalRot = TargetToAlign.transform.rotation;

                        if (DoubleSided)
                        {
                            var Side2 = Side1 * Quaternion.Euler(0, 180, 0);

                            if (Distance1 == Distance2) //If the distance are equal, it means that we need to check the angles then
                            {
                                Distance1 = Quaternion.Angle(AnimalRot, Side1);
                                Distance2 = Quaternion.Angle(AnimalRot, Side2);
                            }

                            Side1 = Distance2 < Distance1 ? Side2 : Side1;
                        }
                            
                        StartCoroutine(
                            MTools.AlignTransform_Rotation(TargetToAlign.transform, Side1 * Quaternion.Euler(0, AngleOffset, 0), AlignTime, AlignCurve));
                    }
                }
            }
        }

         
        /// <summary>
        /// Makes a transform Rotate towards another using LookAt Rotation
        /// </summary>
        /// <param name="t1">Transform that it will be rotated</param>
        /// <param name="t2">Transform reference to Look At</param>
        /// <param name="time">time to do the lookat alignment</param>
        /// <param name="curve">curve for the aligment</param>
        /// <returns></returns>
        IEnumerator AlignLookAtTransform(Transform t1, Transform t2, float time, AnimationCurve curve = null)
        {
            float elapsedTime = 0;

            var Wait = new WaitForFixedUpdate();

            Quaternion CurrentRot = t1.rotation;
            Vector3 direction = (t2.position - t1.position).normalized;
            direction.y = t1.forward.y;
            Quaternion FinalRot = Quaternion.LookRotation(direction) * Quaternion.Euler(0, AngleOffset, 0);

            while ((time > 0) && (elapsedTime <= time))
            {
                float result = curve != null ? curve.Evaluate(elapsedTime / time) : elapsedTime / time;               //Evaluation of the Pos curve

                t1.rotation = Quaternion.SlerpUnclamped(CurrentRot, FinalRot, result);

                elapsedTime += Time.fixedDeltaTime;

                yield return Wait;
            }
            t1.rotation = FinalRot;
        }



        IEnumerator AlignTransform_Position(Transform t1, Vector3 NewPosition, float time, AnimationCurve curve = null)
        {
            float elapsedTime = 0;

            Vector3 CurrentPos = t1.position;

            t1.SendMessage("ResetDeltaRootMotion", SendMessageOptions.DontRequireReceiver); //Nasty but it works

            while ((time > 0) && (elapsedTime <= time))
            {
                float result = curve != null ? curve.Evaluate(elapsedTime / time) : elapsedTime / time;               //Evaluation of the Pos curve
                t1.position = Vector3.LerpUnclamped(CurrentPos, NewPosition, result);
                elapsedTime += Time.deltaTime;

                yield return null;
            }
            t1.position = NewPosition;
        }



#if UNITY_EDITOR

        void Reset()
        {
            mainPoint =  transform;
        } 

        void OnDrawGizmos()
        {
            var WireColor = new Color(DebugColor.r, DebugColor.g, DebugColor.b, 1);
            if (MainPoint)
            {
                if (AlignLookAt && LookAtRadius > 0)
                {
                    UnityEditor.Handles.color = DebugColor;
                    UnityEditor.Handles.DrawWireDisc(MainPoint.position, transform.up, LookAtRadius);
                }

                if (SecondPoint)
                {
                    Gizmos.color = WireColor;
                    Gizmos.DrawLine(MainPoint.position, SecondPoint.position);
                    Gizmos.DrawCube(MainPoint.position, Vector3.one * 0.05f);
                    Gizmos.DrawCube(SecondPoint.position, Vector3.one * 0.05f);

                    if (DoubleSided)
                    {
                        var AlingPoint1Opp = transform.InverseTransformPoint(MainPoint.position);
                        var AlingPoint2Opp = transform.InverseTransformPoint(SecondPoint.position);

                        AlingPoint1Opp.z *= -1;
                        AlingPoint2Opp.z *= -1;
                        AlingPoint1Opp = transform.TransformPoint(AlingPoint1Opp);
                        AlingPoint2Opp = transform.TransformPoint(AlingPoint2Opp);

                        Gizmos.DrawLine(AlingPoint1Opp, AlingPoint2Opp);
                        Gizmos.DrawCube(AlingPoint1Opp, Vector3.one * 0.05f);
                        Gizmos.DrawCube(AlingPoint2Opp, Vector3.one * 0.05f);
                    }
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (AlignLookAt && LookAtRadius > 0 && MainPoint)
            {
                UnityEditor.Handles.color = new Color(1, 1, 0, 1);
                UnityEditor.Handles.DrawWireDisc(MainPoint.position, transform.up, LookAtRadius);
            }
        }
#endif
    }


#if UNITY_EDITOR

    [CustomEditor(typeof(Aligner)), CanEditMultipleObjects]
    public class AlignEditor : Editor
    {

        SerializedProperty
            AlignPos, AlignRot, AlignLookAt, AlingPoint1, AlingPoint2, AlignTime, AlignCurve, DoubleSided, LookAtRadius, DebugColor, LookAtRadiusTime, AngleOffset;

        // MonoScript script;
        private void OnEnable()
        {
            //script = MonoScript.FromMonoBehaviour((MonoBehaviour)target);

            AlignPos = serializedObject.FindProperty("AlignPos");
            AngleOffset = serializedObject.FindProperty("AngleOffset");
            AlignRot = serializedObject.FindProperty("AlignRot");
            AlignLookAt = serializedObject.FindProperty("AlignLookAt");
            AlingPoint1 = serializedObject.FindProperty("mainPoint");
            AlingPoint2 = serializedObject.FindProperty("secondPoint");
            AlignTime = serializedObject.FindProperty("AlignTime");
            AlignCurve = serializedObject.FindProperty("AlignCurve");
            DoubleSided = serializedObject.FindProperty("DoubleSided");
            LookAtRadius = serializedObject.FindProperty("LookAtRadius");
            DebugColor = serializedObject.FindProperty("DebugColor");
            //PosOffset = serializedObject.FindProperty("PosOffset");
            LookAtRadiusTime = serializedObject.FindProperty("LookAtRadiusTime");
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            MalbersEditor.DrawDescription("Aligns the Position and Rotation of an Target object relative this gameobject");

            EditorGUI.BeginChangeCheck();
         //   EditorGUILayout.BeginVertical(MalbersEditor.StyleGray);
            {
                //MalbersEditor.DrawScript(script);

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    EditorGUILayout.BeginHorizontal();

                    var currentGUIColor = GUI.color;
                    var selected = (GUI.color + Color.green) / 2;


                    GUI.color = AlignPos.boolValue ? selected : currentGUIColor;
                    AlignPos.boolValue = GUILayout.Toggle(AlignPos.boolValue, new GUIContent("Position", "Align Position"), EditorStyles.miniButton);

                    GUI.color = AlignRot.boolValue ? selected : currentGUIColor;
                    AlignRot.boolValue = GUILayout.Toggle(AlignRot.boolValue, new GUIContent("Rotation", "Align Rotation"), EditorStyles.miniButton);
                    if (AlignPos.boolValue || AlignRot.boolValue) AlignLookAt.boolValue = false;

                    GUI.color = AlignLookAt.boolValue ? selected : currentGUIColor;
                    AlignLookAt.boolValue = GUILayout.Toggle(AlignLookAt.boolValue, new GUIContent("Look At", "Align a gameObject Looking at the Aligner"), EditorStyles.miniButton);

                    GUI.color = currentGUIColor;

                    if (AlignLookAt.boolValue) AlignPos.boolValue = AlignRot.boolValue = false;

                    EditorGUILayout.PropertyField(DebugColor, GUIContent.none, GUILayout.MaxWidth(40));

                    EditorGUILayout.EndHorizontal();



                    if (AlignRot.boolValue || AlignPos.boolValue)
                        EditorGUILayout.PropertyField(DoubleSided, new GUIContent("Double Sided", "When Rotation is Enabled then It will find the closest Rotation"));

                    if (AlignLookAt.boolValue)
                    {
                        EditorGUILayout.PropertyField(LookAtRadius, new GUIContent("Radius", "The Target will move close to the Aligner equals to the Radius"));

                      // if (LookAtRadius.floatValue > 0)
                        //    EditorGUILayout.PropertyField(LookAtRadiusTime, new GUIContent("Look At Align Time", "Time to move The Target to the Aligner "));
                    }
                }
                EditorGUILayout.EndVertical();


                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    EditorGUILayout.PropertyField(AlingPoint1, new GUIContent("Main Point", "The Target GameObject will move to the Position of the Align Point"));
                    if (AlignPos.boolValue)
                    { 
                        EditorGUILayout.PropertyField(AlingPoint2, 
                            new GUIContent("2nd Point", "If Point End is Active then the Animal will align to the closed position from the 2 align points line"));
                        //EditorGUILayout.PropertyField(PosOffset);
                    }
                }
                EditorGUILayout.EndVertical();


                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(AlignTime, new GUIContent("Align Time", "Time needed to make the Aligment"));
                    EditorGUILayout.PropertyField(AlignCurve, GUIContent.none, GUILayout.MaxWidth(75));
                    EditorGUILayout.EndHorizontal();
                }

                if (AlignRot.boolValue || AlignLookAt.boolValue)
                    EditorGUILayout.PropertyField(AngleOffset);

                EditorGUILayout.EndVertical();


            }
        //    EditorGUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Aligner Inspector");
                EditorUtility.SetDirty(target);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}