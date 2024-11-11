using MalbersAnimations.Scriptables;
using System.Collections.Generic;
using UnityEngine;

namespace MalbersAnimations.Controller
{
    /// <summary>
    /// Sets the Ability Index by checking which direction the Input is being pressed.
    /// </summary>
    [CreateAssetMenu(menuName = "Malbers Animations/Modifier/Mode/Input Axis")]
    public class ModifierInputAxis : ModeModifier
    {
        public List<AxisAbility> axisAbilities = new List<AxisAbility>(1) { new AxisAbility() { Ability = 1, Direction = Vector3.zero } };

        public override void OnModeEnter(Mode mode)
        {
            var MoveInput = mode.Animal.RawInputAxis;

            var close = axisAbilities[0];
            //float Diference = Vector3.Distance(close.Direction, MoveInput);
            float Diference = (close.Direction - MoveInput).sqrMagnitude;

            foreach (var mt in axisAbilities)
            {
                //var newDiff = Vector3.Distance(mt.Direction, MoveInput);
                var newDiff = (mt.Direction - MoveInput).sqrMagnitude;

                if (newDiff < Diference)
                {
                    Diference = newDiff;
                    close = mt;
                }
            }

            mode.AbilityIndex = close.Ability;
            if (mode.Animal.debugModes) mode.Debugging($"Input Axis Mode Modifier Set Index to [{close.Name} - {close.Ability}]");
        }
    }

    [System.Serializable]
    public struct AxisAbility
    {
        public string Name;
        public Vector3 Direction;
        public int Ability;
    }
}