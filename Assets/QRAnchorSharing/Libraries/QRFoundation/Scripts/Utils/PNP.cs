using System.Collections.Generic;
using CSQR;
using UnityEngine;
using static QRFoundation.QRCodeTracker;

namespace QRFoundation
{
    /// <summary>
    /// How the <b>L</b>eft, <b>R</b>ight and <b>C</b>enter point are ordered
    /// in Z distance (far to near).
    /// </summary>
    public enum ZOrder
    {
        XXC,
        LCR,
        RCL,
        CXX
    }

    public class PNP
    {
        private static readonly int iterations = 20;

        public static bool debugMode = false;

        /// <summary>
        /// Naive iterative P3P solver using bisection. The order distances of the points has to be known in advance.
        /// (I would say that this method is actually pretty darn accutate, but probably too slow for large sample sizes)
        /// </summary>
        /// <returns>The distances of center, left, and right corners.</returns>
        /// <param name="centerVec">Target corner in image space</param>
        /// <param name="leftVec">Left of target corner in image space</param>
        /// <param name="rightVec">Right of target corner in image space</param>
        /// <param name="centerSource">Target corner in parameter space</param>
        /// <param name="leftSource">Left corner in parameter space</param>
        /// <param name="rightSource">Right corner in parameter space</param>
        /// <param name="zOrder">The order in which the points are away from the camera.</param>
        /// <param name="camera">The Camera</param>
        public static Vector3 P3P(Vector2 centerVec, Vector2 leftVec, Vector2 rightVec, Vector2 centerSource, Vector2 leftSource, Vector2 rightSource, ZOrder zOrder, Camera camera)
        {
            float diameter = Vector2.Distance(leftSource, rightSource);
            float leftArm = Vector2.Distance(centerSource, leftSource);
            float rightArm = Vector2.Distance(centerSource, rightSource);
            float longerArm = Mathf.Max(leftArm, rightArm);

            Ray back = camera.ScreenPointToRay(centerVec);
            Ray left = camera.ScreenPointToRay(leftVec);
            Ray right = camera.ScreenPointToRay(rightVec);

            float min = 0;
            float max_1 = (longerArm / 2) / Mathf.Sin(Vector3.Angle(back.direction, left.direction) * Mathf.Deg2Rad * 0.5f);
            float max_2 = (longerArm / 2) / Mathf.Sin(Vector3.Angle(back.direction, right.direction) * Mathf.Deg2Rad * 0.5f);
            float max = max_1 > max_2 ? max_1 : max_2;
            // Add a slight tolerance to avoid convergence at the end of the inverval.
            max *= 1.1f;

            // Save the last found intersections to avoid ending up with no intersection after the iterations.
            float halfSafe = 0;
            float leftCutSafe = 0;
            float rightCutSafe = 0;

            // Perform a binary search
            for (int i = 0; i < iterations; ++i)
            {
                float half = (min + max) / 2;
                float leftCut = 0;
                float rightCut = 0;

                switch (zOrder)
                {
                    case ZOrder.CXX:
                        leftCut = IntersectRaySphere(left.origin, left.direction, back.origin + back.direction * half, leftArm, true);
                        rightCut = IntersectRaySphere(right.origin, right.direction, back.origin + back.direction * half, rightArm, true);
                        break;
                    case ZOrder.XXC:
                        leftCut = IntersectRaySphere(left.origin, left.direction, back.origin + back.direction * half, leftArm, false);
                        rightCut = IntersectRaySphere(right.origin, right.direction, back.origin + back.direction * half, rightArm, false);
                        break;
                    case ZOrder.LCR:
                        leftCut = IntersectRaySphere(left.origin, left.direction, back.origin + back.direction * half, leftArm, false);
                        rightCut = IntersectRaySphere(right.origin, right.direction, back.origin + back.direction * half, rightArm, true);
                        break;
                    case ZOrder.RCL:
                        leftCut = IntersectRaySphere(left.origin, left.direction, back.origin + back.direction * half, leftArm, true);
                        rightCut = IntersectRaySphere(right.origin, right.direction, back.origin + back.direction * half, rightArm, false);
                        break;
                }

                if (float.IsPositiveInfinity(leftCut) || float.IsPositiveInfinity(rightCut))
                {
                    // The distance is too far to span at least one of the sides.
                    max = half;
                    continue;
                }
                if (leftCut == 0f || rightCut == 0f)
                {
                    // The distance is too close.
                    min = half;
                    continue;
                }
                // The values are safe. Save them.
                halfSafe = half;
                leftCutSafe = leftCut;
                rightCutSafe = rightCut;

                float dist = Vector3.Distance(left.origin + left.direction * leftCut, right.origin + right.direction * rightCut);

                if ((dist > diameter) ^ (zOrder == ZOrder.LCR || zOrder == ZOrder.RCL))
                    max = half;
                else
                    min = half;

            }

            return new Vector3(halfSafe, leftCutSafe, rightCutSafe);
        }

