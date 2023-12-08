using MalbersAnimations.Controller.Reactions;
using UnityEngine;

namespace MalbersAnimations
{
    /// <summary>Damagee interface for components that can be damaged</summary>
    public interface IMDamage
    { 
        /// <summary>Which direction the Damage came from</summary>
        Vector3 HitDirection { get; set; }

        ///// <summary>Which direction the Damage came from</summary>
        //Vector3 HitPosition { get; set; }

        /// <summary>Who is doing the Damage?</summary>
        GameObject Damager { get; set; }

        /// <summary>Who is Receiving the Damage?</summary>
        GameObject Damagee { get; }

        /// <summary>  Method to receive damage from an Atacker  </summary>
        /// <param name="Direction">Direction where the damage comes from</param>
        /// <param name="Damager">Who is sending the Damage?</param>
        /// <param name="stat">What stat to modify</param>
        /// <param name="IsCritical">was the damage critical</param>
        /// <param name="react">does the Animal use default reaction? </param>
        /// <param name="ignoreDamageeM">Ignore Damagee Multiplier</param>
        void ReceiveDamage(Vector3 Direction, GameObject Damager, StatModifier stat, bool IsCritical, bool Default_react, MReaction custom,  bool ignoreDamageeM);


        /// <summary>  Method to receive damage from an Atacker (Simplified)  </summary>
        void ReceiveDamage(StatID stat, float amount);
    }

    /// <summary>The Damager Interface</summary>
    public interface IMDamager : IMLayer
    {
        /// <summary> ID of the Damager </summary>
        int Index { get; }

        /// <summary>Enable/Disable the Damager</summary>
        bool Enabled { get; set; }

        /// <summary>Owner of the Damager, Usually the Character. This is used to avoid Hitting yourself</summary>
        GameObject Owner { get; set; }

        /// <summary>Do Damage with Attack Triggers</summary>
        void DoDamage(bool value, float multiplier);
    }

    /// <summary>Used to activate Damager GameObject By its ID (Damagers with Triggers). E.g An animal has several Damagers</summary>
    public interface IMDamagerSet
    {
        /// <summary>
        ///   Activate an specific Damager by its ID. Zero(0) Deactivate all Damagers. -1 Activate all Damagers 
        /// </summary>
        /// <param name="ID"> ID of the Attack Trigger... this will enable or disable the Colliders</param>
        /// <param name="multiplier">Multiplier to multipy to the Damage Value</param>
        /// <param name="StateInfoHash">ID of the Animation to know who's sending the Damager..is it the same Animation or a new one???</param>
        void ActivateDamager(int ID, float multiplier);

        void DamagerAnimationStart(int hash);

        void DamagerAnimationEnd(int hash);
    }
}