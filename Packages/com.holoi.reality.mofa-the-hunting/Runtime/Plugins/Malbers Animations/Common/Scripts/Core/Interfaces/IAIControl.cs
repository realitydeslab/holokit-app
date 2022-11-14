using UnityEngine;

namespace MalbersAnimations 
{
    /// <summary>Interface used for Moving the Animal using AI</summary>
    public interface IAIControl
    {
        /// <summary>Gets the Agent Transform Reference</summary>  
        Transform Transform { get; }

        /// <summary>Target Assigned to the AI Control</summary>  
        Transform Target { get; set; }

        /// <summary>Destination Position to use on Agent.SetDestination()</summary>
        Vector3 DestinationPosition { get; set; }

        /// <summary>Direction the Animal is Travelling</summary>
        Vector3 AIDirection { get; set; }

        /// <summary>Is the Target an AI Target</summary>
        IAITarget IsAITarget { get; set; }

        /// <summary>Returns the Current Target Position.</summary>
        Vector3 GetTargetPosition();

        /// <summary>Current Stopping Distance for the Current Destination</summary>
        float StoppingDistance { get; set; }

        /// <summary>Reset the Stopping Distance to its Default value</summary>
        void ResetStoppingDistance();


        /// <summary>Returns the Height of the AI Agent</summary>
        float Height { get; }

        /// <summary>Current Slowing Distance for the Destination</summary>
        float CurrentSlowingDistance { get; set; }
        float SlowingDistance { get; }

        /// <summary>Stores the Remainin distance to the Target's Position</summary>
        float RemainingDistance { get; set; }

        /// <summary>Set the next Target, and set if the Agent will move or not to that target</summary>  
        void SetTarget(Transform target, bool move);

        /// <summary>Remove the current Target and stop the Agent </summary>
        void ClearTarget();

        /// <summary>Calculate the Next Target from the Current Target; if the current target is a Waypoint</summary>
        void MovetoNextTarget();  

        /// <summary>Set the next Destination Position without having a target, and set if the Agent will move or not to that destination</summary>   
        void SetDestination(Vector3 PositionTarget, bool move);

        /// <summary> Stop the Agent on the Animal... also remove the Transform target and the Target Position and Stops the Animal </summary>
        void Stop();

        /// <summary>If the Animal was waiting for a Next Target, it will stop the Wait Logic</summary>
        void StopWait();

        /// <summary>Calculates the Path and the Current Direction the Animal must go</summary>
        void Move(); 

        /// <summary>Enable/Disable the AI Source Control</summary>
        void SetActive(bool value);

        /// <summary>Has the Agent Arrived to the Target Position?</summary>
        bool HasArrived { get; set; }

        /// <summary>Is the Agent in a OffMesh Link</summary>       
        bool InOffMeshLink { get; set; }

        /// <summary>Do the necesary logic to complete all the Off mesh link traversal</summary>
        void CompleteOffMeshLink();

        /// <summary>Is the target moving, changed position?</summary>
        bool TargetIsMoving { get; }

        /// <summary>The Character will assign and go to a new Target (Given by the current Target) when it arrives to the current target</summary>
        bool AutoNextTarget { get; set; } 

        /// <summary>is the AI Enabled/Active?</summary>
        bool Active { get;} 
        
        /// <summary>The Character will assign and go to a new Target (Given by the current Target) when it arrives to the current target</summary>
        bool LookAtTargetOnArrival { get; set; }
        
        /// <summary>Recalculate the Targets Destination, in case the target moved</summary>
        bool UpdateDestinationPosition { get; set; }

        /// <summary>Event to send when a new Target is assigned</summary>  
        MalbersAnimations.Events.TransformEvent TargetSet { get; }

        /// <summary>Event to send when  the AI has arrived to the target</summary>  
        MalbersAnimations.Events.TransformEvent OnArrived { get; }
    }

    /// <summary>Interface used to know if Target used on the AI Movement is and AI Target</summary>
    public interface IAITarget
    {
        /// <summary> Reference for the Target's Transform</summary>
        Transform transform { get; }

        /// <summary> Stopping Distance Radius used for the AI</summary>
        float StopDistance();

        /// <summary> Default Height for the ahi Target</summary>
        float Height { get; }

        /// <summary> When the AI animal arrives to the target, do we align it so it look ats at it?</summary>
        bool ArriveLookAt { get; }

        /// <summary>Distance for AI driven animals to start slowing its speed before arriving to a gameObject. If its set to zero or lesser than the Stopping distance, the Slowing Movement Logic will be ignored</summary>
        float SlowDistance();

        /// <summary>Returns the AI Destination on an AI Target</summary>
        Vector3 GetPosition();

        /// <summary>Returns the AI Destination + the Height</summary>
        Vector3 GetCenter();

        /// <summary>Where is the Target Located, Ground, Water, Air? </summary>
        WayPointType TargetType { get; }

        /// <summary>Call this method when someones arrives to the Waypoint</summary>
        void TargetArrived(GameObject target);
    }
}