        /// <summary>
        /// Performs the P3P on three points of a quad (target corner -> <paramref name="ulv2d"/>, left of corner -> <paramref name="urv2d"/>, right of corner -> <paramref name="llv2d"/>),
        /// using the the point oppisite of the target corner (<paramref name="lrv2d"/>) as an ortientation to pick one of the four possible solutions.
        /// </summary>
        /// <returns>The distances of center, left, and right corners.</returns>
        /// <param name="ulv2d">Target corner in image space</param>
        /// <param name="urv2d">Left of target corner in image space</param>
        /// <param name="llv2d">Right of target corner in image space</param>
        /// <param name="lrv2d">Opposite corner in image space</param>
        /// <param name="ulSoure">Target corner in parameter space</param>
        /// <param name="urSource">Left corner in parameter space</param>
        /// <param name="llSource">Right corner in parameter space</param>
        /// <param name="lrSource">Opposite corner in parameter space</param>
        /// <param name="camera">The camera</param>
        public static Vector3 P3PStabilized(Vector2 ulv2d, Vector2 urv2d, Vector2 llv2d, Vector2 lrv2d, Vector2 ulSoure, Vector2 urSource, Vector2 llSource, Vector2 lrSource, Camera camera)
        {
            // Calculate the rates at which the side lengths of the quad got scaled by the projection...
            float ulur = Vector2.Distance(ulv2d, urv2d) / Vector2.Distance(ulSoure, urSource);
            float urlr = Vector2.Distance(urv2d, lrv2d) / Vector2.Distance(urSource, lrSource);
            float lrll = Vector2.Distance(lrv2d, llv2d) / Vector2.Distance(lrSource, llSource);
            float llul = Vector2.Distance(llv2d, ulv2d) / Vector2.Distance(llSource, ulSoure);
            float ullr = Vector2.Distance(ulv2d, lrv2d) / Vector2.Distance(ulSoure, lrSource);
            float urll = Vector2.Distance(urv2d, llv2d) / Vector2.Distance(urSource, llSource);

            // ... the corner adjacent to the sides that got scaled up the most (or scaled down the least) is closest to the camera.
            ZOrder zOrder = ZOrder.XXC;
            if (urll > ullr)
            {
                if (urlr > llul)
                    zOrder = ZOrder.CXX;
                else
                    zOrder = ZOrder.XXC;
            }
            else
            {
                if (lrll > ulur)
                    zOrder = ZOrder.LCR;
                else
                    zOrder = ZOrder.RCL;
            }
 
            Vector3 res = P3P(ulv2d, urv2d, llv2d, ulSoure, urSource, llSource, zOrder, camera);

            return new Vector3(res.x, res.y, res.z);
        }

