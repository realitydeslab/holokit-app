using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using MalbersAnimations.Scriptables;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using MalbersAnimations.Events;

namespace MalbersAnimations.Controller
{
    /// <summary>Use this Script's Transform as the Respawn Point</summary>
    [AddComponentMenu("Malbers/Animal Controller/Respawner")]
    public class MRespawner : MonoBehaviour
    {
        public static MRespawner instance;

        #region Respawn
        [Tooltip("Animal Prefab to Swpawn"), FormerlySerializedAs("playerPrefab")]
        public GameObject player;

        //[ContextMenuItem("Set Default", "SetDefaultRespawnPoint")]
        //public Vector3Reference RespawnPoint;
        public StateID RespawnState;
        public FloatReference RespawnTime = new FloatReference(4f);
        [Tooltip("If True: it will destroy the MainPlayer GameObject and Respawn a new One")]
        public BoolReference DestroyAfterRespawn = new BoolReference(true);


        /// <summary>Active Player Animal GameObject</summary>
        private GameObject InstantiatedPlayer;
        /// <summary>Active Player Animal</summary>
        private MAnimal activeAnimal;
        /// <summary>Old Player Animal GameObject</summary>
        private GameObject oldPlayer;
        #endregion

        [FormerlySerializedAs("OnRestartGame")]
        public GameObjectEvent OnRespawned = new GameObjectEvent();

        private bool Respawned;


        void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
        { 
            FindMainAnimal();
        }

        public virtual void SetPlayer(GameObject go) => player = go;

        public virtual void DontDestroyOnLoad_GameObject(GameObject gameObject) => DontDestroyOnLoad(gameObject);

        void OnEnable()
        {
            if (!isActiveAndEnabled) return;

            if (instance == null)
            {
                instance = this;
                transform.parent = null;
                DontDestroyOnLoad(gameObject);
                gameObject.name = gameObject.name + " Instance";
                SceneManager.sceneLoaded += OnLevelFinishedLoading;
                FindMainAnimal();
            }
            else
            {
                Destroy(gameObject); //Destroy This GO since is already a Spawner in the scene
            }
        }


        private void OnDisable()
        {
            if (instance == this)
            {
                SceneManager.sceneLoaded -= OnLevelFinishedLoading;
                
                if (activeAnimal != null)
                    activeAnimal.OnStateChange.RemoveListener(OnCharacterDead);  //Listen to the Animal changes of states
            }
        }

        public void ResetScene()
        {
            var scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
            Respawned = false;
        }

        /// <summary>Finds the Main Animal used as Player on the Active Scene</summary>
        void FindMainAnimal()
        {
            if (Respawned) return; //meaning the animal was already respawned.
             
            if (player == null)
            {
                activeAnimal = MAnimal.MainAnimal;
                if (activeAnimal) player = activeAnimal.gameObject;
            } 

            if (player != null)
            {
                if (player.IsPrefab())
                {
                    InstantiateNewPlayer();
                }
                else
                {
                    activeAnimal = player.GetComponent<MAnimal>();

                    if (activeAnimal)
                    {
                        SceneAnimal();
                    }
                }
            }
            else
            {
                Debug.LogWarning("[Respawner Removed]. There's no Character assigned", this);
                Destroy(gameObject); //Destroy This GO since is already a Spawner in the scene
            }
        }

        private void SceneAnimal()
        {
            activeAnimal.OnStateChange.AddListener(OnCharacterDead);        //Listen to the Animal changes of states
            activeAnimal.Teleport_Internal(transform.position);             //Move the Animal to is Start Position
            activeAnimal.transform.rotation = (transform.rotation);         //Move the Animal to is Start Position
            activeAnimal.OverrideStartState = RespawnState;
            activeAnimal.SetMainPlayer();
            Respawned = true;
        }

        /// <summary>Listen to the Animal States</summary>
        public void OnCharacterDead(int StateID)
        {
            if (!Respawned) return;

            if (StateID == StateEnum.Death)                      //Means Death
            {
                oldPlayer = InstantiatedPlayer;                  //Store the old player IMPORTANT

                activeAnimal.OnStateChange.RemoveListener(OnCharacterDead);        //Remove listener from the Animal

                if (player != null && player.IsPrefab())         //If the Player is a Prefab then then instantiate it on the created scene
                {
                    this.Delay_Action(RespawnTime, () =>
                     {
                         DestroyDeathPlayer();
                         this.Delay_Action(() => InstantiateNewPlayer());
                     }
                    );
                }
                else
                {
                   this.Delay_Action(RespawnTime, () => ResetScene());
                }
            }
        }

        void DestroyDeathPlayer()
        {
            if (oldPlayer != null)
            {
                if (DestroyAfterRespawn)
                    Destroy(oldPlayer);
                else
                    DestroyAllComponents(oldPlayer);
            }
        }

        void InstantiateNewPlayer()
        {
           // Debug.Log("InstantiateNewPlayer");
            InstantiatedPlayer = Instantiate(player, transform.position, transform.rotation);
            activeAnimal = InstantiatedPlayer.GetComponent<MAnimal>();
            activeAnimal.OverrideStartState = RespawnState;
            activeAnimal.OnStateChange.AddListener(OnCharacterDead);
            OnRespawned.Invoke(InstantiatedPlayer);
            activeAnimal.SetMainPlayer();
            Respawned = true;
        }


        /// <summary>Destroy all the components on  Animal and leaves the mesh and bones</summary>
        private void DestroyAllComponents(GameObject target)
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