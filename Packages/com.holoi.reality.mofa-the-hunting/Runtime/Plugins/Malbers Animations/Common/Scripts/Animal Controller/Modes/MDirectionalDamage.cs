using UnityEngine;

namespace MalbersAnimations.Controller
{
    [CreateAssetMenu(menuName = "Malbers Animations/Modifier/Mode/Directional Damage")]
    public class MDirectionalDamage : ModeModifier
    {
        public enum HitDirection { TwoSides, FourSides, SixSides }

       
        [Header("Damage Abilities")]
        public HitDirection hitDirection = HitDirection.SixSides;

        [Hide("hitDirection", (int)HitDirection.SixSides)]
        public int FrontRight = 4;
        public int Right = 2;
        [Hide("hitDirection", (int)HitDirection.SixSides)]
        public int BackRight = 5;
        [Hide("hitDirection", (int)HitDirection.SixSides)]
        public int FrontLeft = 3;
        public int Left = 1;
        [Hide("hitDirection", (int)HitDirection.SixSides)]
        public int BackLeft = 6;
        [Hide("hitDirection", (int)HitDirection.FourSides)]
        public int Front = 3;
        [Hide("hitDirection", (int)HitDirection.FourSides)]
        public int Back = 4;

        public bool debug = false;

        public override void OnModeEnter(Mode mode)
        {
            MAnimal animal = mode.Animal;

            Vector3 HitDirection = animal.GetComponent<IMDamage>().HitDirection;

            if (HitDirection == Vector3.zero) return; //Set it to random if there's no hit direction


            HitDirection = -Vector3.ProjectOnPlane(HitDirection, animal.UpVector);  //Remove the Y on the Direction
            float Angle = Vector3.Angle(animal.Forward, HitDirection);             //Get The angle
            bool left = Vector3.Dot(animal.Right, HitDirection) < 0;                //Calculate which directions comes the hit Left or right

            var Colordeb = Color.blue;
            float mult = 2;

            int Side = -99;

            float Dtime = 3f;
          

            switch (hitDirection)
            {
                case MDirectionalDamage.HitDirection.TwoSides:
                    Side = left ? Left : Right;

                    if (debug)
                    {
                        Debug.DrawRay(animal.transform.position, animal.transform.forward * mult, Colordeb, Dtime);
                        Debug.DrawRay(animal.transform.position, -animal.transform.forward * mult, Colordeb, Dtime);
                        Debug.DrawRay(animal.transform.position,
                            Quaternion.Euler(0, Angle * (left ? -1 : 1), 0) * animal.transform.forward * mult, Color.red, Dtime);
                    }
                    break;
                case MDirectionalDamage.HitDirection.FourSides:

                    if (Angle <= 45)
                    {
                        Side = Front;
                    }
                    else if (Angle >= 45 && Angle <= 135)
                    {
                        Side = left ? Right : Left;
                    }
                    else if (Angle >= 135)
                    {
                        Side = Back;
                    }


                    if (debug)
                    {
                        Debug.DrawRay(animal.transform.position, Quaternion.Euler(0, 45, 0) * animal.transform.forward * mult, Colordeb, Dtime);
                        Debug.DrawRay(animal.transform.position, Quaternion.Euler(0, -45, 0) * animal.transform.forward * mult, Colordeb, Dtime);
                        Debug.DrawRay(animal.transform.position, Quaternion.Euler(0, 135, 0) * animal.transform.forward * mult, Colordeb, Dtime);
                        Debug.DrawRay(animal.transform.position, Quaternion.Euler(0, -135, 0) * animal.transform.forward * mult, Colordeb, Dtime);
                        Debug.DrawRay(animal.transform.position,
                            Quaternion.Euler(0, Angle * (left ? -1 : 1), 0) * animal.transform.forward * mult, Color.red, Dtime);
                    }


                    break;
                case MDirectionalDamage.HitDirection.SixSides:

                    if (debug)
                    {
                        Debug.DrawRay(animal.transform.position, animal.transform.forward * mult, Colordeb, Dtime);
                        Debug.DrawRay(animal.transform.position, -animal.transform.forward * mult, Colordeb, Dtime);
                        Debug.DrawRay(animal.transform.position, Quaternion.Euler(0, 60, 0) * animal.transform.forward * mult, Colordeb, Dtime);
                        Debug.DrawRay(animal.transform.position, Quaternion.Euler(0, -60, 0) * animal.transform.forward * mult, Colordeb, Dtime);
                        Debug.DrawRay(animal.transform.position, Quaternion.Euler(0, 120, 0) * animal.transform.forward * mult, Colordeb, Dtime);
                        Debug.DrawRay(animal.transform.position, Quaternion.Euler(0, -120, 0) * animal.transform.forward * mult, Colordeb, Dtime);
                        Debug.DrawRay(animal.transform.position, Quaternion.Euler(0, Angle * (left ? -1 : 1), 0) * animal.transform.forward * mult, Color.red, Dtime);
                    }

                    if (!left)
                    {
                        if (Angle >= 0 && Angle <= 60) Side = FrontRight;
                        else if (Angle > 60 && Angle <= 120) Side = Right;
                        else if (Angle > 120 && Angle <= 180) Side = BackRight;
                    }
                    else
                    {
                        if (Angle >= 0 && Angle <= 60) Side = FrontLeft;
                        else if (Angle > 60 && Angle <= 120) Side = Left;
                        else if (Angle > 120 && Angle <= 180) Side = BackLeft;
                    }
                    break;
                default:
                    break;
            }
            mode.AbilityIndex = Side;
        }
    }
}