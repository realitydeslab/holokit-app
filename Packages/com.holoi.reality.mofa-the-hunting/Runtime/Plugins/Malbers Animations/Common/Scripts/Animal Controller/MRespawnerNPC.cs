using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using MalbersAnimations.Scriptables;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using MalbersAnimations.Events;

namespace MalbersAnimations.Controller.AI
{
    /// <summary>Use this Script's Transform as the Respawn Point</summary>
    [AddComponentMenu("Malbers/Animal Controller/Respawner NPC")]
    public class MRespawnerNPC : MonoBehaviour
    {
        #region Respawn
        [Tooltip("Animal Prefab to Swpawn")]
        public MAnimal NPC;
        public StateID RespawnState;
        public FloatReference RespawnTime = new FloatReference(10f);
        [Tooltip("If True: it will destroy the MainPlayer GameObject and Respawn a new One")]
        public BoolReference DestroyAfterRespawn = new BoolReference(true);


        /// <summary>Active Animal</summary>
        private MAnimal ActiveAnimal;
        
        #endregion

        [FormerlySerializedAs("OnRestartGame")]
        public GameObjectEvent OnRespawned = new GameObjectEvent();

        private bool Respawned;
        private MAnimalBrain NPCBrain;


        void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
        {
           FindNPCAnimal();
        }

       

        public virtual void DontDestroyOnLoad_GameObject(GameObject gameObject) => DontDestroyOnLoad(gameObject);

        void OnEnable()
        {
            if (!isActiveAndEnabled) return;  

            DontDestroyOnLoad(gameObject);
            gameObject.name = gameObject.name + " Instance";
            SceneManager.sceneLoaded += OnLevelFinishedLoading;
            FindNPCAnimal(); 
        }


        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnLevelFinishedLoading;

            if (ActiveAnimal != null)
                ActiveAnimal.OnStateChange.RemoveListener(OnCharacterDead);  //Listen to the Animal changes of states
        }


        void FindNPCAnimal()
        {
            if (Respawned) return; //meaning the animal was already respawned. 

            if (NPC != null)
            {
                if (NPC.gameObject.IsPrefab())
                {
                    ActiveAnimal = Instantiate(NPC);
                }
                else
                {
                    ActiveAnimal = NPC;
                }

                SceneAnimal();
            }
            else
            {
                Debug.LogWarning("[Respawner Removed]. There's no Character assigned", this);
                Destroy(gameObject); //Destroy This GO since is already a Spawner in the scene
            }
        }

        private void SceneAnimal()
        {

            ActiveAnimal.OverrideStartState = RespawnState;
            ActiveAnimal.ResetController();
            ActiveAnimal.enabled = true;
            ActiveAnimal.OnStateChange.AddListener(OnCharacterDead);        //Listen to the Animal changes of states
            ActiveAnimal.Teleport_Internal(transform.position);             //Move the Animal to is Start Position
            ActiveAnimal.transform.rotation = (transform.rotation);         //Move the Animal to is Start Position
            ActiveAnimal.isPlayer.Value = false;
            Respawned = true;

            NPCBrain = ActiveAnimal.GetComponentInChildren<MAnimalBrain>();
            if (NPCBrain != null)
                NPCBrain.enabled = true;
           // Debug.Log("Placed");
        }


        /// <summary>Listen to the Animal States</summary>
        public void OnCharacterDead(int StateID)
        {
            if (!Respawned) return;

            if (StateID == StateEnum.Death)              //Means Death
            {
                // Debug.Log("OnCharacterDead" + StateID);
                ActiveAnimal.OnStateChange.RemoveListener(OnCharacterDead);        //Remove listener from the Animal

                Respawned = false;

                if (NPC != null)         //If the Player is a Prefab then then instantiate it on the created scene
                {
                    if (NPC.gameObject.IsPrefab())
                    {
                        this.Delay_Action(RespawnTime, () =>
                         {
                             DestroyCurrentDeathAnimal();
                             this.Delay_Action(() => FindNPCAnimal());
                         }
                        );
                    }
                    else
                    {
                        var DeathS = ActiveAnimal.activeState as Death; //make sure the Death does not disable all things... since where reusing the same animal
                        DeathS.disableAnimal = false;
                        DeathS.DisableAllComponents = false;
                        DeathS.RemoveAllColliders = false;
                        DeathS.RemoveAllTriggers = false;

                        this.Delay_Action(RespawnTime, () => SceneAnimal());

                    }
                }
            }
        }

        void DestroyCurrentDeathAnimal()
        {
            if (ActiveAnimal != null)
            {
                if (DestroyAfterRespawn)
                    Destroy(ActiveAnimal.gameObject);
                else
                    DestroyAllComponents(ActiveAnimal);
            }
        }

        


        /// <summary>Destroy all the components on  Animal and leaves the mesh and bones</summary>
        private void DestroyAllComponents(MAnimal target)
        {
            if (!target) return;

            var components = target.GetComponentsInChildren<MonoBehaviour>();
            foreach (var comp in components) Destroy(comp);
            var colliders = target.GetComponentsInChildren<Collider>();
            if (colliders != null)
            {
                foreach (var col in colliders) Destroy(col);
            }
            var rb = target.GetComponentInChildren<Rigidbody>();
            if (rb != null) Destroy(rb);
            var anim = target.GetComponentInChildren<Animator>();
            if (anim != null) Destroy(anim);
        }
    }
}