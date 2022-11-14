using System.Collections;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MalbersAnimations
{
  //  public enum AnimCycle { None, Loop, Repeat, PingPong }

    [CreateAssetMenu(menuName = "Malbers Animations/Extras/Anim Transform", order = 2100)]
    public class TransformAnimation : ScriptableCoroutine
    {
        public enum AnimTransType { TransformAnimation, MountTriggerAdjustment }

        public AnimTransType animTrans = AnimTransType.TransformAnimation;

        static Keyframe[] K = { new Keyframe(0, 0), new Keyframe(1, 1) };

        public float time = 1f;
        public float delay = 1f;
        //public AnimCycle cycle;

        public bool UsePosition = false;
        public Vector3 Position;
        public AnimationCurve PosCurve = new AnimationCurve(K);

        public bool SeparateAxisPos = false;
        public AnimationCurve PosXCurve = new AnimationCurve(K);
        public AnimationCurve PosYCurve = new AnimationCurve(K);
        public AnimationCurve PosZCurve = new AnimationCurve(K);

        public bool UseRotation = false;
        public Vector3 Rotation;
        public AnimationCurve RotCurve = new AnimationCurve(K);

        public bool SeparateAxisRot = false;
        public AnimationCurve RotXCurve = new AnimationCurve(K);
        public AnimationCurve RotYCurve = new AnimationCurve(K);
        public AnimationCurve RotZCurve = new AnimationCurve(K);

        public bool UseScale = false;
        public Vector3 Scale = Vector3.one;
        public AnimationCurve ScaleCurve = new AnimationCurve(K);

        public Vector3 TargetPos { get; private set; }
        public Vector3 TargetRot { get; private set; }
        public Vector3 TargetScale { get; private set; }

        public Vector3 StartPos { get; private set; }
        public Vector3 StartRot { get; private set; }
        public Vector3 StartScale { get; private set; }

        public void Play(Transform item)
        {
            StartCoroutine(item, PlayTransformAnimation(item,time,delay));
        }

        public void PlayForever(Transform item)
        {
            StartCoroutine(item, PlayTransformAnimationForever(item, time, delay));
        }

        internal override void Evaluate(MonoBehaviour mono, Transform target, float time, AnimationCurve curve = null)
        {
            mono.StartCoroutine(PlayTransformAnimation(target, time, 0));
        }

        /// <summary> Plays the Transform Animations for the Selected item   </summary>
        internal IEnumerator PlayTransformAnimation(Transform item, float time,float delay)
        {
            if (item != null)
            {
                if (delay != 0) yield return new WaitForSeconds(delay);         //Wait for the Delay     

                float elapsedTime = 0;

                StartPos = item.localPosition;                                          //Store the Current Position Rotation and Scale
                StartRot = item.localEulerAngles;
                StartScale = item.localScale;

                TargetPos = StartPos + Position;
                TargetRot = StartRot + (Rotation);
                TargetScale = Vector3.Scale(StartScale, Scale);

                while ((time > 0) && (elapsedTime <= time) && item != null)
                {

                    float resultPos = PosCurve.Evaluate(elapsedTime / time);               //Evaluation of the Pos curve
                    float resultRot = RotCurve.Evaluate(elapsedTime / time);               //Evaluation of the Rot curve
                    float resultSca = ScaleCurve.Evaluate(elapsedTime / time);               //Evaluation of the Scale curve

                    if (UsePosition) item.localPosition = Vector3.LerpUnclamped(StartPos, TargetPos, resultPos);

                    if (UseRotation) item.transform.localEulerAngles = Vector3.LerpUnclamped(StartRot, TargetRot, resultRot);

                    if (UseScale) item.transform.localScale = Vector3.LerpUnclamped(StartScale, TargetScale, resultSca);

                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                ExitValue(item);
            }

            yield return null;
            Stop(item);
        }

        internal override void ExitValue(Component component)
        {
            var item = (Transform)component;
            if (item == null) return;

            if (UsePosition)
            {
                float FresultPos = PosCurve.Evaluate(1 / time);               //Evaluation of the Pos curve
                item.localPosition = Vector3.LerpUnclamped(StartPos, TargetPos, FresultPos);
            }
            if (UseRotation)
            {
                float FresultRot = RotCurve.Evaluate(1 / time);               //Evaluation of the Rot curve
                item.transform.localEulerAngles = Vector3.LerpUnclamped(StartRot, TargetRot, FresultRot);
            }
            if (UseScale)
            {
                float FresultSca = ScaleCurve.Evaluate(1 / time);               //Evaluation of the Scale curve
                item.transform.localScale = Vector3.LerpUnclamped(StartScale, TargetScale, FresultSca);
            }
        }


        internal IEnumerator PlayTransformAnimation(Transform item)
        {
            yield return PlayTransformAnimation(item, time, delay);
        }


        internal IEnumerator PlayTransformAnimationForever(Transform item)
        {
            yield return PlayTransformAnimationForever(item, time, delay);
        }


        /// <summary> Plays the Transform Animations for the Selected item   </summary>
        internal IEnumerator PlayTransformAnimationForever(Transform item, float time, float delay)
        {
            if (item != null)
            {
                if (delay != 0) yield return new WaitForSeconds(delay);         //Wait for the Delay     

                float elapsedTime = 0;

                Vector3 StartPos = item.localPosition;                                          //Store the Current Position Rotation and Scale
                Vector3 StartRot = item.localEulerAngles;
                Vector3 StartScale = item.localScale;

                var TargetPos = StartPos + Position;
                var TargetRot = StartRot + (Rotation);
                var TargetScale = Vector3.Scale(StartScale, Scale);

                while (true)
                {
                    float resultPos = PosCurve.Evaluate(elapsedTime / time);               //Evaluation of the Pos curve
                    float resultRot = RotCurve.Evaluate(elapsedTime / time);               //Evaluation of the Rot curve
                    float resultSca = ScaleCurve.Evaluate(elapsedTime /time);               //Evaluation of the Scale curve



                    if (UsePosition)  item.localPosition = Vector3.LerpUnclamped(StartPos, TargetPos, resultPos);
                    if (UseRotation)  item.transform.localEulerAngles= Vector3.LerpUnclamped(StartRot, TargetRot, resultRot);
                    if (UseScale)    item.transform.localScale = Vector3.LerpUnclamped(StartScale, TargetScale, resultSca);

                    elapsedTime += Time.deltaTime;
                    elapsedTime %= time;
                  
                    yield return null;
                }
            }
            yield return null;
            Stop(item);
        }
    }


    //INSPECTOR!!!
#if UNITY_EDITOR

    [CustomEditor(typeof(TransformAnimation)), CanEditMultipleObjects]
    public class TransformAnimationEditor : Editor
    {
        TransformAnimation My;
        private MonoScript script;

        void OnEnable()
        {
            My = (TransformAnimation)target;
            script = MonoScript.FromScriptableObject(My);
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            MalbersEditor.DrawDescription("Use to animate a transform to this values");

            using (var cc = new EditorGUI.ChangeCheckScope())
            {
                using (new GUILayout.VerticalScope())
                {
                    //  MalbersEditor.DrawScript(script);

                    using (new GUILayout.HorizontalScope(EditorStyles.helpBox))
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("animTrans"), new GUIContent("Used for"));
                    }


                    using (new GUILayout.HorizontalScope(EditorStyles.helpBox))
                    {
                        EditorGUIUtility.labelWidth = 50;
                        var type = My.animTrans == TransformAnimation.AnimTransType.TransformAnimation;
                        My.time = EditorGUILayout.FloatField(new GUIContent(type ? "Time" : "Mount", type ? "Duration of the animation" : "Scale for the Mount Animation on the MountTriger Script"), My.time);
                        My.delay = EditorGUILayout.FloatField(new GUIContent(type ? "Delay" : "Dismount", type ? "Delay before the animation is started" : "Scale for the Dismount Animation on the MountTriger Script"), My.delay);
                        EditorGUIUtility.labelWidth = 0;
                    }

                    using (new GUILayout.HorizontalScope(EditorStyles.helpBox))
                    {
                        My.UsePosition = GUILayout.Toggle(My.UsePosition, new GUIContent("Position"), EditorStyles.miniButton);
                        My.UseRotation = GUILayout.Toggle(My.UseRotation, new GUIContent("Rotation"), EditorStyles.miniButton);
                        My.UseScale = GUILayout.Toggle(My.UseScale, new GUIContent("Scale"), EditorStyles.miniButton);
                    }


                    if (My.UsePosition)
                    {
                        using (new GUILayout.VerticalScope())
                        {
                            using (new GUILayout.HorizontalScope(EditorStyles.helpBox))
                            {
                                My.SeparateAxisPos = GUILayout.Toggle(My.SeparateAxisPos, new GUIContent("|", "Separated Axis"), EditorStyles.miniButton);
                                EditorGUILayout.LabelField("Position", GUILayout.MinWidth(50), GUILayout.MaxWidth(100));
                                My.Position = EditorGUILayout.Vector3Field("", My.Position, GUILayout.MinWidth(120));
                                if (!My.SeparateAxisPos) My.PosCurve = EditorGUILayout.CurveField(My.PosCurve, GUILayout.MinWidth(30));
                            }


                            if (My.SeparateAxisPos)
                            {
                                using (new GUILayout.HorizontalScope())
                                {
                                    My.PosXCurve = EditorGUILayout.CurveField(My.PosXCurve, Color.red, new Rect());
                                    My.PosYCurve = EditorGUILayout.CurveField(My.PosYCurve, Color.green, new Rect());
                                    My.PosZCurve = EditorGUILayout.CurveField(My.PosZCurve, Color.blue, new Rect());
                                }

                            }
                        }
                    }

                    if (My.UseRotation)
                    {
                        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                        {
                            using (new GUILayout.HorizontalScope())

                            {
                                My.SeparateAxisRot = GUILayout.Toggle(My.SeparateAxisRot, new GUIContent("|", "Separated Axis"), EditorStyles.miniButton);
                                EditorGUILayout.LabelField("Rotation", GUILayout.MinWidth(50), GUILayout.MaxWidth(100));
                                My.Rotation = EditorGUILayout.Vector3Field("", My.Rotation, GUILayout.MinWidth(120));
                                if (!My.SeparateAxisRot) My.RotCurve = EditorGUILayout.CurveField(My.RotCurve, GUILayout.MinWidth(30));
                            }


                            if (My.SeparateAxisRot)
                            {
                                using (new GUILayout.HorizontalScope())

                                {
                                    My.RotXCurve = EditorGUILayout.CurveField(My.RotXCurve, Color.red, new Rect());
                                    My.RotYCurve = EditorGUILayout.CurveField(My.RotYCurve, Color.green, new Rect());
                                    My.RotZCurve = EditorGUILayout.CurveField(My.RotZCurve, Color.blue, new Rect());
                                }

                            }
                        }
                    }

                    if (My.UseScale)
                    {
                        using (new GUILayout.HorizontalScope(EditorStyles.helpBox))

                        {
                            EditorGUILayout.LabelField("Scale", GUILayout.MinWidth(50), GUILayout.MaxWidth(100));
                            My.Scale = EditorGUILayout.Vector3Field("", My.Scale, GUILayout.MinWidth(120));
                            My.ScaleCurve = EditorGUILayout.CurveField(My.ScaleCurve, GUILayout.MinWidth(50));
                        }
                    }
                }

                if (cc.changed)
                {
                    Undo.RecordObject(target, "Transform Animation Inspector");
                    EditorUtility.SetDirty(target);
                }
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}