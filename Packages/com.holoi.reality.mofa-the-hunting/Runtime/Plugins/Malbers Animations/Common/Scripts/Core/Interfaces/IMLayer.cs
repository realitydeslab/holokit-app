using UnityEngine;

namespace MalbersAnimations
{
    /// <summary> Interface to check Layers and Layer Interactions</summary>
    public interface IMLayer
    {
        /// <summary>Layers to Interact</summary>
        LayerMask Layer { get; set; }

        /// <summary>What to do with the Triggers ... Ignore them? Use them?</summary>
        QueryTriggerInteraction TriggerInteraction { get; set; }
    }
}