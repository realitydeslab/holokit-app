using MalbersAnimations.Utilities;
using UnityEngine;

namespace MalbersAnimations.Controller
{
    [HelpURL("https://docs.google.com/document/d/1QBLQVWcDSyyWBDrrcS2PthhsToWkOayU0HUtiiSWTF8/edit#heading=h.kraxblx9518t")]
    public class Death : State
    {
        public override string StateName => "Death";

        [Header("Death Parameters")]

        public bool DisableAllComponents = true;
        public bool RemoveAllColliders = false;
        public bool RemoveAllTriggers = true;
        public int DelayFrames = 2;
        public float RigidbodyDrag = 5;
        public float RigidbodyAngularDrag = 15;

        [Space]
        public bool disableAnimal = true;
        
        [Hide("disableAnimal")] 
        public float disableAnimalTime = 5f;



        public override void Activate()
        {
            animal.Mode_Stop(); //Interrupt all modes.
            base.Activate();
        }

        public override void EnterCoreAnimation()
        {
            animal.Mode_Interrupt();
            //animal.Mode_Disable_All();
            animal.StopMoving();
            animal.Mode_Stop();
            animal.Delay_Action(DelayFrames, ()=> DisableAll()); //Wait 2 frames
        }

        void DisableAll()
        {
            SetEnterStatus(0);

            if (DisableAllComponents)
            {
                var AllComponents = animal.GetComponentsInChildren<MonoBehaviour>();
                foreach (var comp in AllComponents)
                {
                    if (comp == animal) continue;
                    if (comp != null) comp.enabled = false;
                }
            }

            var AllTriggers = animal.GetComponentsInChildren<Collider>();

            foreach (var trig in AllTriggers)
            {
                if (RemoveAllColliders || (RemoveAllTriggers && trig.isTrigger))
                {
                    Destroy(trig);
                }
            }


            animal.SetCustomSpeed(new MSpeed("Death"));

            if (animal.RB)
            {
                animal.RB.drag = RigidbodyDrag;
                animal.RB.angularDrag = RigidbodyAngularDrag;
            }

            if (disableAnimal)
                animal.DisableSelf(disableAnimalTime); //Disable the Animal Component after x time
        }


#if UNITY_EDITOR

        public override void SetSpeedSets(MAnimal animal)
        {
            //Do nothing... Death does not need a Speed Set
        }

        void Reset()
        {
           
            ID = MTools.GetInstance<StateID>("Death");

            General = new AnimalModifier()
            {
                modify = (modifier)(-1),
                Persistent = true,
                LockInput = true,
                LockMovement = true,
                AdditiveRotation = true,
            };
        }
#endif
    }
}