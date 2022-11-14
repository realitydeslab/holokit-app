using MalbersAnimations.Scriptables;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using MalbersAnimations.Controller;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MalbersAnimations.Weapons
{
    public enum ProjectileRotation { None, FollowTrajectory, Random, Axis };
    public enum ImpactBehaviour { None, StickOnSurface, DestroyOnImpact, ActivateRigidBody};

    [AddComponentMenu("Malbers/Damage/Projectile")]
    [SelectionBase]
    [HelpURL("https://malbersanimations.gitbook.io/animal-controller/secondary-components/mdamager/mprojectile")]
    public class MProjectile : MDamager, IProjectile
    {
        public ImpactBehaviour impactBehaviour = ImpactBehaviour.None;
        public ProjectileRotation rotation = ProjectileRotation.None;

        public float Penetration = 0.1f;

        [SerializeField, Tooltip("Keep Projectile Damage Values, The throwable wont affect the Damage Values")]
        private BoolReference m_KeepDamageValues = new BoolReference(false);

        [SerializeField, Tooltip("Gravity applied to the projectile, if gravity is zero the projectile will go straight. If the Projectile is thrown by a Projectile Thrower." +
            "It will inherit the gravity from it")]
        private Vector3Reference gravity = new Vector3Reference(Physics.gravity);

        [Tooltip("Life of the Projectile on the air, if it has not touch anything on this time it will destroy it self")]
        public FloatReference Life = new FloatReference(10f);
        [Tooltip("Life of the Projectile After Impact. If the projectile is not destroyed on impact, then wait this time to do it. (0 -> Ignores it) ")]
        public FloatReference LifeImpact = new FloatReference(0f);

        [Tooltip("Multiplier of the Force to Apply to the object the projectile impact ")]
        public FloatReference PushMultiplier = new FloatReference(1);

        [Tooltip("Torque for the rotation of the projectile")]
        public FloatReference torque = new FloatReference(50f);
        [Tooltip("Axis Torque for the rotation of the projectile")]
        public Vector3 torqueAxis = Vector3.up;

        [Tooltip("Offset to position the projectile. E.g. (Arrow in the Weapon) ")]
        public Vector3 m_PosOffset;

        [Tooltip("Offset to position the projectile. E.g. (Arrow in the Weapon) ")]
        public Vector3 m_RotOffset;

        [Tooltip("Offset to position the projectile. E.g. (Arrow in the Weapon) ")]
        public Vector3 m_ScaleOffset;

        public UnityEvent OnFire = new UnityEvent();                       //Send the transform to the event
        public Rigidbody rb;
        public Collider m_collider;

        private Vector3 Prev_pos;

        #region Properties
        /// <summary>Initial Velocity (Direction * Power) </summary>
        public Vector3 Velocity { get; set; }

        /// <summary>Has the projectile impacted with something</summary>
        public bool HasImpacted { get; set; }

        /// <summary>Do Fly Raycast</summary>
        private bool doRayCast;

        /// <summary>Is the Projectile Flying</summary>
        public bool IsFlying { get; set; }

        public Vector3 Gravity { get => gravity.Value; set => gravity.Value = value; }
        public bool KeepDamageValues { get => m_KeepDamageValues.Value; set => m_KeepDamageValues.Value = value; }
        public Vector3 TargetHitPosition { get; set; }
        public bool FollowTrajectory => rotation == ProjectileRotation.FollowTrajectory;
        public bool DestroyOnImpact => impactBehaviour == ImpactBehaviour.DestroyOnImpact;
        public bool StickOnSurface => impactBehaviour == ImpactBehaviour.StickOnSurface;

        public Vector3 PosOffset { get => m_PosOffset; set => m_PosOffset = value; }
        public Vector3 RotOffset { get => m_RotOffset; set => m_RotOffset = value; }
        // public Vector3 ScaleOffset { get => m_ScaleOffset; set => m_ScaleOffset = value; }
        #endregion


        [HideInInspector] public int Editor_Tabs1;


        private void Awake()
        {
           if (!rb) rb = GetComponent<Rigidbody>();
           if(!m_collider) m_collider = GetComponentInChildren<Collider>();  
        }



        /// <summary> Initialize the Projectile main references and parameters</summary>
        private void Initialize()
        {
            HasImpacted = false;
            Invoke(nameof(DestroyProjectile), Life); //Destroy Projectile after a time
        }


        /// <summary> Prepare the Projectile for firing </summary>
        public virtual void Prepare(GameObject Owner, Vector3 Gravity, Vector3 ProjectileVelocity, LayerMask HitLayer, QueryTriggerInteraction triggerInteraction)
        {
            this.Layer = HitLayer;
            this.TriggerInteraction = triggerInteraction;
            this.Owner = Owner;
            this.Gravity = Gravity;
            this.Velocity = ProjectileVelocity;
            this.Force = Velocity.magnitude;


            Debugging("Projectile Prepared",this);
        }
        
        public virtual void Fire(Vector3 ProjectileVelocity)
        {
            this.Velocity = ProjectileVelocity;
            this.Force = Velocity.magnitude;
            Fire();
        }

        public virtual void Fire()
        {
            Initialize();

            gameObject.SetActive(true); //Just to make sure is working

            if (Velocity == Vector3.zero) //Hack when the Velocity is not set
            {
                Velocity = transform.forward;
                Force = 1;
            }

            doRayCast = true;

            if (m_collider)
            {
                EnableCollider(0.1f); //Don't enable it right away so it does not collide with the thrower
                doRayCast = m_collider.isTrigger;
            }

            if (rb)
            {
                rb.isKinematic = false; //IMPORTANT!!!
                rb.velocity = Vector3.zero; //Reset the velocity IMPORTANT!


                StartCoroutine(Artificial_Gravity()); //Check if the Gravity is not the Physics Gravity

                if (rotation == ProjectileRotation.Random)
                {
                    rb.AddTorque(new Vector3(Random.value, Random.value, Random.value).normalized * torque, ForceMode.Acceleration);
                }
                else if (rotation == ProjectileRotation.Axis)
                {
                    rb.AddTorque(torqueAxis * torque.Value, ForceMode.Impulse);
                }
                Debug.Log("RIGID BODY Gravity");
                rb.AddForce(Velocity, ForceMode.VelocityChange);
            }

            StartCoroutine(FlyingProjectile());

            OnFire.Invoke();

            Debugging("Projectile Fired", this);
        }

        public void EnableCollider(float time) => Invoke(nameof(Enable_Collider), time);

        private void Enable_Collider()
        {
            if (m_collider) m_collider.enabled = true;
        }

        private void DestroyProjectile()
        {
            if (HasImpacted && !DestroyOnImpact)
                Destroy(gameObject, Life); //Reset after has impacted the Destroy Time
            else
                Destroy(gameObject);
        }


        void OnCollisionEnter(Collision other)
        {
            if (rb && rb.isKinematic) return;
            if (HasImpacted) return; //Do not check new Collisions
            if (IsInvalid(other.collider)) return;
            if (!enabled) return;

            if (Prev_pos == Vector3.zero) Prev_pos = transform.position;

            ProjectileImpact(other.rigidbody, other.collider, other.contacts[0].point, (other.collider.bounds.center - m_collider.transform.position).normalized); 

        }

        private void OnTriggerEnter(Collider other)
        {
            if (HasImpacted) return; //Do not check new Collisions
            if (IsInvalid(other)) return;
            if (!enabled) return;

            if (Prev_pos == Vector3.zero) Prev_pos = transform.position;

            ProjectileImpact(other.attachedRigidbody, other, Prev_pos, (other.bounds.center - m_collider.transform.position).normalized);
        }

        private void OnDisable() { StopAllCoroutines(); }


        /// <summary> When the Gravity is not Physic.Gravity whe apply our own </summary>
        IEnumerator Artificial_Gravity()
        {
            if (Gravity == Physics.gravity)
            {
                rb.useGravity = true;
            }
            else if (Gravity != Vector3.zero)
            {
                var waitForFixedUpdate = new WaitForFixedUpdate();
                rb.useGravity = false;
                while (!HasImpacted)
                {
                    rb.AddForce(Gravity, ForceMode.Acceleration);
                    yield return waitForFixedUpdate;
                }
            }
            yield return true;
        }

        /// <summary> Logic Applied when the projectile is flying</summary>
        IEnumerator FlyingProjectile()
        {
            Vector3 start = transform.position;
            Prev_pos = start;
            float deltatime = Time.fixedDeltaTime;
            var waitForFixedUpdate = new WaitForFixedUpdate();

            int i = 1;

            while (!HasImpacted && enabled)
            {
                var time = deltatime * i;
                Vector3 next_pos = start + Velocity * time + Gravity * time * time / 2;

                if (!rb) transform.position = Prev_pos; //If there's no Rigid body move the Projectile!!

                Direction = next_pos - Prev_pos;
                Debug.DrawLine(Prev_pos, next_pos, Color.yellow);

                if (FollowTrajectory) //The Projectile will rotate towards de Direction
                {
                    if (Direction.sqrMagnitude > 0)
                        transform.rotation = Quaternion.LookRotation(Direction, transform.up);
                }

                if (doRayCast && Physics.Linecast(Prev_pos, next_pos, out RaycastHit hit, Layer, triggerInteraction))
                {
                    yield return waitForFixedUpdate;

                    if (!IsInvalid(hit.collider))
                    {
                        ProjectileImpact(hit.rigidbody, hit.collider, hit.point, hit.normal);
                        yield break;
                    }
                }

                Prev_pos = next_pos;
                i++;

                yield return waitForFixedUpdate;
            }
            yield return null;
        }

        public void PrepareDamage(StatModifier modifier, float CriticalChance, float CriticalMultiplier)
        {
            if (!KeepDamageValues)
            {
                statModifier = new StatModifier(modifier);
                this.CriticalChance = CriticalChance;
                this.CriticalMultiplier = CriticalMultiplier;
            }
        }

        public virtual void ProjectileImpact(Rigidbody targetRB, Collider collider, Vector3 HitPosition, Vector3 normal)
        {
            if (!enabled) return;

            Debugging($"<color=yellow> <b>[Projectile Impact] </b> [{collider.name}] </color>",collider);  //Debug

            HasImpacted = true;
            TargetHitPosition = HitPosition; //Store the Hit position of the Projectile

            StopAllCoroutines();

            if (rb)
            {
                if (!m_collider || m_collider.isTrigger) //if there's no collider or the projectile collider is a trigger
                {
                    rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                    rb.isKinematic = true;
                    rb.constraints = RigidbodyConstraints.FreezeAll;
                }
            }

            TryInteract(collider.gameObject);
            TryDamage(collider.gameObject, statModifier);

            // TryPhysics(targetRB, collider, Direction, Force);
            targetRB?.AddForceAtPosition(Direction.normalized * Velocity.magnitude * PushMultiplier, HitPosition, forceMode); //Add a force to the Target RigidBody

            OnHit.Invoke(collider.transform);
            OnHitPosition.Invoke(HitPosition);

            TryHitEffectProjectile(HitPosition, normal, collider.transform);

            switch (impactBehaviour)
            {
                case ImpactBehaviour.None:
                    break;
                case ImpactBehaviour.StickOnSurface:
                    Stick_On_Surface(collider, HitPosition);
                    break;
                case ImpactBehaviour.DestroyOnImpact:
                    DestroyProjectile();
                    return;
                case ImpactBehaviour.ActivateRigidBody:
                    if (rb)
                    {
                        rb.useGravity = true;
                        rb.isKinematic = false;
                        rb.constraints = RigidbodyConstraints.None;
                        if (collider)
                        {
                            collider.enabled = true;
                            collider.isTrigger = false;
                            Destroy(this);
                        }
                    }
                    break;
                default:
                    break;
            }


            //Life Impact Logic
            if (LifeImpact > 0 && impactBehaviour != ImpactBehaviour.DestroyOnImpact)
            {
                Destroy(gameObject, LifeImpact); //Reset after has impacted the Destroy Time
            }
        }

        private void Stick_On_Surface(Collider collider, Vector3 HitPosition)
        {
            transform.SetParentScaleFixer(collider.transform, HitPosition);
            transform.position += transform.forward * Penetration; //Put the Projectile a bit deeper in the collider
        }

        protected void TryHitEffectProjectile(Vector3 HitPosition, Vector3 Normal, Transform hitTransform)
        {
            if (HitEffect != null)
            {
                var HitRotation = Quaternion.FromToRotation(Vector3.up, Normal);

                if (debug) MTools.DrawWireSphere(HitPosition, Color.red, 0.2f, 1);

                if (HitEffect != null)
                {

                    Debugging($"<color=yellow> <b>[HitEffect] </b> [{HitEffect.name}] , {HitPosition} </color>",this);  //Debug


                    if (HitEffect.IsPrefab())
                    {
                        var instance = Instantiate(HitEffect, HitPosition, HitRotation);

                        instance.transform.SetParentScaleFixer(hitTransform, HitPosition); //Fix the Scale issue
                       
                        //Reset the gameobject visibility 

                        CheckHitEffect(instance);
                        if (DestroyHitEffect > 0) Destroy(instance, DestroyHitEffect);
                    }
                    else
                    {
                        HitEffect.transform.position = HitPosition;
                        HitEffect.transform.rotation = HitRotation;
                        CheckHitEffect(HitEffect);
                    }
                }
            }
        }

        public void DamageMultiplier(float multiplier) => statModifier.Value *= multiplier;


#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            rb = GetComponent<Rigidbody>();
            m_collider = GetComponentInChildren<Collider>();
        }
#endif


    }



    /// ----------------------------------------
    /// EDITOR
    /// ----------------------------------------

    #region Inspector


