using UnityEngine;

namespace MalbersAnimations
{
    /// <summary>
    /// Interface to Identify Core GameObjects even if they have parents and are not in the scene root.. E.g. MAnimal, MRider, Pickables, Collectables..
    /// </summary>
    public interface IObjectCore
    { 
        /// <summary>Transform associated to the gameobject</summary>
        Transform transform { get; }
    } 
}