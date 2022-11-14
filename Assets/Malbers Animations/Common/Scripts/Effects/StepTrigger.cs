using UnityEngine;
using System.Collections;
using MalbersAnimations.Utilities;

namespace MalbersAnimations
{
    /// <summary>Works with the Step manager ... get the terrain below the animal </summary>
    [AddComponentMenu("Malbers/Utilities/Effects - Audio/Step Trigger")]
    public class StepTrigger : MonoBehaviour
    {
        [RequiredField] 
        public StepsManager m_StepsManager;
       
        public float WaitNextStep = 0.2f;
        public AudioSource StepAudio;

        public SphereCollider m_Trigger;

        public Color DebugColor = Color.cyan;

        private LayerMask GroundLayer => m_StepsManager.GroundLayer.Value;

        WaitForSeconds wait;
        bool waitrack;                      // Check if is time to put a track; 

        void Awake()
        {
            if (m_StepsManager == null) m_StepsManager = transform.root.FindComponent<StepsManager>();

          if (m_Trigger == null)  m_Trigger = GetComponent<SphereCollider>();

            if (m_StepsManager == null) //If there's no  StepManager Remove the Stepss
            {
                Destroy(gameObject);
                return;
            }

            m_Trigger.isTrigger = true;

            if (m_StepsManager.Active == false) //If there's no  StepManager Remove the Stepss
            {
                gameObject.SetActive(false);
                return;
            }

            SetAudio();

            wait = new WaitForSeconds(WaitNextStep);
        }

        private void SetAudio()
        {
            if (StepAudio == null)
                StepAudio = GetComponent<AudioSource>();
            if (StepAudio == null)
                StepAudio = gameObject.AddComponent<AudioSource>();

            StepAudio.spatialBlend = 1;  //Make the Sound 3D
            if (m_StepsManager) StepAudio.volume = m_StepsManager.StepsVolume;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.isTrigger) return;
            if (!MTools.CollidersLayer(other, GroundLayer)) return; //Ignore layers that are not Ground

            if (!waitrack)
            {
                waitrack = true;
                this.Delay_Action(wait, () => waitrack = false);
                m_StepsManager.EnterStep(this, other);
            }
        }


        void OnDrawGizmos()
        {
            GizmoSelected(false);
        }

        void OnDrawGizmosSelected()
        {
            GizmoSelected(true);
        }


        [ContextMenu("Find Sphere Trigger")]
        void GetTrigger()
        {
            m_Trigger = GetComponent<SphereCollider>();
            MTools.SetDirty(this);
        }

        void GizmoSelected(bool sel)
        {
            if (m_Trigger && m_Trigger.enabled)
            {
                var DebugColorWire = new Color(DebugColor.r, DebugColor.g, DebugColor.b, 1);
                Gizmos.matrix = transform.localToWorldMatrix;

                    Gizmos.color = DebugColor;
                Gizmos.DrawSphere(Vector3.zero + m_Trigger.center, m_Trigger.radius);
                Gizmos.color = sel? Color.yellow : DebugColorWire;
                Gizmos.DrawWireSphere(Vector3.zero + m_Trigger.center, m_Trigger.radius);
            }
        }


        private void OnValidate()
        {
            if (m_Trigger == null) m_Trigger = GetComponent<SphereCollider>();
        }

        [ContextMenu("Find Audio Source")]
        private void FindAudioSource()
        {
            StepAudio = GetComponent<AudioSource>();
            if (StepAudio)
            {
                StepAudio.spatialBlend = 1;  //Make the Sound 3D
                if (m_StepsManager) StepAudio.volume = m_StepsManager.StepsVolume;
                StepAudio.maxDistance = 5;
                StepAudio.minDistance = 1;
                StepAudio.playOnAwake = false;
            }
            MTools.SetDirty(StepAudio);
        }
    }
}