#if UNITY_EDITOR
    [CustomEditor(typeof(MProjectile))]
    public class MProjectileEditor : MDamagerEd
    {
        SerializedProperty gravity, Penetration, /*InstantiateOnImpact,*/ PushMultiplier, Editor_Tabs1, KeepDamageValues,
            Life, LifeImpact,
            OnFire, impactBehaviour, rotation, torque, torqueAxis, m_PosOffset, m_RotOffset, rb, m_collider;

        protected string[] Tabs1 = new string[] { "General", "Damage", "Physics", "Events" };
        MProjectile M;

        readonly string[] rotationTooltip = new string[] {
             "No Rotation is applied to the projectile while flying",
             "The projectile will follow its trajectory while flying",
             "The projectile will inherit the rotation it had before it was fired",
             "The projectile will rotate randomly while flying",
             "The projectile will rotate around an axis (world relative)"};

        private void OnEnable()
        {
            FindBaseProperties();
            M = (MProjectile)target;

            gravity = serializedObject.FindProperty("gravity");

            OnFire = serializedObject.FindProperty("OnFire");

            Life = serializedObject.FindProperty("Life");
            LifeImpact = serializedObject.FindProperty("LifeImpact");
            impactBehaviour = serializedObject.FindProperty("impactBehaviour");
            rotation = serializedObject.FindProperty("rotation");

            Penetration = serializedObject.FindProperty("Penetration");
            PushMultiplier = serializedObject.FindProperty("PushMultiplier");
           
            m_PosOffset = serializedObject.FindProperty("m_PosOffset");
            m_RotOffset = serializedObject.FindProperty("m_RotOffset");
            KeepDamageValues = serializedObject.FindProperty("m_KeepDamageValues");


            torque = serializedObject.FindProperty("torque");
            torqueAxis = serializedObject.FindProperty("torqueAxis");
          //  InstantiateOnImpact = serializedObject.FindProperty("InstantiateOnImpact");
            Editor_Tabs1 = serializedObject.FindProperty("Editor_Tabs1");
            rb = serializedObject.FindProperty("rb");
            m_collider = serializedObject.FindProperty("m_collider");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawDescription("Logic for Projectiles. When is fired by a Thrower component, use the method Prepare() to transfer all the properties from the thrower");

            Editor_Tabs1.intValue = GUILayout.Toolbar(Editor_Tabs1.intValue, Tabs1);

            int Selection = Editor_Tabs1.intValue;
            if (Selection == 0) DrawGeneral();
            else if (Selection == 1) DrawDamage();
            else if (Selection == 2) DrawExtras();
            else if (Selection == 3) DrawEvents();
           // EditorGUILayout.PropertyField(debug);
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawExtras()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            DrawPhysics(false);
            EditorGUILayout.PropertyField(gravity);
                EditorGUILayout.PropertyField(PushMultiplier);
            EditorGUILayout.EndVertical();
            DrawMisc();
        }

        private void DrawDamage()
        {
            EditorGUILayout.PropertyField(KeepDamageValues, new GUIContent("Keep Values"));
            if (!M.KeepDamageValues)
            {
                EditorGUILayout.HelpBox("If the Projectile is thrown by a Throwable, the Stat will be set by the Throwable. [E.g. The Arrow will get the Damage from the bow]", MessageType.Info);
            }
            else
            {
                DrawStatModifier();
                DrawCriticalDamage();
            }
        }

        protected override void DrawGeneral(bool drawbox = true)
        {
            base.DrawGeneral(drawbox);


            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                Life.isExpanded = MalbersEditor.Foldout(Life.isExpanded, "Projectile Life");

                if (Life.isExpanded)
                {
                    EditorGUILayout.PropertyField(Life);
                    EditorGUILayout.PropertyField(LifeImpact);
                }

                m_PosOffset.isExpanded = MalbersEditor.Foldout(m_PosOffset.isExpanded, "Offsets");

                if (m_PosOffset.isExpanded)
                {
                    EditorGUILayout.PropertyField(m_PosOffset, new GUIContent("Position"));
                    EditorGUILayout.PropertyField(m_RotOffset, new GUIContent("Rotation"));
                }

                rotation.isExpanded = MalbersEditor.Foldout(rotation.isExpanded, "Rotation Behaviour");

                if (rotation.isExpanded)
                {
                    EditorGUILayout.PropertyField(rotation, new GUIContent("Rotation", rotationTooltip[rotation.intValue]));

                    if (rotation.intValue == 2)
                        EditorGUILayout.PropertyField(torque);
                    else if (rotation.intValue == 3)
                    {
                        EditorGUILayout.PropertyField(torque);
                        EditorGUILayout.PropertyField(torqueAxis);
                    }
                }

                impactBehaviour.isExpanded = MalbersEditor.Foldout(impactBehaviour.isExpanded, "On Impact");

                if (impactBehaviour.isExpanded)
                {
                    EditorGUILayout.PropertyField(impactBehaviour);
                    if (impactBehaviour.intValue == 1)
                        EditorGUILayout.PropertyField(Penetration);
                }

                rb.isExpanded = MalbersEditor.Foldout(rb.isExpanded, "References");

                if (rb.isExpanded)
                {
                    EditorGUILayout.PropertyField(rb, new GUIContent("Rigid Body"));
                    EditorGUILayout.PropertyField(m_collider, new GUIContent("Collider"));
                }
            }

        }

        protected override void DrawCustomEvents() => EditorGUILayout.PropertyField(OnFire);
    }
#endif

    #endregion
}