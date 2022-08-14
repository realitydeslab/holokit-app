using System;
using System.Collections.Generic;
using ZXing.Common;

// Largely a C# port of https://github.com/cozmo/jsQR under Apache-2.0 License
namespace CSQR
{
    public class Sample
    {
        /// <summary>
        /// Location of point in image space
        /// </summary>
        public Point image;
        /// <summary>
        /// Location of point in parameter space
        /// </summary>
        public Point param;

        public Sample(Point image, Point param)
        {
            this.image = image;
            this.param = param;
        }
    }

    public class Sampler
    {
        private readonly int iterations = 6;

        private BitMatrix image;
        private BitMatrix param;
        private Func<Point, Point> mapping;

        private int qrWidth;

        public Sampler(BitMatrix image, BitMatrix param, Func<Point, Point> mapping)
        {
            this.image = image;
            this.param = param;
            this.mapping = mapping;

            this.qrWidth = param.Width;
        }

        /// <summary>
        /// Search for squares which have no horizontal or vertical neighbors of the
        /// same color. Then try to approach their center as close as possible.
        /// </summary>
        /// <returns>The samples.</returns>
        /// <param name="max">The maximum number of sampels to look for.</param>
        public Sample[] GetSamples(int max)
        {
            List<Sample> ulRes = new List<Sample>(max / 4);
            List<Sample> urRes = new List<Sample>(max / 4);
            List<Sample> lrRes = new List<Sample>(max / 4);
            List<Sample> llRes = new List<Sample>(max / 4);

            // First, try to sample the finder patterns and the alignment pattern.
            TrySample(3, 3, 2, ulRes);
            TrySample(qrWidth - 4, 3, 2, llRes);
            TrySample(3, qrWidth - 4, 2, urRes);
            TrySample(qrWidth - 7, qrWidth - 7, 2, lrRes);

            // "Spiral" from outside
            string s = "";
            for (int x = 0; x < qrWidth / 2; x++)
            {
                s += "\n";
                for (int y = 0; y < qrWidth / 2; y++)
                {
                    s += param[x, y] ? "█ " : "□ ";
                    TrySample(x, y, 1, ulRes);
                    TrySample(qrWidth - x - 1, y, 1, llRes);
                    TrySample(x, qrWidth - y - 1, 1, urRes);
                    TrySample(qrWidth - x - 1, qrWidth - y - 1, 1, lrRes);
                }
            }
            //UnityEngine.Debug.Log(s);

            List<Sample> res = new List<Sample>(max);
            res.AddRange(ulRes);
            res.AddRange(urRes);
            res.AddRange(lrRes);
            res.AddRange(llRes);

            return res.ToArray();
        }

        /// <summary>
        /// Tests if the square fulfills the requirements to become a sample, and if
        /// to, center it and add it to the set of samples.
        /// </summary>
        /// <returns><c>true</c> if the square was successfully sampled.</returns>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="width">The direction in which to look for neighbors.</param>
        /// <param name="samples">The list of samples.</param>
        private bool TrySample(int x, int y, int width, List<Sample> samples)
        {
            if (samples.Count >= samples.Capacity)
            {
                return false;
            }

            // All neighbors horizontal and vertical neighbors must have a different color
            bool value = param[x, y];
            if (
                /*            param[x - width, y-width) == value ||
                            param[x + width, y -width) == value ||
                            param[x + width, y + width) == value ||
                            param[x - width, y + width) == value ||*/
                y - width >= 0 ? param[x, y - width] : false == value ||
                x + width < param.Width ? param[x + width, y] : false == value ||
                y + width < param.Height ? param[x, y + width] : false == value ||
                x - width >= 0 ? param[x - width, y] : false == value
            )
            {
                return false;
            }

            Point pi = mapping(new Point(x + 0.5, y + 0.5));
            Point po = mapping(new Point(x + width + 0.5, y + width + 0.5));
            int distance = (int)Point.Distance(pi, po);
            int xi = (int)pi.x;
            int yi = (int)pi.y;
            int xr = (int)pi.x;
            int yr = (int)pi.y;

            value = image[xr, yr];
            // Try to approach the center of the square iteratively.
            // Each step, march in all directions until the square is left and move to the thereby found center point.
            for (int i = 0; i < iterations; i++)
            {
                int xmin, xmax, ymin, ymax;

                for (xmin = xr; xmin > xr - distance; xmin--)
                {
                    if (image[xmin, yi] != value)
                    {
                        break;
                    }
                }
                for (xmax = xr; xmax < xr + distance; xmax++)
                {
                    if (image[xmax, yi] != value)
                    {
                        break;
                    }
                }
                for (ymin = yr; ymin > yr - distance; ymin--)
                {
                    if (image[xi, ymin] != value)
                    {
                        break;
                    }
                }
                for (ymax = yr; ymax < yr + distance; ymax++)
                {
                    if (image[xi, ymax] != value)
                    {
                        break;
                    }
                }

                xr = (xmin + xmax) / 2;
                yr = (ymin + ymax) / 2;
            }

            samples.Add(new Sample(
                new Point(xr, yr),
                new Point(x + 0.5, y + 0.5)
            ));
            return true;
        }
    }
}