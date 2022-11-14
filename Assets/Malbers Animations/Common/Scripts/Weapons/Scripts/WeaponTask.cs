using MalbersAnimations.Utilities;
using UnityEngine;
using MalbersAnimations.Weapons;


namespace MalbersAnimations.Controller.AI
{
    public enum BrainWeaponActions { Draw_Holster, Store_Weapon, Equip_Weapon,Unequip_Weapon ,Aim, Attack, Reload }

    [CreateAssetMenu(menuName = "Malbers Animations/Pluggable AI/Tasks/Weapon Tasks", fileName = "new Weapon Task")]
    public class WeaponTask : MTask
    {
        public override string DisplayName => "Weapons/Weapon Tasks";
        [Tooltip("Play the mode only when the animal has arrived to the target")]
        public bool near = false;
        public BrainWeaponActions Actions = BrainWeaponActions.Attack;

        [Hide("Actions",(int)BrainWeaponActions.Equip_Weapon)]
        public MWeapon Weapon;
        [Hide("Actions",(int)BrainWeaponActions.Draw_Holster)]
        public HolsterID HolsterID;
        [Hide("Actions",(int)BrainWeaponActions.Aim)]
        public bool AimValue = true;


        //[Hide("Actions",(int)BrainWeaponActions.Attack)]
        //public float AttackRate = 0.33f;

        public override void StartTask(MAnimalBrain brain, int index)
        {
          var WeaponManager = brain.Animal.GetComponentInChildren<MWeaponManager>();
           brain.TasksVars[index].mono = WeaponManager;

            if (near && !brain.AIControl.HasArrived) return; //Dont play if Play on target is true but we are not near the target.

            if (WeaponManager)
            {
                switch (Actions)
                {
                    case BrainWeaponActions.Equip_Weapon:
                        WeaponManager.Equip_External(Weapon);
                        break;
                    case BrainWeaponActions.Draw_Holster:
                        WeaponManager.Holster_Equip(HolsterID);
                        break;
                    case BrainWeaponActions.Aim:
                        WeaponManager.Aim_Set(AimValue);
                        break;
                    case BrainWeaponActions.Attack:
                        // WeaponManager.MainAttack(); //This will be called on Update!
                        break;
                    case BrainWeaponActions.Store_Weapon:
                        WeaponManager.Store_Weapon();
                        break;
                    case BrainWeaponActions.Reload:
                        WeaponManager.ReloadWeapon();
                        break;
                    case BrainWeaponActions.Unequip_Weapon:
                        WeaponManager.UnEquip();
                        brain.TaskDone(index);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                brain.TaskDone(index);
                Debug.Log("No Weapon Manager Found on the Animal", brain.Animal);
            }
        }

        public override void UpdateTask(MAnimalBrain brain, int index)
        {
            if (near && !brain.AIControl.HasArrived) return; //Dont play if Play on target is true but we are not near the target.

            var WeaponManager = brain.TasksVars[index].mono as MWeaponManager;

            if (WeaponManager)
            {
                switch (Actions)
                {
                    case BrainWeaponActions.Equip_Weapon:
                        break;
                    case BrainWeaponActions.Draw_Holster:
                        if (WeaponManager.ActiveHolster == HolsterID && WeaponManager.WeaponAction == Weapon_Action.Idle)
                        {
                            //Debug.Log("EquipHolster Done!!!");
                            brain.TaskDone(index);
                        }
                        break;
                    case BrainWeaponActions.Store_Weapon:
                        if (WeaponManager.WeaponAction == Weapon_Action.None)
                        {
                            //Debug.Log("UnEquip Done!!!");
                            brain.TaskDone(index);
                        }
                        break;
                    case BrainWeaponActions.Aim:
                        if (WeaponManager.Weapon  && WeaponManager.Weapon.IsReady)
                        {
                            //Debug.Log("Aim Ready!!!");
                            brain.TaskDone(index);
                        }
                        break;
                    case BrainWeaponActions.Attack:
                        {
                            if (WeaponManager.Weapon)
                            {
                                if (WeaponManager.Weapon is MMelee)
                                {
                                    WeaponManager.MainAttack();
                                }
                                else //
                                {
                                    if (!WeaponManager.Weapon.Input ||
                                        (WeaponManager.Weapon as MShootable).releaseProjectile == MShootable.Release_Projectile.OnAttackStart)
                                    {
                                        WeaponManager.MainAttack();
                                    }
                                    else
                                    {
                                        WeaponManager.MainAttackReleased();
                                    }
                                }
                            }
                           
                           
                            //Debug.Log("ATTACK1");
                        }
                            break;
                    case BrainWeaponActions.Reload:
                        if (!WeaponManager.IsReloading)
                        {
                            //Debug.Log("Reload Done");
                            brain.TaskDone(index);
                        }
                        break;
                    default:
                        break;
                }
            }

        }


        void Reset()
        { Description = "Use common Methods of the Weapon Manager to play on the "; }
    }
}