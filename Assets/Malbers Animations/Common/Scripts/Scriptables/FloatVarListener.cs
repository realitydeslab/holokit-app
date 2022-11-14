using MalbersAnimations.Scriptables;
using MalbersAnimations.Events;
using UnityEngine;
using System.Collections;

namespace MalbersAnimations
{
    [AddComponentMenu("Malbers/Variables/Float Listener")]
    [HelpURL("https://malbersanimations.gitbook.io/animal-controller/secondary-components/variable-listeners-and-comparers")]
    public class FloatVarListener : VarListener
    {
        public FloatReference value;
        public FloatEvent Raise = new FloatEvent();

        public virtual float Value
        {
            get => value;
            set
            {
                this.value.Value = value;
                if (Auto) Invoke(value);
            }
        }

        void OnEnable()
        {
            if (value.Variable != null && Auto) value.Variable.OnValueChanged += Invoke;
            if (InvokeOnEnable) Raise.Invoke(value);
        }

        void OnDisable()
        {
            if (value.Variable != null && Auto) value.Variable.OnValueChanged -= Invoke;
        }

        public virtual void Invoke(float value)
        { if (Enable) Raise.Invoke(value); }

        public virtual void InvokeFloat(float value) => Invoke(value);

        public virtual void Invoke() => Invoke(Value);


        /// <summary> Set the Value to Zero in x Seconds </summary>
        public virtual void Time_ValueToZero(float time)
        {
            if (Value == 0) return;

            StopAllCoroutines();

            StartCoroutine(I_FloatInTime(Value,0,time));
        }


        /// <summary> Set the Value to Zero in x Seconds </summary>
        public virtual void Time_ZeroToValue(float time)
        {
            if (Value == 0) return;

            StopAllCoroutines();

            StartCoroutine(I_FloatInTime(0,Value, time));
        }


        /// <summary> Set the Value to Zero in x Seconds </summary>
        public virtual void Time_ValueToZero_FixedUpdate(float time)
        {
            if (Value == 0) return;

            StopAllCoroutines();

            StartCoroutine(I_FloatInTime_FixedUpdate(Value, 0, time));
        }


        /// <summary> Set the Value to Zero in x Seconds </summary>
        public virtual void Time_ZeroToValue_FixedUpdate(float time)
        {
            if (Value == 0) return;

            StopAllCoroutines();

            StartCoroutine(I_FloatInTime_FixedUpdate(0, Value, time));
        }


        IEnumerator I_FloatInTime(float start,float end,float time)
        {
           
            float currentTime = 0;

            while (currentTime <= time)
            {
                Value = Mathf.Lerp(start,end, currentTime / time);

                Debug.Log("Value = " + Value);


                currentTime += Time.deltaTime;

                yield return null;
            }

            Value = end;
            yield return null;
        }


        IEnumerator I_FloatInTime_FixedUpdate(float start, float end, float time)
        {
            var wait = new WaitForFixedUpdate();

            float currentTime = 0;

            while (currentTime <= time)
            {
                Value = Mathf.Lerp(start, end, currentTime / time);
                currentTime += Time.fixedDeltaTime;

                yield return wait;
            }

            Value = end;
            yield return null;
        }
    }



    //INSPECTOR
#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(FloatVarListener)), UnityEditor.CanEditMultipleObjects]
    public class FloatVarListenerEditor : VarListenerEditor
    {
        private UnityEditor.SerializedProperty  Raise;

        private void OnEnable()
        {
            base.SetEnable();
            Raise = serializedObject.FindProperty("Raise");
        }

        protected override void DrawEvents()
        {
            UnityEditor.EditorGUILayout.PropertyField(Raise);
            base.DrawEvents();
        }
    }
#endif
}