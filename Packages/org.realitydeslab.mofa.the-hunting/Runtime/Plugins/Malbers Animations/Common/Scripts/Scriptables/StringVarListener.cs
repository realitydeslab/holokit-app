using MalbersAnimations.Scriptables;
using MalbersAnimations.Events;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MalbersAnimations
{
    [AddComponentMenu("Malbers/Variables/String Listener")]
    [HelpURL("https://malbersanimations.gitbook.io/animal-controller/secondary-components/variable-listeners-and-comparers")]
    public class StringVarListener : VarListener
    {
        public StringReference value;
        public StringEvent Raise = new StringEvent();

        public virtual string Value
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

        public virtual void Invoke(string value)
        { if (Enable) Raise.Invoke(value);}

        public virtual void Invoke(Object value) => Invoke(value.name);

        public virtual void Invoke() => Invoke(Value);

#if UNITY_EDITOR
        [ContextMenu("Connect To Text")]
        private void ConnectToText()
        {
            var text = GetComponent<Text>();

            if (text)
            {
                var method = MTools.Property_Set_UnityAction<string>(text, "text");
                if (method != null) UnityEditor.Events.UnityEventTools.AddPersistentListener(Raise, method);
                MTools.SetDirty(this);
            }
        }
#endif
    }

#if UNITY_EDITOR
    public static class TextContextMenu
    {
        [MenuItem("CONTEXT/Text/Connect to String Listener", false, 611)]
        static void ConnectToString(MenuCommand command)
        {
            var test = command.context as Text;

            var stringVar = test.GetComponent<StringVarListener>();

            if (stringVar == null)
                stringVar = test.gameObject.AddComponent<StringVarListener>();
            stringVar.ShowEvents = true;
            var method = MTools.Property_Set_UnityAction<string>(test, "text");
            if (method != null) UnityEditor.Events.UnityEventTools.AddPersistentListener(stringVar.Raise, method); 
        }
    }

    //INSPECTOR
    [UnityEditor.CustomEditor(typeof(StringVarListener)), UnityEditor.CanEditMultipleObjects]
    public class StringVarListenerEditor : VarListenerEditor
    {
        private UnityEditor.SerializedProperty Raise;

        void OnEnable()
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