using MalbersAnimations.Scriptables;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace MalbersAnimations.Weapons
{
    public class MWeaponBehavior : StateMachineBehaviour
    {
        public List<WeaponMessages> weaponActions = new List<WeaponMessages>();

      //  [Space(-22)]
        public bool debug;

        private IWeaponManager manager;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (manager == null) manager = animator.FindInterface<IWeaponManager>();

            if (manager != null)
            {
                foreach (var item in weaponActions)
                {
                    item.MessageSent = false; //Reset all the sent messages

                    //WEAPON BEHAVIOR SEND MESSAGE ON ENTER
                    if (item.time == 0)   item.Execute(animator, manager,debug);
                }
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (manager != null)
            {
                foreach (var item in weaponActions)
                {
                    //WEAPON BEHAVIOR SEND MESSAGE ON EXIT
                    if (!item.MessageSent) item.Execute(animator, manager, debug); //Sent everything that was not sent
                     
                }
            }
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (manager != null)
            {
                foreach (var item in weaponActions)
                {
                    //WEAPON BEHAVIOR SEND MESSAGE ON TIME
                    if (!item.MessageSent && stateInfo.normalizedTime >= item.time)     item.Execute(animator,manager, debug);
                }
            }
        }


#if UNITY_EDITOR
        private void OnValidate()
        {
            foreach (var item in weaponActions)
            {
                item.name = Regex.Replace(item.Action.ToString(), @"((?<=\p{Ll})\p{Lu})|((?!\A)\p{Lu}(?>\p{Ll}))", " $0");

                if (item.Action == WeaponOption.WeaponIsReady) item.name = item.ready ? "Weapon is Ready" : "Weapon is NOT Ready";
              //  else if (item.Action == WeaponOption.FreeHandIK) item.name = item.IK ? "Weapon IK [ON]" : "Weapon IK [OFF]";

                if (item.time == 0)
                {
                    item.name += $"  -  [On Enter]";
                }
                else if (item.time == 1)
                {
                    item.name += $"  -  [On Exit]";
                }
                else
                    item.name += $"  -  [OnTime] ({item.time:F2})";
            }
        }
#endif
    }

   
    
    [System.Serializable]
    public class WeaponMessages
    {
        [HideInInspector]
        public string name;
        public WeaponOption Action = WeaponOption.Equip;
        [Range(0,1), Tooltip("Normalized Time. \n[0]: On State Enter\n[1]:On State Exit\n[0-1]: On State Update")]
        public float time = 0.0f;

        [Hide("Action", false, (int) WeaponOption.PlaySound)]
        public int value;

        [Hide("Action",  false, (int)WeaponOption.WeaponIsReady)]
        public bool ready = true; 

        //[Hide("Action",  false, (int)WeaponOption.FreeHandIK, (int)WeaponOption.AimIK)]
        //public bool IK = true;

        /// <summary> Was the message sent? </summary>
        public bool MessageSent {get; set;}

        public void Execute(Animator anim, IWeaponManager manager, bool debug)
        {
            switch (Action)
            {
                case WeaponOption.Equip:
                    manager.Equip_Weapon();
                    break;
                case WeaponOption.Unequip:
                    manager.Unequip_Weapon();
                    break;
                case WeaponOption.EquipProjectile:
                    if (manager.Weapon is MShootable) (manager.Weapon as MShootable).EquipProjectile();
                    break;
                case WeaponOption.FireProjectile:
                    if (manager.Weapon is MShootable) (manager.Weapon as MShootable).ReleaseProjectile();
                    break;
                case WeaponOption.Reload:
                    if (manager.Weapon is MShootable) (manager.Weapon as MShootable).ReloadWeapon();
                    break;
                case WeaponOption.FinishReload:
                    if (manager.Weapon is MShootable) (manager.Weapon as MShootable).FinishReload();
                    break;
                case WeaponOption.WeaponIsReady:
                    manager.Weapon.WeaponReady(ready);
                    break;
                case WeaponOption.CheckAim:
                    manager.CheckAim();
                    break;
                case WeaponOption.PlaySound:
                    manager.Weapon.PlaySound(value);
                    break;
                case WeaponOption.UseFreeHand:
                    manager.FreeHandUse();
                    break;
                case WeaponOption.ReleaseFreeHand:
                    manager.FreeHandRelease();
                    break;
                default:
                    break;
            }
            if (debug) Debug.Log($"[{anim.name}] Weapon Message: <color=white>[{Action}]</color>",anim);
            MessageSent = true;
        }
    }

    public enum WeaponOption
    {
        [InspectorName("Weapon/Equip")]
        Equip,
        [InspectorName("Weapon/Unequip")]
        Unequip,
        [InspectorName("Projectile/Equip")]
        EquipProjectile,
        [InspectorName("Projectile/Fire-Release")]
        FireProjectile,
        [InspectorName("Fire Weapon/Reload")]
        Reload,
        [InspectorName("Fire Weapon/Finish Reload")]
        FinishReload,
        [InspectorName("Weapon/Is Ready")]
        WeaponIsReady,
        [InspectorName("Weapon/Check Aiming")]
        CheckAim,
        [InspectorName("Weapon/Sound")]
        PlaySound,
        [InspectorName("Weapon/Use Free Hand")]
        UseFreeHand,
        [InspectorName("Weapon/Release Free Hand")]
        ReleaseFreeHand,
        //[InspectorName("IK/Free Hand IK")]
        //FreeHandIK,
        //[InspectorName("IK/Aim IK")]
        //AimIK,
        //[InspectorName("Riding/Release Reins/ Left Hand")]
        //ReleaseReinLeftHand,
        //[InspectorName("Riding/Release Reins/ Right Hand")]
        //ReleaseReinRightHand,
        //[InspectorName("Riding/Release Reins/ Both Hands")]
        //ReleaseReinFromHands,

        //[InspectorName("Riding/ Grab Reins/ Left Hand")]
        //GrabReinLeftHand,
        //[InspectorName("Riding/ Grab Reins/ Right Hand")]
        //GrabReinRightHand,
        //[InspectorName("Riding/ Grab Reins/ Both Hands")]
        //GrabReinBothHands,
    }
}