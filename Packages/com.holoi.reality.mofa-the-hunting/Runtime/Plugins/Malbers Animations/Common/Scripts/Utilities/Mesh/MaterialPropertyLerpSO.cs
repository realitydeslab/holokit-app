using UnityEngine;
using System.Collections;
using MalbersAnimations.Scriptables;
using System.Collections.Generic;

namespace MalbersAnimations.Utilities
{
    [CreateAssetMenu(menuName = "Malbers Animations/Extras/Material Property Lerp", order = 2100)]
    public class MaterialPropertyLerpSO : ScriptableCoroutine
    {
        [Tooltip("Index of the Material")]
        public IntReference materialIndex = new IntReference();
        public FloatReference time = new FloatReference(1f);
        public AnimationCurve curve = new AnimationCurve(MTools.DefaultCurve);


        public StringReference propertyName;
        public MaterialPropertyType propertyType = MaterialPropertyType.Float;
       
        public FloatReference FloatValue = new FloatReference(1f);
        public Color ColorValue = Color.white;
        [ColorUsage(true, true)]
        public Color ColorHDRValue = Color.white;
        public FloatReference StartMultiplier = new FloatReference(1f);


        public void LerpMaterial(Component go) => LerpMaterial(go.gameObject);
        public void LerpMaterial(GameObject go)
        {
            var all = go.transform.root.GetComponentsInChildren<SkinnedMeshRenderer>();
            var all2 = go.transform.root.GetComponentsInChildren<MeshRenderer>();

            foreach (var item in all) LerpMaterial(item);
            foreach (var item in all2) LerpMaterial(item);
        }

        internal override void Evaluate(MonoBehaviour mono, Transform target, float time, AnimationCurve curve = null)
        {
            var t = target.GetComponent<MeshRenderer>(); 

            var curv = curve != null ? curve : this.curve;

            switch (propertyType)
            {
                case MaterialPropertyType.Float:
                    mono.StartCoroutine(LerperFloat(t,time, curv));
                    break;
                case MaterialPropertyType.Color:
                    mono.StartCoroutine(LerperColor(t, ColorValue, time, curv));
                    break;
                case MaterialPropertyType.HDRColor:
                    mono.StartCoroutine(LerperColor(t, ColorHDRValue, time, curv));
                    break;
                default:
                    break;

            }
        }

        [System.Obsolete("Lerp is Obsolete, use LerpMaterial(Renderer) instead")]
        public virtual void Lerp(Renderer mesh) => LerpMaterial(mesh);

        public virtual void LerpMaterial(Renderer mesh)
        {
            if (mesh)
            {
                IEnumerator ICoroutine = null;
                switch (propertyType)
                {
                    case MaterialPropertyType.Float:
                        ICoroutine = LerperFloat(mesh, time, curve);
                        break;
                    case MaterialPropertyType.Color:
                        ICoroutine = LerperColor(mesh, ColorValue, time, curve);
                        break;
                    case MaterialPropertyType.HDRColor:
                        ICoroutine = LerperColor(mesh, ColorHDRValue, time, curve);
                        break;
                    default:
                        break;

                }
                StartCoroutine(mesh, ICoroutine);
            }
        }

        IEnumerator LerperFloat(Renderer mesh, float time, AnimationCurve curve)
        {
            float elapsedTime = 0;
            var mat =   mesh.materials[materialIndex];


            while (elapsedTime <= time)
            {
                float value = curve.Evaluate(elapsedTime / time);
                mat.SetFloat(propertyName, value * FloatValue);
                Debug.Log("value = " + value);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            mat.SetFloat(propertyName, curve.Evaluate(curve.keys[curve.keys.Length - 1].time));

            yield return null;
            Stop(mesh);
        }


        IEnumerator LerperColor(Renderer mesh, Color FinalColor,  float time, AnimationCurve curve)
        {
            float elapsedTime = 0;

            var mat = mesh.materials[materialIndex];

            Color StartingColor = mat.GetColor(propertyName)* StartMultiplier;
            Color ElapsedColor;// = Color.LerpUnclamped(StartingColor, FinalColor, curve.Evaluate(curve.keys[curve.keys.Length - 1].time));

            if (time > 0)
            {
                while (elapsedTime <= time)
                {
                    float value = curve.Evaluate(elapsedTime / time);

                    ElapsedColor = Color.LerpUnclamped(StartingColor, FinalColor, value);

                    mat.SetColor(propertyName, ElapsedColor);

                    elapsedTime += Time.deltaTime;

                    yield return null;
                }
            }

            ElapsedColor = Color.LerpUnclamped(StartingColor, FinalColor, curve.Evaluate(1));

            mat.SetColor(propertyName, ElapsedColor);

            yield return null;

            Stop(mesh);
        }


    }

    [System.Serializable]
    public class MaterialPropertyInternal
    {
        public string propertyName;
        public MaterialPropertyType propertyType = MaterialPropertyType.Float;
        public float FloatValue = 1f;
        public Color ColorValue = Color.white;
        [ColorUsage(true, true)]
        public Color ColorHDRValue = Color.white;

        [HideInInspector] public bool isFloat; 
        [HideInInspector] public bool isColor; 
        [HideInInspector] public bool isColorHDR; 
    }

    public enum MaterialPropertyType
    {
        Float,
        Color,
        HDRColor
    }

    //Inspector

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(MaterialPropertyLerpSO)),UnityEditor.CanEditMultipleObjects]
    public class MaterialPropertyLerpSOEditor : UnityEditor.Editor
    {
        UnityEditor.SerializedProperty propertyName, materialIndex, propertyType, time, FloatValue, ColorValue, ColorHDRValue, ColorMultiplier, curve;//, UseMaterialPropertyBlock, shared;

        private void OnEnable()
        {
            propertyName = serializedObject.FindProperty("propertyName");
            materialIndex = serializedObject.FindProperty("materialIndex");
            propertyType = serializedObject.FindProperty("propertyType");
            time = serializedObject.FindProperty("time");
            FloatValue = serializedObject.FindProperty("FloatValue");
            ColorValue = serializedObject.FindProperty("ColorValue");
            ColorHDRValue = serializedObject.FindProperty("ColorHDRValue");
            ColorMultiplier = serializedObject.FindProperty("StartMultiplier");
            curve = serializedObject.FindProperty("curve");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            UnityEditor.EditorGUILayout.PropertyField(propertyName);
            UnityEditor.EditorGUILayout.PropertyField(materialIndex);
            UnityEditor.EditorGUILayout.PropertyField(time);

            UnityEditor.EditorGUILayout.PropertyField(propertyType);

            var pType = (MaterialPropertyType)propertyType.intValue;

            switch (pType)
            {
                case MaterialPropertyType.Float:
                    UnityEditor.EditorGUILayout.PropertyField(FloatValue);
                    break;
                case MaterialPropertyType.Color:
                    UnityEditor.EditorGUILayout.PropertyField(ColorValue);
                    UnityEditor.EditorGUILayout.PropertyField(ColorMultiplier);
                    break;
                case MaterialPropertyType.HDRColor:
                    UnityEditor.EditorGUILayout.PropertyField(ColorHDRValue);
                    UnityEditor.EditorGUILayout.PropertyField(ColorMultiplier);
                    break;
                default:
                    break;
            }


            UnityEditor.EditorGUILayout.PropertyField(curve);
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif

  
}

