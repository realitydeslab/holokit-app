using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalbersAnimations.Weapons
{
    [CreateAssetMenu(menuName = "Malbers Animations/Weapons/IK Profile")]
    public class IKProfile : ScriptableObject
    {
        [Tooltip("Use Animator.SetLookAtWeight() Function")]
        public bool LookAtIK = false;
        [Hide("LookAtIK")]
        [Tooltip("(0-1) the global weight of the LookAt, multiplier for other parameters.")]
        public float Weight = 1;
        [Hide("LookAtIK")]
        [Tooltip("(0-1) determines how much the body is involved in the LookAt.")]
        public float BodyWeight = 1;
        [Hide("LookAtIK")]
        [Tooltip("(0-1) determines how much the head is involved in the LookAt.")]
        public float HeadWeight = 1;
        [Hide("LookAtIK")]
        [Tooltip("(0-1) determines how much the eyes is involved in the LookAt.")]
        public float EyesWeight = 1;
        [Hide("LookAtIK")]
        [Tooltip("(0-1) 0.0 means the character is completely unrestrained in motion, 1.0 means he's completely clamped (look at becomes impossible), and 0.5 means he'll be able to move on half of the possible range (180 degrees).")]
        public float ClampWeight = 0f;
        [Hide("LookAtIK")]
        [Tooltip("(0-1) Distance to Determine the LookAtPosition")]
        public float Distance = 100f;
        [Hide("LookAtIK")]
        [Tooltip("Offset of the LookAt Ray Horizontally")]
        public float HorizontalOffset = 0;
        [Hide("LookAtIK")]
        [Tooltip("Offset of the LookAt Ray Vertically")]
        public float VerticalOffset = 0;
        //[Hide("LookAtIK")]
        //public HumanBodyBones AimOrigin = HumanBodyBones.Head;

        [Space]
        public List<BoneOfsset> offsets;
        public virtual void ApplyLookAt(Animator Anim, Vector3 origin,  Vector3 Dir, float weight)
        {
          //  var origin = Anim.GetBoneTransform(AimOrigin);
            Dir = Quaternion.AngleAxis(HorizontalOffset, Vector3.up) * Dir;

            var RightV = Vector3.Cross(Dir, Vector3.up);
            Dir = Quaternion.AngleAxis(VerticalOffset, RightV) * Dir;


            var ray = new Ray(origin, Dir);
            var Point = ray.GetPoint(Distance);
            Debug.DrawLine(origin, Point, Color.cyan);
            Anim.SetLookAtWeight(Weight * weight, BodyWeight, HeadWeight, EyesWeight, ClampWeight);
            Anim.SetLookAtPosition(Point);
        }

        public virtual void ApplyOffsets(Animator Anim, Vector3 Origin, Vector3 Direction, float Weight)
        {


            var transform = Anim.transform.root; //Best to use Root... (Riding)!

            for (int i = 0; i < offsets.Count; i++)
            {
                if (Direction == Vector3.zero) continue;
                var offset = offsets[i];

                var bn = Anim.GetBoneTransform(offset.bone);
                if (bn == null) return;

                // offset.ParentBoneOffset = bn.parent.localRotation;

                var OffsetRot = Quaternion.Euler(offset.RotationOffset);
                var InverseRot = Quaternion.Inverse(bn.parent.rotation); //This is the Bone Rotation in world coordinates
                var BoneRotation = bn.localRotation;


                //if (i > 0 && Anim.GetBoneTransform(offsets[i - 1].bone) == bn.parent)
                //{ BoneRotation = offsets[i - 1].ParentBoneOffset; }

                Quaternion finalRotation = Quaternion.identity;

                switch (offset.rotationType)
                {
                    case BoneOfsset.IKType.AdditiveOffset:
                        finalRotation = BoneRotation * OffsetRot;
                        break;
                    case BoneOfsset.IKType.OffsetOnly:
                        finalRotation = OffsetRot;
                        break;
                    case BoneOfsset.IKType.WorldRotation:
                        finalRotation = InverseRot * OffsetRot;
                        break;
                    case BoneOfsset.IKType.LookAtDir:
                        finalRotation = InverseRot * Quaternion.LookRotation(Direction, transform.up) * OffsetRot;
                        break;
                    case BoneOfsset.IKType.RootRotation:
                        finalRotation = InverseRot * transform.rotation * OffsetRot;
                        break;
                    case BoneOfsset.IKType.LootAtYAxis:
                        Vector3 RotationAxis = Vector3.Cross(transform.up, Direction).normalized;
                        var VerticalAngle = (Vector3.Angle(transform.up, Direction) - 90);   //Get the Normalized value for the look direction
                        finalRotation = Quaternion.AngleAxis(VerticalAngle, RotationAxis); // * transform.rotation;
                        finalRotation = InverseRot * finalRotation * OffsetRot;
                        Debug.DrawRay(bn.position, RotationAxis, Color.red);
                        Debug.DrawRay(bn.position, Direction, Color.green);
                        break;
                    default:
                        break;
                }

                var ResWeight = offset.Weight * Weight;
                if (ResWeight > 0)
                {
                    var result = Quaternion.Lerp(BoneRotation, finalRotation, ResWeight);
                    Anim.SetBoneLocalRotation(offset.bone, result);
                }
            }

            if (LookAtIK) ApplyLookAt(Anim, Origin, Direction, Weight);
        }

        void OnValidate()
        {
            Weight = Mathf.Clamp01(Weight);
            BodyWeight = Mathf.Clamp01(BodyWeight);
            HeadWeight = Mathf.Clamp01(HeadWeight);
            EyesWeight = Mathf.Clamp01(EyesWeight);
            ClampWeight = Mathf.Clamp01(ClampWeight);


            foreach (var item in offsets)
            {
                item.name = item.rotationType.ToString() + " [" + item.bone.ToString() + "]";
            }
        }
    }

    [System.Serializable]
    public class BoneOfsset
    {
        [HideInInspector]
        public string name;
        public enum IKType { AdditiveOffset, OffsetOnly, WorldRotation, RootRotation, LookAtDir, LootAtYAxis }
        public IKType rotationType;
        [SearcheableEnum] public HumanBodyBones bone;
        public Vector3 RotationOffset;
        [Range(0, 1)]
        public float Weight;

        public Quaternion ParentBoneOffset {get;set;}
    }

    public struct IKGoalOffsets
    {
        [SearcheableEnum] public AvatarIKGoal ikGoal;
        public Vector3 PositionOffset;
        public Vector3 RotationOffset;
    }
}