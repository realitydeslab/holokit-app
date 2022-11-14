using UnityEngine; 

namespace MalbersAnimations.Scriptables
{
    ///<summary> Store a list of Materials</summary>
    [CreateAssetMenu(menuName = "Malbers Animations/Collections/Audio Clip Set", order = 1000)]
    public class AudioClipListVar : ScriptableList<AudioClip>
    {
        public void Play(AudioSource source)
        {
            var clip = Item_GetRandom();
            source.clip = clip;
            source.Play();
        }


        public void Play()
        {
            var NewGO = new GameObject() { name = "Audio [" + this.name +"]"};
            var source = NewGO.AddComponent<AudioSource>();
            source.spatialBlend = 1f;
            var clip = Item_GetRandom();
            source.clip = clip;
            source.Play();
        }

        void Reset() => Description = "Store a Collection of AudioClip";
    }

    [System.Serializable]
    public class AudioClipReference : ReferenceVar
    {
        public AudioClip ConstantValue;
        [RequiredField] public AudioClipListVar Variable;

        public bool NullOrEmpty() => UseConstant ? (ConstantValue == null) : (Variable == null);
        public AudioClip GetValue() => UseConstant ? ConstantValue : (Variable != null ? Variable.Item_GetRandom() : null);

        public AudioClip GetValue(int index) => UseConstant ? ConstantValue : Variable.Item_Get(index);

        public AudioClip GetValue(string name) => UseConstant ? ConstantValue : Variable.Item_Get(name);

        public void Play(AudioSource source)
        {
            var clip = GetValue();
            source.clip = clip;
            source.Play();
        }
    }
}