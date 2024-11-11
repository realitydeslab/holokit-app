using MalbersAnimations.Scriptables;
using UnityEngine;

namespace MalbersAnimations.Controller.AI
{
    [CreateAssetMenu(menuName = "Malbers Animations/Pluggable AI/Tasks/Play Audio")]
    public class PlayAudioTask : MTask
    {
        public override string DisplayName => "General/Play Audio";

        [Space]
        public AudioClipReference Clips;
        public string AudioSource = "BrainAudio";
       

        public override void StartTask(MAnimalBrain brain, int index)
        {
            var findAudio = brain.transform.FindGrandChild(AudioSource);

            if (!findAudio)
            {
                findAudio = new GameObject(name: AudioSource).transform;
                findAudio.parent = brain.transform;
            }

            var sourc = findAudio.GetComponent<AudioSource>();
            if (sourc == null) sourc = findAudio.gameObject.AddComponent<AudioSource>();

            brain.TasksVars[index].AddComponent(sourc); //Save the audio source to the task variables

            Clips.Play(sourc);

            brain.TaskDone(index);
        }

        //public override void UpdateTask(MAnimalBrain brain, int index)
        //{
            
        //}
    }
}
