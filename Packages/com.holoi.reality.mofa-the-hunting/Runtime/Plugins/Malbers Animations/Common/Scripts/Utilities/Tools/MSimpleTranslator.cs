using MalbersAnimations.Scriptables;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MalbersAnimations.Utilities
{
    /// <summary>
    /// Based on 3DKit Controller from Unity
    /// </summary>
    [AddComponentMenu("Malbers/Utilities/Transform/Simple Translator")]
    [SelectionBase]
    public class MSimpleTranslator : MSimpleTransformer
    {
        [ContextMenuItem("Invert",nameof(InvertStartEnd))]
        public Vector3Reference start;
        [ContextMenuItem("Invert",nameof(InvertStartEnd))]
        public Vector3Reference end = new Vector3Reference(Vector3.forward);

        public override void Evaluate(float curveValue)
        {
            var curvePosition = m_Curve.Evaluate(curveValue);
            var pos = transform.TransformPoint(Vector3.Lerp(start, end, curvePosition));

            Object.position = pos;
        }

        private void InvertStartEnd()
        {
            Vector3 d = start;

            start.Value = end.Value;
            end.Value = d;
            MTools.SetDirty(this);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(MSimpleTranslator), true)]
    public class SimpleTranslatorEditor : MSimpleTransformerEditor
    {
        void OnSceneGUI()
        {
            var t = target as MSimpleTranslator;
            var start = t.transform.TransformPoint(t.start.Value);
            var end = t.transform.TransformPoint(t.end.Value);


            using (var cc = new EditorGUI.ChangeCheckScope())
            {
                start = Handles.PositionHandle(start, Quaternion.AngleAxis(180, t.transform.up) * t.transform.rotation);
                Handles.color = Color.yellow;
                Handles.SphereHandleCap(0, start, t.transform.rotation, 0.1f * t.transform.lossyScale.y, EventType.Repaint);
                Handles.SphereHandleCap(0, end, t.transform.rotation, 0.1f * t.transform.lossyScale.y, EventType.Repaint);

                end = Handles.PositionHandle(end, Quaternion.AngleAxis(180, t.transform.up) * t.transform.rotation);
               
                if (cc.changed)
                {
                    Undo.RecordObject(t, "Move Handles");
                    t.start.Value = t.transform.InverseTransformPoint(start);
                    t.end.Value = t.transform.InverseTransformPoint(end);
                    t.Evaluate(t.previewPosition);
                }
            }
            Handles.DrawDottedLine(start, end, 5);
            Handles.Label(Vector3.Lerp(start, end, 0.5f), "Distance:" + (end - start).magnitude.ToString("F2"));
        }
    }
#endif
}