using JetBrains.Annotations;
using MalbersAnimations.Scriptables;
using MalbersAnimations.Weapons;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;
using UnityEngine.Events;

namespace MalbersAnimations
{
    [System.Serializable]
    [UnityEngine.CreateAssetMenu(menuName = "Malbers Animations/ID/Holster", fileName = "New Holster ID", order = -1000)]
    public class HolsterID : IDs { }


    [System.Serializable]
    public class Holster
    {
        public HolsterID ID;
        public int Index;

        [Tooltip("Slots Transforms used to store weapons")]
        public List<Transform> Slots;

        /// <summary> Transform Reference for the holster</summary>
        [Tooltip("Weapon GameObject asociated to the Holster")]
        public MWeapon Weapon;

        [Tooltip("Input to Equip the weapon in the holster")]
        public StringReference Input = new StringReference();

        /// <summary> Used to connect the Inputs to the Abilities instead of the General Mode </summary>
        public UnityAction<bool> InputListener;
         
        public WeaponEvent OnWeaponInHolster = new WeaponEvent();

        public int GetID => ID != null ? ID.ID : 0;

        public void SetWeapon(MWeapon weap)
        {
            if (Weapon) //Meaning there's a OLD weapon already on the holster
            {
                Weapon.IsCollectable?.Drop();
            }

            Weapon = weap;

            if (Weapon != null)
            {
                Weapon.IsCollectable?.DisablePhysics();
                OnWeaponInHolster.Invoke(Weapon);
            }
            else
            {
                OnWeaponInHolster.Invoke(null);
            }
        }

        public void SetWeapon(GameObject weap) => Weapon = weap.GetComponent<MWeapon>();


        public Transform GetSlot(int index) => Slots[index];

        /// <summary>  Prepare the weapons. Instantiate/Place them in the holster if they are linked in the Weapon Manager </summary>
        public bool PrepareWeapon()
        {
            if (Weapon)
            {
                var slot = Slots[Weapon.HolsterSlot]; //Get the correct slot

                if (Weapon.gameObject.IsPrefab()) //if it is a prefab then instantiate it!!
                {
                    if (slot.childCount > 0)
                    {
                        Object.Destroy(slot.GetChild(0).gameObject);
                    }

                    Weapon = GameObject.Instantiate(Weapon);
                    Weapon.name = Weapon.name.Replace("(Clone)", "");

                    Weapon.Debugging("[Instantiated]", Weapon);
                }

                Weapon.Holster = ID; //MAKE SURE THE WEAPON HAS THE SAME ID OF THE WEAPON 

                //Reparent a frame after
                Weapon.Delay_Action(() =>
                {
                    if (!Weapon.IsEquiped)
                    {
                        Weapon.transform.SetParent(slot);
                        Weapon.transform.SetLocalTransform(Weapon.HolsterOffset);
                    }
                }
                );

                OnWeaponInHolster.Invoke(Weapon);
                Weapon.IsCollectable?.Pick(); //Pick the weapon in case is a collectible

                return true;
            }
            return false;
        } 

        public static implicit operator int(Holster reference) => reference.GetID;
    }
}