        /// <summary>
        /// Turn the samples into poses.
        /// </summary>
        /// <returns>All extracted poses.</returns>
        /// <param name="samples">Samples.</param>
        /// <param name="width">The resulution of the code (width in squares).</param>
        /// <param name="camera">The camera object.</param>
        public static List<PoseResult> PNPoses(Sample[] samples, float width, Camera camera, Pose offset)
        {
            List<PoseResult> res = new List<PoseResult>(samples.Length / 4);
            List<Sample> sCopy = new List<Sample>(samples);
            while (sCopy.Count >= 4)
            {
                Sample c1 = GetClosestSample(new Point(-width / 2, -width / 2), sCopy, width / 2);
                if (c1 == null)
                {
                    break;
                }
                sCopy.Remove(c1);

                Sample c2 = GetClosestSample(new Point(width / 2, -width / 2), sCopy, width / 2);
                if (c2 == null)
                {
                    break;
                }
                sCopy.Remove(c2);

                Sample c3 = GetClosestSample(new Point(width / 2, width / 2), sCopy, width / 2);
                if (c3 == null)
                {
                    break;
                }
                sCopy.Remove(c3);

                Sample c4 = GetClosestSample(new Point(-width / 2, width / 2), sCopy, width / 2);
                if (c4 == null)
                {
                    break;
                }
                sCopy.Remove(c4);

                List<Sample> corners = new List<Sample>(4)
                {
                    c1,
                    c2,
                    c3,
                    c4
                };

                List<Sample> reAdd = new List<Sample>(4)
                {
                    c1,
                    c2,
                    c3,
                    c4
                };
                reAdd.RemoveAt(Random.Range(0, 3));
                sCopy.AddRange(reAdd);

                for (int rot = 0; rot < 4; rot++)
                {
                    // Rotate the samples for an even distribution of the 3 more relevant corners
                    Vector2 ulImage = new Vector2((float)corners[(rot + 0) % 4].image.x, (float)corners[(rot + 0) % 4].image.y);
                    Vector2 urImage = new Vector2((float)corners[(rot + 1) % 4].image.x, (float)corners[(rot + 1) % 4].image.y);
                    Vector2 llImage = new Vector2((float)corners[(rot + 3) % 4].image.x, (float)corners[(rot + 3) % 4].image.y);
                    Vector2 lrImage = new Vector2((float)corners[(rot + 2) % 4].image.x, (float)corners[(rot + 2) % 4].image.y);
                    Vector2 ulParam = new Vector2((float)corners[(rot + 0) % 4].param.x, (float)corners[(rot + 0) % 4].param.y);
                    Vector2 urParam = new Vector2((float)corners[(rot + 1) % 4].param.x, (float)corners[(rot + 1) % 4].param.y);
                    Vector2 llParam = new Vector2((float)corners[(rot + 3) % 4].param.x, (float)corners[(rot + 3) % 4].param.y);
                    Vector2 lrParam = new Vector2((float)corners[(rot + 2) % 4].param.x, (float)corners[(rot + 2) % 4].param.y);

                    if (!ContainsMalformedCorner(ulParam, urParam, lrParam, llParam))
                    {
                        continue;
                    }

                    Vector3 depths = P3PStabilized(
                        ulImage,
                        urImage,
                        llImage,
                        lrImage,
                        ulParam,
                        urParam,
                        llParam,
                        lrParam,
                        camera
                    );

                    Ray ulRay = camera.ScreenPointToRay(ulImage);
                    Ray urRay = camera.ScreenPointToRay(urImage);
                    Ray llRay = camera.ScreenPointToRay(llImage);
                    Vector3 ulP = ulRay.origin + ulRay.direction.normalized * depths.x;
                    Vector3 urP = urRay.origin + urRay.direction.normalized * depths.y;
                    Vector3 llP = llRay.origin + llRay.direction.normalized * depths.z;

                    PoseResult pose = CalcPose(ulP, urP, llP, ulParam, urParam, llParam, lrParam, width);
                    pose = ApplyOffset(pose, offset);
                    res.Add(pose);

                    if (debugMode)
                    {
                        Ray lrRay = camera.ScreenPointToRay(lrImage);
                        Debug.DrawLine(ulP, urP, Color.cyan, 0.3f);
                        Debug.DrawLine(ulP, llP, Color.cyan, 0.3f);
                        Debug.DrawLine(lrRay.origin, urP, Color.red, 0.3f);
                        Debug.DrawLine(lrRay.origin, llP, Color.red, 0.3f);
                    }
                }
            }

            res = RemoveOutliers(res);

            return res;
        }

        private static List<PoseResult> RemoveOutliers(List<PoseResult> poses)
        {
            if (poses.Count < 2)
            {
                return poses;
            }
            float max = float.NegativeInfinity;
            PoseResult apart = poses[0];
            PoseResult bpart = poses[1];
            float diameter = 0;
            foreach (PoseResult a in poses)
            {
                foreach (PoseResult b in poses)
                {
                    float dist = PoseResult.Difference(a, b);
                    diameter += dist;
                    if (dist > max)
                    {
                        max = dist;
                        apart = a;
                        bpart = b;
                    }
                }
            }
            diameter /= Mathf.Pow(poses.Count, 2);
            var clusterA = new List<PoseResult>(poses.Count);
            var clusterB = new List<PoseResult>(poses.Count);

            foreach (PoseResult p in poses)
            {
                if (PoseResult.Difference(p, apart) < PoseResult.Difference(p, bpart))
                {
                    clusterA.Add(p);
                    Debug.DrawRay(p.pose.position, p.pose.forward, Color.green);
                }
                else
                {
                    clusterB.Add(p);
                    Debug.DrawRay(p.pose.position, p.pose.forward, Color.red);
                }
            }

            float diamA = GetClusterDiameter(clusterA);
            float diamB = GetClusterDiameter(clusterB);

            // Heuristic!
            // If the clusters are not more "clustered" than the entire data set, just return everything.
            if ((diamA + diamB) * 2  > diameter)
            {
                return poses;
            }

            if (diamA < diamB)
            {
                return clusterA;
            }
            else
            {
                return clusterB;
            }
        }

