using MalbersAnimations.Scriptables;
using MalbersAnimations.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace MalbersAnimations.Weapons
{
    [AddComponentMenu("Malbers/Damage/Projectile Thrower")]

    public class MProjectileThrower : MonoBehaviour, IThrower , IAnimatorListener
    {
        /// <summary> Is Used to calculate the Trajectory and Display it as a LineRenderer </summary>
        public System.Action<bool> Predict { get; set; }

        [Header("Projectile")]
        [SerializeField, Tooltip("What projectile will be instantiated")]
        private GameObjectReference m_Projectile = new GameObjectReference();
        [Tooltip("The projectile will be fired on start")]
        public BoolReference FireOnStart;

        [Header("Multipliers")]
        [Tooltip("Multiplier value to Apply to the Projectile Stat Modifier"),FormerlySerializedAs("Multiplier") ]
        public FloatReference DamageMultiplier = new FloatReference(1);
        [Tooltip("Multiplier value to apply to the Projectile Scale")]
        public FloatReference ScaleMultiplier = new FloatReference(1);
        [Tooltip("Multiplier value to apply to the Projectile Launch Force")]
        public FloatReference PowerMultiplier = new FloatReference(1);


        [Header("Layer Interaction")]
        [SerializeField] private LayerReference hitLayer = new LayerReference(-1);
        [SerializeField] private QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore;

        [Header("References")]
        [SerializeField, Tooltip("When this parameter is set it will Aim Directly to the Target")]
        private TransformReference m_Target;
        [SerializeField, Tooltip("Transform Reference for to calculate the Thrower Aim Origin Position")]
        private Transform m_AimOrigin;
        [SerializeField, Tooltip("Owner of the Thrower Component. By default it should be the Root GameObject")] 
        private GameObjectReference m_Owner;
        [Tooltip("Reference for the Aimer Component")]
        public Aim Aimer;

        [Tooltip("if its set to False. it will use this GameObject Forward Direction")]
        public BoolReference useAimerDirection = new BoolReference( true);

        [Header("Physics Values")]
        [SerializeField, Tooltip("Launch force for the Projectile")]
        private float m_power = 50f;
        
        [Range(0, 90)]
        [SerializeField, Tooltip("Angle of the Projectile when a Target is assigned")]
        private float m_angle = 45f;
        [SerializeField, Tooltip("Gravity to apply to the Projectile. By default is set to Physics.gravity")]
        private Vector3Reference gravity = new Vector3Reference(Physics.gravity);

        
        
        public Vector3 Gravity { get => gravity.Value; set => gravity.Value = value; }
        public LayerMask Layer { get => hitLayer.Value; set => hitLayer.Value = value; }
        public QueryTriggerInteraction TriggerInteraction { get => triggerInteraction; set => triggerInteraction = value; }
        public Vector3 AimOriginPos => m_AimOrigin.position;
        public Transform Target { get => m_Target.Value; set => m_Target.Value = value; }

        /// <summary> Owner of the  </summary>
        public GameObject Owner { get => m_Owner.Value; set => m_Owner.Value = value; }
        public GameObject Projectile { get => m_Projectile.Value; set => m_Projectile.Value = value; }

        /// <summary> Projectile Launch Velocity(v3) </summary>
        public Vector3 Velocity { get; set; }

        /// <summary>Force to Apply to the Projectile</summary>
        public float Power { get => m_power * PowerMultiplier; set => m_power = value; }

        /// <summary>Set if the Aimer Direction will be used or not</summary>
        public bool UseAimerDirection { get => useAimerDirection.Value; set => useAimerDirection.Value = value; }

        public Transform AimOrigin => m_AimOrigin;

        public bool CalculateTrajectory
        {
            get => m_CalculateTrajectory;
            set
            {
                m_CalculateTrajectory = value;
                Predict?.Invoke(value);
            }
        }
        private bool m_CalculateTrajectory;

        private void OnEnable()
        {
            if (Owner == null) Owner = transform.root.gameObject;
            if (m_AimOrigin == null) m_AimOrigin = transform;
            if (Aimer) m_AimOrigin = Aimer.AimOrigin; //Set the Aim origin from the Aimer

            if (FireOnStart) Fire();
        }


        public virtual void SetProjectile(GameObject newProjectile)
        {
            if (Projectile != newProjectile) Projectile = newProjectile;
        }

        /// <summary> Fire Proyectile </summary>
        public virtual void Fire()
        {
            if (!enabled) return;
            if (Projectile == null) return;

            CalculateVelocity();
            var Inst_Projectile = Instantiate(Projectile, AimOriginPos, Quaternion.identity);

            Inst_Projectile.transform.localScale *= ScaleMultiplier;//

            Prepare_Projectile(Inst_Projectile);

            Predict?.Invoke(false);
        }


        private void Update()
        {
            if (CalculateTrajectory) CalculateVelocity();
        }

        public void SetTarget(Transform target) => Target = target;
        public void ClearTarget() => Target = null;
        public void SetTarget(GameObject target) => Target = target.transform;

        public virtual bool OnAnimatorBehaviourMessage(string message, object value) => this.InvokeWithParams(message, value);

        void Prepare_Projectile(GameObject p)
        {
            var projectile = p.GetComponent<IProjectile>();

            if (projectile != null) //Means its a Malbers Projectile ^^
            {
                projectile.Prepare(Owner, Gravity, Velocity,  Layer, TriggerInteraction);
                projectile.DamageMultiplier(DamageMultiplier); //Apply Multiplier
                projectile.Fire();
            }
            else //Fire without the Projectile Component
            {
                var rb = p.GetComponent<Rigidbody>();
                rb?.AddForce(Velocity, ForceMode.VelocityChange);
            }
        }

        public virtual void SetDamageMultiplier(float m) => DamageMultiplier = m;
        public virtual void SetScaleMultiplier(float m) => ScaleMultiplier = m;
        public virtual void SetPowerMultiplier(float m) => PowerMultiplier = m;

        public virtual void CalculateVelocity()
        {
            if (Target)
            {
                var target_Direction = (Target.position - AimOriginPos).normalized;

                float TargetAngle = 90 - Vector3.Angle(target_Direction, -Gravity) + 0.1f; //in case the angle is smaller than the target height

                if (TargetAngle < m_angle)
                    TargetAngle = m_angle;

                Power = MTools.PowerFromAngle(AimOriginPos, Target.position, TargetAngle);
                Velocity = MTools.VelocityFromPower(AimOriginPos, Power, TargetAngle, Target.position);
            }
            else 
            if (Aimer && useAimerDirection.Value)
            {
                Velocity = Aimer.AimDirection.normalized * Power;
            }
            else
            {
                Velocity = transform.forward.normalized * Power;
            }

            Predict?.Invoke(true);
        }

        void Reset()
        {
            m_Owner.Value = transform.root.gameObject;
            m_AimOrigin = transform;
        }


#if UNITY_EDITOR

        void OnDrawGizmos()
        {
            if (AimOrigin)
            {
                UnityEditor.Handles.color = Color.blue;
                UnityEditor.Handles.ArrowHandleCap(0, m_AimOrigin.position, transform.rotation, Power * 0.01f, EventType.Repaint);
            }
        }
#endif
    }
}