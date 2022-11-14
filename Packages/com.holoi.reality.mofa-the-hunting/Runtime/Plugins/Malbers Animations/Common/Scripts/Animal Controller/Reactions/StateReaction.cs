using UnityEngine;


namespace MalbersAnimations.Controller.Reactions
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "Malbers Animations/Animal Reactions/State Reaction"/*, order = 0*/)]
    public class StateReaction : MReaction
    {
        public State_Reaction type = State_Reaction.Activate;
        public StateID ID;

        [Tooltip("This will change the value of the Exit Status parameter on the Animator. Useful to use different animations when Exiting a State")]
        [Hide("ShowExitStatus",false)]
        public int ExitStatus;

        [Hide("ShowEnterStatus",  false)]
        [Tooltip("This will change the value of the Enter Status parameter on the Animator. Useful to use different animations when Activating a State")]
        public int EnterStatus;

        protected override void _React(MAnimal animal)
        {
            switch (type)
            {
                case State_Reaction.Activate:
                    animal.State_Activate(ID);
                    break;
                case State_Reaction.AllowExit:
                    if (ID == null || animal.ActiveStateID == ID) animal.ActiveState.AllowExit();
                    break;
                case State_Reaction.ForceActivate:
                    animal.State_Force(ID);
                    break;
                case State_Reaction.Enable:
                    animal.State_Enable(ID);
                    break;
                case State_Reaction.Disable:
                    animal.State_Disable(ID);
                    break;
                case State_Reaction.SetExitStatus:
                    animal.State_SetExitStatus(ExitStatus);
                    break;
                case State_Reaction.AllowExitToState:
                    animal.ActiveState.AllowExit(ID.ID, ExitStatus);
                    break;
                default:
                    break;
            }
        }


        protected override bool _TryReact(MAnimal animal)
        {
            if (!animal.HasState(ID)) return false;
            
            switch (type)
            {
                case State_Reaction.Activate:
                    return animal.State_TryActivate(ID);
                case State_Reaction.AllowExit:
                    if (ID == null || animal.ActiveStateID == ID)
                    {
                        return animal.ActiveState.AllowExit();
                    }
                    return false;
                case State_Reaction.ForceActivate:
                    animal.State_Force(ID);
                    return true;
                case State_Reaction.Enable:
                    animal.State_Enable(ID);
                    return true;
                case State_Reaction.Disable:
                    animal.State_Disable(ID);
                    return true;
                case State_Reaction.SetExitStatus:
                    animal.State_SetStatus(ExitStatus);
                    return true;
                case State_Reaction.AllowExitToState:
                    animal.ActiveState.AllowExit(ID.ID,ExitStatus);
                    return true;
                default:
                    return false;
            }
        }


        public enum State_Reaction
        {
            /// <summary>Tries to Activate the State of the Zone</summary>
            Activate,
            /// <summary>If the Animal is already on the state of the zone it will allow to exit and activate states below the Active one</summary>
            AllowExit,
            /// <summary>Force the State of the Zone to be enable even if it cannot be activate at the moment</summary>
            ForceActivate,
            /// <summary>Enable a  Disabled State </summary>
            Enable,
            /// <summary>Disable State </summary>
            Disable,
            /// <summary>Change the Status ID of the State in case the State uses its</summary>
            SetExitStatus,
            /// <summary>AllowExitTo</summary>
            AllowExitToState
        }




        /// 
        /// VALIDATIONS
        /// 



        private void OnEnable() { Validation(); }

        private void OnValidate() { Validation(); }

        [HideInInspector] public bool ShowExitStatus, ShowEnterStatus;
        private const string reactionName = "State → ";

        void Validation()
        {
            fullName = reactionName + type.ToString() + " [" + (ID != null ? ID.name : "None") + "]";
            ShowExitStatus = ShowEnterStatus = false;

            switch (type)
            {
                case State_Reaction.Activate:
                    description = "Tries to activate a state, checking all the activation conditions";
                    ShowEnterStatus = true;
                    break;
                case State_Reaction.AllowExit:
                    description = $"If the Animal is on the [{(ID != null ? ID.name : "Any")}] state,\n" +
                        "it will enable [Allow Exit] on the Current State, allowing low priority states to try activating themselves";
                    break;
                case State_Reaction.ForceActivate:
                    description = "Forces and activation of a state, regardless the activation conditions";
                    ShowEnterStatus = true;
                    break;
                case State_Reaction.Enable:
                    description = "Enables a State on the Animal";
                    break;
                case State_Reaction.Disable:
                    description = "Disables a State on the Animal";
                    break;
                case State_Reaction.SetExitStatus:
                    description = "Set the Exit Status value of a state";
                    ShowExitStatus = true;
                    break;
                case State_Reaction.AllowExitToState:
                    description = $"The Animal will enable [Allow Exit] on the active state, and it will force the next state to be [{(ID != null ? ID.name : "None")}]";
                    ShowExitStatus = true;
                    break;
                default:
                    break;
            }
        }
    }
}