        private static float GetClusterDiameter(List<PoseResult> poses)
        {
            float sum = 0;
            foreach (PoseResult a in poses)
            {
                foreach (PoseResult b in poses)
                {
                    sum += PoseResult.Difference(a, b);
                }
            }
            return sum / Mathf.Pow(poses.Count, 2);
        }

        private static bool ContainsMalformedCorner(Vector2 ul, Vector2 ur, Vector2 lr, Vector2 ll)
        {
            float maxDeviance = 30;
            if (Mathf.Abs(Vector2.Angle(ur - ul, ll - ul) - 90) > maxDeviance ||
                Mathf.Abs(Vector2.Angle(ul - ur, lr - ur) - 90) > maxDeviance ||
                Mathf.Abs(Vector2.Angle(ur - lr, ll - lr) - 90) > maxDeviance ||
                Mathf.Abs(Vector2.Angle(ul - ll, lr - ll) - 90) > maxDeviance)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Gets the closest sample to the target point. Return null if there is no point closer than <paramref name="max"/>.
        /// </summary>
        /// <returns>The closest sample.</returns>
        /// <param name="target">Target point.</param>
        /// <param name="samples">Samples.</param>
        /// <param name="max">Maximum distance.</param>
        private static Sample GetClosestSample(Point target, List<Sample> samples, float max)
        {
            double minDistance = float.PositiveInfinity;
            Sample c1 = null;
            foreach (Sample sample in samples)
            {
                double distance = Point.Distance(target, sample.param);
                if (distance < minDistance)
                {
                    c1 = sample;
                    minDistance = distance;
                }
            }
            if (minDistance > max)
            {
                return null;
            }
            return c1;
        }

        /// <summary>
        /// Converts a triangle into a Pose.
        /// </summary>
        /// <returns>The pose.</returns>
        /// <param name="centerVec">Center vertex in world space</param>
        /// <param name="leftVec">Left vertex in world space</param>
        /// <param name="rightVec">Right vertex in world space</param>
        /// <param name="centerSource">Center vertex in parameter space</param>
        /// <param name="leftSource">Left vertex in parameter space.</param>
        /// <param name="rightSource">Right vertex in parameter space</param>
        /// <param name="codeWidth">The width of the QR code</param>
        private static PoseResult CalcPose(Vector3 centerVec, Vector3 leftVec, Vector3 rightVec, Vector2 centerSource, Vector2 leftSource, Vector2 rightSource, Vector2 oppositeSource, float codeWidth)
        {
            // Convert the center, the top center and all four corners of the code into barycentric coordinates based on the sample points.
            Vector3 origin = Barycentric(new Vector2(0, 0), centerSource, leftSource, rightSource);
            Vector3 top = Barycentric(new Vector2(-1, 0), centerSource, leftSource, rightSource); // I think it shoul be (0,-1), but like this is works better
            Vector3 ul = Barycentric(new Vector2(-codeWidth / 2, -codeWidth / 2), centerSource, leftSource, rightSource);
            Vector3 ur = Barycentric(new Vector2(codeWidth / 2, -codeWidth / 2), centerSource, leftSource, rightSource);
            Vector3 lr = Barycentric(new Vector2(codeWidth / 2, codeWidth / 2), centerSource, leftSource, rightSource);
            Vector3 ll = Barycentric(new Vector2(-codeWidth / 2, codeWidth / 2), centerSource, leftSource, rightSource);

            // Calculate the poisition and orientation of the Pose
            Vector3 position = origin.x * centerVec + origin.y * leftVec + origin.z * rightVec;
            Vector3 forward = -Vector3.Cross(leftVec - centerVec, rightVec - centerVec).normalized;
            Vector3 up = (top.x * centerVec + top.y * leftVec + top.z * rightVec - position).normalized;
            Quaternion rotation = Quaternion.LookRotation(
                up,
                forward
            );

            Pose pose = new Pose(position, rotation);

            // Calculate the estimated stability of this Pose.
            // As a heuristic we assume, that a bigger (larger area) quads yields more reliable data.
            float weight =
                (leftSource.x - centerSource.x) * (rightSource.y - centerSource.y) -
                (rightSource.x - centerSource.x) * (leftSource.y - centerSource.y)
                +
                (rightSource.x - oppositeSource.x) * (leftSource.y - oppositeSource.y) -
                (leftSource.x - oppositeSource.x) * (rightSource.y - oppositeSource.y);

            return new PoseResult
            {
                pose = pose,
                corners = new Vector3[]
                {
                    ul.x * centerVec + ul.y * leftVec + ul.z * rightVec,
                    ur.x * centerVec + ur.y * leftVec + ur.z * rightVec,
                    lr.x * centerVec + lr.y * leftVec + lr.z * rightVec,
                    ll.x * centerVec + ll.y * leftVec + ll.z * rightVec
                },
                weight = weight
            };
        }

        /// <summary>
        /// Applies an offset to the pose, i.e. shifts the pose to another pose, which encoded relative to it.
        /// </summary>
        /// <returns>The new pose with the offset applied.</returns>
        /// <param name="pose">Pose.</param>
        /// <param name="offset">Offset.</param>
        private static PoseResult ApplyOffset(PoseResult pose, Pose offset)
        {
            if (offset.Equals(Pose.identity))
            {
                return pose;
            }
            PoseResult res = new PoseResult();
            res.pose = new Pose(pose.pose.position + pose.pose.rotation * offset.position, pose.pose.rotation * offset.rotation);
            // Hack
            res.corners = new Vector3[]
            {
                res.pose.position,
                res.pose.position,
                res.pose.position,
                res.pose.position
            };
            res.weight = pose.weight;
            return res;
        }

        /// <summary>
        /// Compute barycentric coordinates (u, v, w) for point <paramref name="p"/> with respect to triangle (<paramref name="a"/>, <paramref name="b"/>, <paramref name="c"/>)
        /// </summary>
        /// <returns>The barycentric coordinates of <paramref name="p"/></returns>
        /// <param name="p">The point to be converted</param>
        /// <param name="a">Triangle vertex</param>
        /// <param name="b">Triangle vertex</param>
        /// <param name="c">Triangle vertex</param>
        private static Vector3 Barycentric(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
        {
            Vector2 v0 = b - a;
            Vector2 v1 = c - a;
            Vector2 v2 = p - a;

            float d00 = Vector2.Dot(v0, v0);
            float d01 = Vector2.Dot(v0, v1);
            float d11 = Vector2.Dot(v1, v1);
            float d20 = Vector2.Dot(v2, v0);
            float d21 = Vector2.Dot(v2, v1);
            float denom = d00 * d11 - d01 * d01;

            float v = (d11 * d20 - d01 * d21) / denom;
            float w = (d00 * d21 - d01 * d20) / denom;
            float u = 1.0f - v - w;

            return new Vector3(u, v, w);
        }

        /// <summary>
        /// Compuses the closest intersection between a ray and a sphere.
        /// </summary>
        /// <returns>Distance along the ray or float.PositiveInfinity if no intersection was found.</returns>
        /// <param name="p">Origin of the ray.</param>
        /// <param name="d">Direction of the ray.</param>
        /// <param name="s">Center of the sphere.</param>
        /// <param name="r">Radius of the sphere.</param>
        private static float IntersectRaySphere(Vector3 p, Vector3 d, Vector3 s, float r, bool near)
        {
            Vector3 m = p - s;
            float b = Vector3.Dot(m, d);
            float c = Vector3.Dot(m, m) - r * r;

            // Exit if r’s origin outside s (c > 0) and r pointing away from s (b > 0).
            if (c > 0.0f && b > 0.0f) return float.PositiveInfinity;
            float discr = b * b - c;

            // A negative discriminant corresponds to ray missing sphere.
            if (discr < 0.0f) return float.PositiveInfinity;

            // Ray now found to intersect sphere, compute smallest t value of intersection.
            float t;
            if (near)
                t = -b - Mathf.Sqrt(discr);
            else
                t = -b + Mathf.Sqrt(discr);
                
            // If t is negative, ray started inside sphere so clamp t to zero.
            if (t < 0.0f) return 0;

            return t;
        }
    }
}