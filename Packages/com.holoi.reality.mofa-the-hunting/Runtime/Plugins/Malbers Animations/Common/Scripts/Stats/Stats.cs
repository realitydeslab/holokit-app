using MalbersAnimations.Events;
using MalbersAnimations.Scriptables;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MalbersAnimations
{
    /// <summary> Component managing Stat Logic</summary>
    [AddComponentMenu("Malbers/Stats/Stats")]
    public class Stats : MonoBehaviour, IAnimatorListener
    {
        //[Tooltip("Track these Stats in a Runtime Set")]
        //[CreateScriptableAsset] public RuntimeStats Set;

        /// <summary>List of Stats</summary>
        public List<Stat> stats = new List<Stat>();
        /// <summary>List of Stats Converted to Dictionary</summary>
        public Dictionary<int, Stat> stats_D;

        /// <summary>Stored Stat to use the 'Pin' Methods</summary>
        public Stat PinnedStat;

        public virtual bool OnAnimatorBehaviourMessage(string message, object value) => this.InvokeWithParams(message, value);

        public void Initialize()
        {
            StopAllCoroutines();

            stats_D = new Dictionary<int, Stat>();

            foreach (var stat in stats)
            {
                if (stat.ID == null)
                {
                    Debug.LogError("One of the Stats has an Empty ID", gameObject);
                    break;
                }
                stat.InitializeStat(this);

                if (!stats_D.ContainsKey(stat.ID)) //Added by SkillManiacs 2020/10
                {
                    stats_D.Add(stat.ID, stat); //Convert them to Dictionary
                }
                else
                {
                    stats_D[stat.ID] = stat; //Replace it
                }
            }
        }

        private void Awake()
        {
            Initialize();
        }


        private void OnEnable()
        {
            foreach (var stat in stats_D)
            {
                if (stat.Value.ID == null)
                {
                    Debug.LogError("One of the Stats has an Empty ID", gameObject);
                    break;
                }
                stat.Value.InitializeStat(this);
            }
        }

        private void OnDisable() => StopAllCoroutines();





        /// <summary>Updates all Stats</summary>
        public virtual void Stats_Update()
        {
            foreach (var s in stats) s.UpdateStat();
        }

        /// <summary>Updates a stat logic by its Stat ID</summary>
        public virtual void Stats_Update(StatID iD) => Stats_Update(iD.ID);

        public virtual void Stats_Update(int iD) => Stat_Get(iD)?.UpdateStat();

        /// <summary> Reset a Stat to the Default Max Value</summary>
        public virtual void Stat_Reset_to_Max(StatID iD) => Stat_Get(iD)?.Reset_to_Max();

        /// <summary> Reset a Stat to the Default Min Value</summary>
        public virtual void Stat_Reset_to_Min(StatID iD) => Stat_Get(iD)?.Reset_to_Min();

        /// <summary>Disable a stat</summary>
        public virtual void Stat_Disable(StatID iD) => Stat_Get(iD)?.SetActive(false);

        /// <summary>Disable a stat Degeneration logic</summary>
        public virtual void Stat_Degenerate_Off(StatID ID) => Stat_Get(ID)?.SetDegeneration(false);

        /// <summary>Enable a stat Degeneration logic</summary>
        public virtual void Stat_Degenerate_On(StatID ID) => Stat_Get(ID)?.SetDegeneration(true);

        /// <summary>Disable a stat Regeneration logic</summary>
        public virtual void Stat_Regenerate_Off(StatID ID) => Stat_Get(ID)?.SetRegeneration(false);

        /// <summary>Enable a stat Regeneration logic</summary>
        public virtual void Stat_Regenerate_On(StatID ID) => Stat_Get(ID)?.SetRegeneration(true);


        #region Callbacks with StatID parameters
        /// <summary>Enable a stat</summary>
        public virtual void Stat_Enable(StatID iD) => Stat_Get(iD)?.SetActive(true);

        /// <summary>Set PinnedStat searching for a StatID</summary>
        public virtual void Stat_Pin(StatID ID) => Stat_Get(ID.ID);

        /// <summary>Find a Stat Using a StatID and Return if the Stat is on the List. Also Saves it to the PinnedStat</summary>
        public virtual Stat Stat_Get(StatID ID) => Stat_Get(ID.ID);

        // <summary>Set the Inmune Value of a Stat to true</summary>
        public virtual void Stat_Inmune_Activate(StatID ID) => Stat_Get(ID)?.SetInmune(true);

        /// <summary>Set the Inmune Value of a Stat to false</summary>
        public virtual void Stat_Inmune_Deactivate(StatID ID) => Stat_Get(ID)?.SetInmune(false);

        #endregion


        /// <summary>Set PinnedStat searching for a Stat Name</summary>
        public virtual void Stat_Pin(string name) => Stat_Get(name);

        /// <summary>Set PinnedStat searching for a int ID value</summary>
        public virtual void Stat_Pin(int ID) => Stat_Get(ID);


        /// <summary>Find a Stat Using its name for the ID and Return if the Stat is on the List. Also Saves it to the PinnedStat</summary>
        public virtual Stat Stat_Get(string name) => PinnedStat = stats.Find(item => item.Name == name);

        /// <summary>Find a Stat Using a int Value for the ID and Return if the Stat is on the List. Also Saves it to the PinnedStat</summary>
        public virtual Stat Stat_Get(int ID)
        {
            if (stats_D != null && stats_D.TryGetValue(ID, out PinnedStat))
                return PinnedStat;
            return null;
        }
        /// <summary>Find a Stat Using an IntVar and Return if the Stat is on the List. Also Saves it to the PinnedStat</summary>
        public virtual Stat Stat_Get(IntVar ID) => Stat_Get(ID.Value);
         
        public virtual void Stat_ModifyValue(StatID ID, float modifyvalue) => Stat_Get(ID)?.Modify(modifyvalue);
        public virtual void Stat_ModifyValue(int ID, float modifyvalue) => Stat_Get(ID)?.Modify(modifyvalue);
        public virtual void Stat_ModifyValue(string name, float modifyvalue) => Stat_Get(name)?.Modify(modifyvalue);

        public virtual void Stat_ModifyValue(StatID ID, float modifyvalue, StatOption modifyType) => Stat_Get(ID)?.Modify(modifyvalue, modifyType);
        public virtual void Stat_ModifyValue(string name, float modifyvalue, StatOption modifyType) => Stat_Get(name)?.Modify(modifyvalue, modifyType);
         
        /// <summary>Modify Stat Value instantly (Add/Remove to the Value)</summary>
        public virtual void Stat_Pin_ModifyValue(float value) => PinnedStat?.Modify(value);

        /// <summary>Modify Stat Value instantly (Add/Remove to the Value)</summary>
        public virtual void Stat_Pin_ModifyValue(FloatVar value) => PinnedStat?.Modify(value.Value);

        /// <summary>Modify Stat Value instantly (Add/Remove to the Value)</summary>
        public virtual void Stat_Pin_SetMult(float value) => PinnedStat?.SetMultiplier(value);

        /// <summary>Modify Stat Value instantly (Add/Remove to the Value)</summary>
        public virtual void Stat_Pin_SetMult(FloatVar value) => PinnedStat?.SetMultiplier(value.Value);

        /// <summary>Modify Stat Value in a X time period(Add/Remove to the Value)</summary>
        public virtual void Stat_Pin_ModifyValue(float value, float time) => PinnedStat?.Modify(value, time);

        /// <summary>Modify Stat Value in 1 second period(Add/Remove to the Value)</summary>
        public virtual void Stat_Pin_ModifyValue_1Sec(float value) => PinnedStat?.Modify(value, 1);

        /// <summary>Set  Stat Value to a fixed Value</summary>
        public virtual void Stat_Pin_SetValue(float value) => PinnedStat.SetValue(value);

        /// <summary>Modify the Pinned Stat MAX Value (Add or remove to the Max Value) </summary>
        public virtual void Stat_Pin_ModifyMaxValue(float value) => PinnedStat?.ModifyMAX(value);

        /// <summary>Set the Pinned Stat MAX Value </summary>
        public virtual void Stat_Pin_SetMaxValue(float value) => PinnedStat?.SetMAX(value);

        /// <summary> Enable/Disable the Pinned Stat Regeneration Rate </summary>
        public virtual void Stat_Pin_Modify_RegenRate(float value) => PinnedStat?.ModifyRegenRate(value);

        /// <summary> Enable/Disable the Pinned Stat Degeneration </summary>
        public virtual void Stat_Pin_Degenerate(bool value) => PinnedStat?.SetDegeneration(value);

        /// <summary> Enable/Disable the Pinned Stat Degeneration </summary>
        public virtual void Stat_Pin_SetInmune(bool value) => PinnedStat?.SetInmune(value);



        /// <summary>Enable/Disable the Pinned Stat Regeneration </summary>
        public virtual void Stat_Pin_Regenerate(bool value) => PinnedStat?.SetRegeneration(value);
        //  public virtual void _PinStatRegenerate(bool value) { Stat_Pin_Regenerate(value); }

        /// <summary> Enable/Disable the Pinned Stat</summary>
        public virtual void Stat_Pin_Enable(bool value) => PinnedStat?.SetActive(value);

        /// <summary>Modify the Pinned Stat value with a 'new Value',  'ticks' times , every 'timeBetweenTicks' seconds</summary>
        public virtual void Stat_Pin_ModifyValue(float newValue, int ticks, float timeBetweenTicks) => PinnedStat?.Modify(newValue, ticks, timeBetweenTicks);

        /// <summary> Clean the Pinned Stat from All Regeneration/Degeneration and Modify Tick Values </summary>
        public virtual void Stat_Pin_CleanCoroutines() => PinnedStat?.CleanRoutines();




        [Obsolete("Use Stat_Degenerate_Off instead")]
        /// <summary>Disable a stat Degeneration logic</summary>
        public virtual void DegenerateOff(StatID ID) => Stat_Degenerate_Off(ID);

        [Obsolete("Use Stat_Degenerate_On instead")]
        /// <summary>Enable a stat Degeneration logic</summary>
        public virtual void DegenerateOn(StatID ID) => Stat_Degenerate_On(ID);






#if UNITY_EDITOR

        [ContextMenu("Create/Stamina")]
        private void ConnectStamina()
        {
            if (stats == null) stats = new List<Stat>();


            var staminaID = MTools.GetInstance<StatID>("Stamina");


            if (staminaID != null)
            {
                var staminaStat = Stat_Get(staminaID);

                if (staminaStat == null)
                {
                    staminaStat = new Stat()
                    {
                        ID = staminaID,
                        value = new FloatReference(100),
                        InmuneTime = new FloatReference(0.5f),
                        regenerate = new BoolReference(true),
                        RegenRate = new FloatReference(40),
                        DegenRate = new FloatReference(20),
                        RegenWaitTime = new FloatReference(2),
                        Above = 15f, Below = 10f,
                    };
                    stats.Add(staminaStat);
                }

                //Connect to the Animal Controller in case it exist
                var method = this.GetUnityAction<bool>("MAnimal", "UseSprint");
              
                if (method != null)
                {
                    Debug.Log("medho" + method.ToString());
                    UnityEditor.Events.UnityEventTools.AddBoolPersistentListener(staminaStat.OnStatBelow, method, false);
                    UnityEditor.Events.UnityEventTools.AddBoolPersistentListener(staminaStat.OnStatAbove, method, true);
                }

                MEvent UIStamina = MTools.GetInstance<MEvent>("UI Stamina Stat");

                if (UIStamina)
                {
                    UnityEditor.Events.UnityEventTools.AddPersistentListener(staminaStat.OnValueChangeNormalized,UIStamina.Invoke);
                    UnityEditor.Events.UnityEventTools.AddPersistentListener(staminaStat.OnStatFull,UIStamina.Invoke);
                }


                var onSprintEnable = this.GetFieldClass<BoolEvent>("MAnimal", "OnSprintEnabled");

                if (onSprintEnable != null)
                {
                    UnityEditor.Events.UnityEventTools.AddObjectPersistentListener<StatID>(onSprintEnable, Stat_Pin, staminaID);
                    UnityEditor.Events.UnityEventTools.AddPersistentListener(onSprintEnable, Stat_Pin_Degenerate);
                }

                MTools.SetDirty(this);
            }
        }

        [ContextMenu("Create/Health")]
        void CreateHealth()
        {
            var health = MTools.GetInstance<StatID>("Health");

            if (health != null)
            {

                var HealthStat = new Stat()
                {
                    ID = health,
                    value = new FloatReference(100),
                    DisableOnEmpty = new BoolReference(true),
                    InmuneTime = new FloatReference(0.1f)
                };
                stats.Add(HealthStat);


                var deathID = MTools.GetInstance<StateID>("Death");

                var method = this.GetUnityAction<StateID>("MAnimal", "State_Activate"); 

                if (method != null) UnityEditor.Events.UnityEventTools.AddObjectPersistentListener<StateID>(HealthStat.OnStatEmpty, method, deathID);


                MEvent UIHealth = MTools.GetInstance<MEvent>("UI Health Stat");

                if (UIHealth)
                {
                    UnityEditor.Events.UnityEventTools.AddPersistentListener(HealthStat.OnValueChangeNormalized, UIHealth.Invoke);
                    UnityEditor.Events.UnityEventTools.AddPersistentListener(HealthStat.OnStatFull, UIHealth.Invoke);
                }

                MTools.SetDirty(this);
            }
        }



        [ContextMenu("Create/Mana")]
        void CreateMana()
        {
            var Mana = MTools.GetInstance<StatID>("Mana");

            if (Mana != null)
            {

                var HealthStat = new Stat()
                {
                    ID = Mana,
                    value = new FloatReference(100),
                    DisableOnEmpty = new BoolReference(true),
                    InmuneTime = new FloatReference(0), 
                    regenerate = new BoolReference(true),
                    RegenWaitTime = new FloatReference(2),
                    RegenRate = new FloatReference(10), 
                    DegenRate = new FloatReference(10) 
                };
                stats.Add(HealthStat);


                MEvent UIHealth = MTools.GetInstance<MEvent>("UI Mana Stat");

                if (UIHealth)
                {
                    UnityEditor.Events.UnityEventTools.AddPersistentListener(HealthStat.OnValueChangeNormalized, UIHealth.Invoke);
                    UnityEditor.Events.UnityEventTools.AddPersistentListener(HealthStat.OnStatFull, UIHealth.Invoke);
                }

                MTools.SetDirty(this);
            }
        }

        private void Reset()
        {
            if (stats == null) stats = new List<Stat>();

            CreateHealth();
            MTools.SetDirty(this);
        }
#endif
    }


    ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    ///STAT CLASS
    ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

    [Serializable]
    public class Stat
    {
        #region Variables 
       
       [Tooltip("Enable/Disable the Stat. Disable Stats cannot be modified")]
        public bool active = true;
       [Tooltip("Key Idendifier for the Stat")]
        public StatID ID; 
        [Tooltip("Current Value of the Stat")]
        public FloatReference value = new FloatReference(0);
        [Tooltip("Maximun Value of the Stat")] 
        public FloatReference maxValue = new FloatReference(100);
        [Tooltip("Minimum Value of the Stat")] 
        public FloatReference minValue = new FloatReference();
        [Tooltip("If the Stat is Empty it will be disabled to avoid future changes")] 
        public BoolReference DisableOnEmpty = new BoolReference();

        /// <summary>Multiplier to modify the Stat value</summary>
        [SerializeField] internal FloatReference multiplier = new FloatReference(1);

        /// <summary>Can the Stat regenerate overtime</summary>
        [SerializeField] internal BoolReference regenerate = new BoolReference( false);
        /// <summary>Regeneration Rate. Change the Speed of the Regeneration</summary>
        public FloatReference RegenRate;
        /// <summary>Regeneration Rate. When the value is modified this will increase or decrease it over time.</summary>
        public FloatReference RegenWaitTime = new FloatReference(0);
        /// <summary>Regeneration Rate. When the value is modified this will increase or decrease it over time.</summary>
        public FloatReference DegenWaitTime = new FloatReference(0);
        /// <summary>Can the Stat degenerate overtime</summary>
        [SerializeField] internal BoolReference degenerate = new BoolReference(false);
        /// <summary>Degeneration Rate. Change the Speed of the Degeneration</summary>
        public FloatReference DegenRate;
        /// <summary>If greater than zero, the Stat cannot be modify until the inmune time have passed</summary>
        public FloatReference InmuneTime;
        /// <summary>If the ResetStat funtion is called it will reset to Max or Low Value</summary>
        public ResetTo resetTo = ResetTo.MaxValue;
        /// <summary> Save the Last State of the Regeneration bool</summary>
        private bool regenerate_LastValue;
        /// <summary> Save the Last State of the Regeneration bool</summary>
        private bool degenerate_LastValue;
        /// <summary> Is the Stat Below the Below Value</summary>
        private bool isBelow = false;
        /// <summary> Is the Stat Above the Above Value</summary>
        private bool isAbove = false;
        #endregion

        #region Events
        public UnityEvent OnStatFull = new UnityEvent();
        public UnityEvent OnStatEmpty = new UnityEvent();
        public UnityEvent OnStat = new UnityEvent();
        public float Below;
        public float Above;
        public UnityEvent OnStatBelow = new UnityEvent();
        public UnityEvent OnStatAbove = new UnityEvent();
        public FloatEvent OnValueChangeNormalized = new FloatEvent();
        public FloatEvent OnValueChange = new FloatEvent();
        public BoolEvent OnDegenerate = new BoolEvent();
        public BoolEvent OnRegenerate = new BoolEvent();
        public BoolEvent OnActive = new BoolEvent();
        #endregion

        #region Properties
        /// <summary>Is the Stat Enabled? when Disable no modification can be done. All current modification can't be stopped</summary>
        public bool Active
        {
            get => active; 
            set
            {
                active = value;

                OnActive.Invoke(value);
                if (value)
                    StartRegeneration(); //If the Stat was activated start the regeneration
                else
                    StopRegeneration();
            }
        }

        public string Name
        {
            get
            {
                if (ID != null)
                {
                    return ID.name;
                }
                return string.Empty;
            }
        }

        /// <summary> Current value of the Stat</summary>
        public float Value
        {
            get => value;
            set => SetValue(value);
        }

        public bool IsFull => Value == MaxValue;
        public bool IsEmpty => Value == MinValue;

        /// <summary> Current Multiplier of the Stat</summary>
        public float Multiplier { get => multiplier.Value; set => multiplier.Value = value; }

        /// <summary>Returns the Normalized value of the Stat</summary>
        public float NormalizedValue => Value / MaxValue;

        /// <summary>If True: The Stat cannot be modify </summary>
        public bool IsInmune { get; set; }      

        /// <summary>Maximum Value of the Stat</summary>
        public float MaxValue { get => maxValue.Value; set => maxValue.Value = value; }

        /// <summary>Minimun Value of the Stat </summary>
        public float MinValue { get => minValue.Value; set => minValue.Value = value; }

        /// <summary>Is the Stat Regenerating?</summary>
        public bool IsRegenerating { get; private set; }

        /// <summary>Is the Stat Degenerating?</summary>
        public bool IsDegenerating { get; private set; }

        [SerializeField] internal int EditorTabs = 0;

        /// <summary>Can the Stat Regenerate over time</summary>
        public bool Regenerate
        {
            get => regenerate.Value;
            set
            {
                regenerate.Value = value;
                regenerate_LastValue = regenerate;           //In case Regenerate is changed 
                OnRegenerate.Invoke(value);

                if (regenerate)
                {
                    degenerate.Value = false;     //Do not Degenerate if we are Regenerating
                    StopDegeneration();
                    StartRegeneration();
                }
                else
                {
                    degenerate.Value = degenerate_LastValue;   //If we are no longer Regenerating Start Degenerating again in case the Degenerate was true
                    StopRegeneration();
                    StartDegeneration();
                }
            }
        }

        /// <summary> Can the Stat Degenerate over time </summary>
        public bool Degenerate
        {
            get => degenerate.Value; 
            set
            {
                degenerate.Value = value;
                degenerate_LastValue = degenerate;           //In case Regenerate is changed 
                OnDegenerate.Invoke(value);

                if (degenerate)
                {
                    regenerate.Value = false;     //Do not Regenerate if we are Degenerating
                    StartDegeneration();
                    StopRegeneration();
                }
                else
                {
                    regenerate.Value = regenerate_LastValue;   //If we are no longer Degenerating Start Regenerating again in case the Regenerate was true
                    StopDegeneration();
                    StartRegeneration();
                }
            }
        }

        #endregion

        [NonSerialized] private WaitForSeconds InmuneWait;

        internal void InitializeStat(Stats holder)
        {
            isAbove = isBelow = false;
            Owner = holder;                                     //Save the Monobehaviour to save coroutines

            if (value.Value >= Above) isAbove = true;           //This means that The Stat Value is over the Above value
            else if (value.Value <= Below) isBelow = true;      //This means that The Stat Value is under the Below value

            regenerate_LastValue = Regenerate;

            if (MaxValue < Value) MaxValue = Value;


            I_Regeneration = null;
            I_Degeneration = null;
            I_ModifyPerTicks = null;

            InmuneWait = new WaitForSeconds(InmuneTime);

            if (Active)
            {
                StartRegeneration();
                StartDegeneration();
            }

            holder.Delay_Action(3,() => ValueEvents());
        }

        internal void SetMultiplier(float value) => multiplier.Value = value;


        internal void ValueEvents()
        {
            OnValueChangeNormalized.Invoke(NormalizedValue);
            OnValueChange.Invoke(value);


            if (this.value == minValue.Value)
            {
                this.value.Value = minValue.Value;
                OnStatEmpty.Invoke();   //if the Value is 0 invoke Empty Stat

                if (DisableOnEmpty.Value)
                {
                    SetActive(false);
                    return;
                }

            }
            else if (this.value == maxValue.Value)
            {
                this.value.Value = maxValue.Value;
                OnStatFull.Invoke();    //if the Value is 0 invoke Empty Stat
            }


            if (this.value >= Above && !isAbove)
            {
                OnStatAbove.Invoke();
                isAbove = true;
                isBelow = false;
            }
            else if (this.value <= Below && !isBelow)
            {
                OnStatBelow.Invoke();
                isBelow = true;
                isAbove = false;
            }
        }

        internal void SetValue(float value)
        {
            var RealValue = Mathf.Clamp(value * Multiplier, MinValue, maxValue);

            if ((!Active) ||                                    //If the  Stat is not Active do nothing 
                (this.value.Value == RealValue)) return;        //If the values are equal do nothing. Avoid Stack Overflow
            
            this.value.Value = RealValue;

            ValueEvents();
        }

        /// <summary>Enable or Disable a Stat </summary>
        public void SetActive(bool value) => Active = value;
        public void SetRegeneration(bool value) => Regenerate = value;
        public void SetDegeneration(bool value) => Degenerate = value;
        public void SetInmune(bool value) => IsInmune = value;

        /// <summary>Adds or remove to the Stat Value </summary>
        public virtual void Modify(float newValue)
        {
            if (!IsInmune && Active)
            {
                Value += newValue;
                StartRegeneration();
                if (!Regenerate) 
                    StartDegeneration();

                SetInmune();
            }
        }

        public virtual void UpdateStat()
        {
            SetValue(value);
            StartRegeneration();
            if (!Regenerate)
                StartDegeneration();
        }

        /// <summary>Adds or remove to the Stat Value</summary>
        public virtual void Modify(float newValue, float time)
        { 
            if (!IsInmune && Active)
            {
                StopSlowModification();
                Owner.StartCoroutine(out I_ModifySlow, C_SmoothChangeValue(newValue, time));
                SetInmune();
            }
        }

        /// <summary>  Modify the Stat value with a 'new Value',  'ticks' times , every 'timeBetweenTicks' seconds </summary>
        public virtual void Modify(float newValue, int ticks, float timeBetweenTicks)
        {
            if (!Active) return;            //Ignore if the Stat is Disable
            StopCoroutine(I_ModifyPerTicks);

            Owner.StartCoroutine(out I_ModifyPerTicks, C_ModifyTicksValue(newValue, ticks, timeBetweenTicks));
        }

        /// <summary> Add or Remove Value the 'MaxValue' of the Stat </summary>
        public virtual void ModifyMAX(float newValue)
        {
            if (!Active) return;            //Ignore if the Stat is Disable
            MaxValue += newValue;
            StartRegeneration();
        }

        /// <summary>Sets the 'MaxValue' of the Stat </summary>
        public virtual void SetMAX(float newValue)
        {
            if (!Active) return;    
            MaxValue = newValue;
            StartRegeneration();
        }


        /// <summary>Add or Remove Rate to the Regeneration Rate</summary>
        public virtual void ModifyRegenRate(float newValue)
        {
            if (!Active) return;            //Ignore if the Stat is Disable

            RegenRate.Value += newValue;
            StartRegeneration();
        }

        public virtual void SetRegenerationWait(float newValue)
        {
            if (!Active) return;            //Ignore if the Stat is Disable

            RegenWaitTime.Value = newValue;

            if (RegenWaitTime < 0) RegenWaitTime.Value = 0;
        }

        /// <summary>Set a new Regeneration Rate</summary>
        public virtual void SetRegenerationRate(float newValue)
        {
            if (!Active) return;            //Ignore if the Stat is Disable
            RegenRate.Value = newValue;
        }

        /// <summary> Reset the Stat to the Default Max Value</summary>
        public virtual void Reset() => Value = (resetTo == ResetTo.MaxValue) ? MaxValue : MinValue;

        /// <summary> Reset the Stat to the Default Min or Max Value</summary>
        public virtual void Reset_to_Max() => Value = MaxValue;

        /// <summary> Reset the Stat to the Default Min  Value</summary>
        public virtual void Reset_to_Min() => Value = MinValue;
        /// <summary>Clean all Coroutines</summary>
        internal void CleanRoutines()
        {
            StopDegeneration();
            StopRegeneration();
            StopTickDamage();
            StopSlowModification();
        }


        public virtual void RegenerateOverTime(float time)
        {
            if (time <= 0)
            {
                StartRegeneration();
            }
            else
            {
                Owner.StartCoroutine(C_RegenerateOverTime(time));
            }
        }

        protected virtual void SetInmune()
        {
            if (InmuneTime > 0)
            {
                StopCoroutine(I_IsInmune);
                Owner.StartCoroutine(out I_IsInmune, C_InmuneTime());
            }
        }

        

        private void StopCoroutine(IEnumerator Cor)
        {
            if (Cor != null) Owner.StopCoroutine(Cor);
        }

        protected virtual void StartRegeneration()
        {
            StopRegeneration();

            if (RegenRate == 0 || !Regenerate) return;   //Means if there's no Regeneration

            Owner.StartCoroutine(out I_Regeneration, C_Regenerate());
        }


        protected virtual void StartDegeneration()
        {
            StopDegeneration();
            if (DegenRate == 0 || !Degenerate) return;  //Means there's no Degeneration

            Owner.StartCoroutine(out I_Degeneration, C_Degenerate());
        }

        protected virtual void StopRegeneration()
        {
            StopCoroutine(I_Regeneration);    //If there was a regenation active .... interrupt it
              
            I_Regeneration = null;
            IsRegenerating = false;
        }

        protected virtual void StopDegeneration()
        {
            StopCoroutine(I_Degeneration);    //if it was ALREADY Degenerating.. stop
              
            I_Degeneration = null;
            IsDegenerating = false;
        }

        protected virtual void StopTickDamage()
        {
            StopCoroutine(I_ModifyPerTicks);   //if it was ALREADY Degenerating.. stop...
            I_ModifyPerTicks = null;
        }

        protected virtual void StopSlowModification()
        {
            StopCoroutine(I_ModifySlow);       //If there was a regenation active .... interrupt it
            I_ModifySlow = null;
        }

        /// <summary>Modify the Stats on an animal </summary>
        public void Modify(float Value, StatOption modify)
        {
            switch (modify)
            {
                case StatOption.AddValue:
                    Modify(Value);
                    break;
                case StatOption.SetValue:
                    this.Value = Value;
                    break;
                case StatOption.SubstractValue:
                    Modify(-Value);
                    break;
                case StatOption.ModifyMaxValue:
                    ModifyMAX(Value);
                    break;
                case StatOption.SetMaxValue:
                    MaxValue = Value;
                    break;
                case StatOption.Degenerate:
                    DegenRate = Value;
                    Degenerate = true;
                    break;
                case StatOption.StopDegenerate:
                    DegenRate = Value;
                    Degenerate = false;
                    break;
                case StatOption.Regenerate:
                    Regenerate = true;
                    RegenRate = Value;
                    break;
                case StatOption.StopRegenerate:
                    Regenerate = false;
                    RegenRate = Value;
                    break;
                case StatOption.Reset:
                    Reset();
                    break;
                case StatOption.ReduceByPercent:
                    Modify(-(MaxValue * Value / 100));
                    break;
                case StatOption.IncreaseByPercent:
                    Modify(MaxValue * Value / 100);
                    break;
                case StatOption.Multiplier:
                    Multiplier = Value;
                    break;
                case StatOption.ResetToMax:
                    Reset_to_Max();
                    break;
                case StatOption.ResetToMin:
                    Reset_to_Min();
                    break;
                case StatOption.None:
                    break;
                default:
                    break;
            }
        }


        #region Coroutines
        /// <summary>
        ///  I need this to use coroutines in this class because it does not inherit from Monobehaviour, Also to Identify where is this Stat coming from
        /// </summary>
        public Stats Owner { get; private set; }       
        private IEnumerator I_Regeneration;
        private IEnumerator I_Degeneration;
        private IEnumerator I_ModifyPerTicks;
        private IEnumerator I_ModifySlow;
        private IEnumerator I_IsInmune;


        protected IEnumerator C_RegenerateOverTime(float time)
        {
            float ReachValue = RegenRate > 0 ? MaxValue : MinValue;                                //Set to the default or 0
            bool Positive = RegenRate > 0;                                                          //Is the Regeneration Positive?
            float currentTime = Time.time;

            while (Value != ReachValue || currentTime > time )
            {
                Value += (RegenRate * Time.deltaTime);

                if (Positive && Value > MaxValue)
                {
                    Value = MaxValue;
                }
                else if (!Positive && Value < 0)
                {
                    Value = MinValue;
                }
                currentTime += Time.deltaTime;

                yield return null;
            }
            yield return null;
        }

        protected IEnumerator C_InmuneTime()
        {
            IsInmune = true;
            yield return InmuneWait;
            IsInmune = false;
        }

        protected IEnumerator C_Regenerate()
        {
            yield return null;

            if (RegenWaitTime > 0)
                yield return new WaitForSeconds(RegenWaitTime);          //Wait a time to regenerate

            IsRegenerating = true;



            while (Regenerate && Value < MaxValue)
            {
                Value += (RegenRate * Time.deltaTime);
                yield return null;
            }

            IsRegenerating = false;
            yield return null;
        }

        protected IEnumerator C_Degenerate()
        {
            yield return null;
           
            if (DegenWaitTime > 0)
                yield return new WaitForSeconds(DegenWaitTime);          //Wait a time to regenerate
            
            IsDegenerating = true;

            while (Degenerate && Value > MinValue)
            {
                Value -= (DegenRate * Time.deltaTime);
                yield return null;
            }
            IsDegenerating = false;
            yield return null;
        }

        protected IEnumerator C_ModifyTicksValue(float value, int Ticks, float time)
        {
            var WaitForTicks = new WaitForSeconds(time);

            for (int i = 0; i < Ticks; i++)
            {
                Value += value;
                if (Value <= MinValue)
                {
                    Value = MinValue;
                    break;
                }
                yield return WaitForTicks;
            }

            yield return null;

            StartRegeneration();
        }

        protected IEnumerator C_SmoothChangeValue(float newvalue, float time)
        {
            StopRegeneration();
            float currentTime = 0;
            float currentValue = Value;
            newvalue = Value + newvalue;

            yield return null;
            while (currentTime <= time)
            {

                Value = Mathf.Lerp(currentValue, newvalue, currentTime / time);
                currentTime += Time.deltaTime; 

                yield return null;
            }
            Value = newvalue;

            yield return null;
            StartRegeneration();
        }
        #endregion

        public enum ResetTo
        {
            MinValue,
            MaxValue
        }
    }


}