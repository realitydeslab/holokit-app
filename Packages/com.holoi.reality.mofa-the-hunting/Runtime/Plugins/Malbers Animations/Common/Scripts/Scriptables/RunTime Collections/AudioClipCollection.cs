using UnityEngine; 

namespace MalbersAnimations.Scriptables
{
    ///<summary> Store a list of Materials</summary>
    [CreateAssetMenu(menuName = "Malbers Animations/Collections/Audio Clip Set", order = 1000)]
    public class AudioClipCollection : RuntimeCollection<AudioClip> {}
}