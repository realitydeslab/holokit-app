using UnityEngine;

namespace MalbersAnimations
{
    /// <summary>Interface to Identify Collectables Item</summary>
    public interface ICollectable : IObjectCore
    {
        /// <summary>Applies the Item Dropped Logic</summary>
        void Drop();

        /// <summary>Applies the Item Picked Logic</summary>

        void Pick();

        void EnablePhysics();

        void DisablePhysics();

        /// <summary>Can the Collectable be droped or Picked?</summary>
        bool InCoolDown { get; }

        /// <summary> Is the Item Picked?</summary>
        bool IsPicked { get; set; }

        /// <summary>When an Object is Collectable it means that the Picker can still pick objects, the item was collected by other compoent (E.g. Weapons or Inventory)</summary>
        bool Collectable { get; set; }
    }
}