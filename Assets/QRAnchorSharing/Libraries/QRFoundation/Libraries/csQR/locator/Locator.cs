using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZXing.Common;

// Largely a C# port of https://github.com/cozmo/jsQR under Apache-2.0 License
namespace CSQR
{
    public class QRLocation
    {
        public Point topRight;
        public Point bottomLeft;
        public Point topLeft;
        public Point alignmentPattern;
        public int dimension;

        public QRLocation(Point topLeft, Point topRight, Point alignmentPattern, Point bottomLeft, int dimension)
        {
            this.topRight = topRight;
            this.topLeft = topLeft;
            this.bottomLeft = bottomLeft;
            this.alignmentPattern = alignmentPattern;
            this.dimension = dimension;
        }
    }

    public struct Point
    {
        public double x;
        public double y;

        public Point(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public static double Distance(Point a, Point b)
        {
            return Math.Sqrt(Math.Pow(a.x - b.x, 2) + Math.Pow(a.y - b.y, 2));
        }

        public override string ToString()
        {
            return "x: " + x + ", y: " + y;
        }
    }

    public class Locator
    {
        const int MAX_FINDERPATTERNS_TO_SEARCH = 4;
        const double MIN_QUAD_RATIO = 0.5d;
        const double MAX_QUAD_RATIO = 1.5d;

        public static double Sum(double[] values)
        {
            double sum = 0;
            foreach (double v in values)
            {
                sum += v;
            }
            return sum;
        }

        public static int Sum(int[] values)
        {
            int sum = 0;
            foreach (int v in values)
            {
                sum += v;
            }
            return sum;
        }

        // Takes three finder patterns and organizes them into topLeft, topRight, etc
        public static Point[] ReorderFinderPatterns(Point pattern1, Point pattern2, Point pattern3)
        {
            // Find distances between pattern centers
            double oneTwoDistance = Point.Distance(pattern1, pattern2);
            double twoThreeDistance = Point.Distance(pattern2, pattern3);
            double oneThreeDistance = Point.Distance(pattern1, pattern3);

            Point bottomLeft;
            Point topLeft;
            Point topRight;

            // Assume one closest to other two is B; A and C will just be guesses at first
            if (twoThreeDistance >= oneTwoDistance && twoThreeDistance >= oneThreeDistance)
            {
                bottomLeft = pattern2;
                topLeft = pattern1;
                topRight = pattern3;
            }
            else if (oneThreeDistance >= twoThreeDistance && oneThreeDistance >= oneTwoDistance)
            {
                bottomLeft = pattern1;
                topLeft = pattern2;
                topRight = pattern3;
            }
            else
            {
                bottomLeft = pattern1;
                topLeft = pattern3;
                topRight = pattern2;
            }

            // Use cross product to figure out whether bottomLeft (A) and topRight (C) are correct or flipped in relation to topLeft (B)
            // This asks whether BC x BA has a positive z component, which is the arrangement we want. If it's negative, then
            // we've got it flipped around and should swap topRight and bottomLeft.
            if (((topRight.x - topLeft.x) * (bottomLeft.y - topLeft.y)) - ((topRight.y - topLeft.y) * (bottomLeft.x - topLeft.x)) < 0)
            {
                Point temp = bottomLeft;
                bottomLeft = topRight;
                topRight = temp;
            }

            return new Point[] { bottomLeft, topLeft, topRight };
        }

        // Computes the dimension (number of modules on a side) of the QR Code based on the position of the finder patterns
        public static (int, double) ComputeDimension(Point topLeft, Point topRight, Point bottomLeft, BitMatrix matrix)
        {
            double moduleSize = (
                Sum(CountBlackWhiteRun(topLeft, bottomLeft, matrix, 5)) / 7 + // Divide by 7 since the ratio is 1:1:3:1:1
                Sum(CountBlackWhiteRun(topLeft, topRight, matrix, 5)) / 7 +
                Sum(CountBlackWhiteRun(bottomLeft, topLeft, matrix, 5)) / 7 +
                Sum(CountBlackWhiteRun(topRight, topLeft, matrix, 5)) / 7
            ) / 4; // !

            if (moduleSize < 1)
            {
                throw new Exception("Invalid module size");
            }

            int topDimension = (int)Math.Round(Point.Distance(topLeft, topRight) / moduleSize);
            int sideDimension = (int)Math.Round(Point.Distance(topLeft, bottomLeft) / moduleSize);
            int dimension = (topDimension + sideDimension) / 2 + 7;
            switch (dimension % 4)
            {
                case 0:
                    dimension++;
                    break;
                case 2:
                    dimension--;
                    break;
            }
            return (dimension, moduleSize);
        }

        // Takes an origin point and an end point and counts the sizes of the black white run from the origin towards the end point.
        // Returns an array of elements, representing the pixel size of the black white run.
        // Uses a variant of http://en.wikipedia.org/wiki/Bresenham's_line_algorithm
        public static List<double> CountBlackWhiteRunTowardsPoint(Point origin, Point end, BitMatrix matrix, int length)
        {
            List<Point> switchPoints = new List<Point> { new Point((int)origin.x, (int)origin.y) }; // !
            bool steep = Math.Abs(end.y - origin.y) > Math.Abs(end.x - origin.x);

            int fromX;
            int fromY;
            int toX;
            int toY;
            if (steep)
            {
                fromX = (int)origin.y;
                fromY = (int)origin.x;
                toX = (int)end.y;
                toY = (int)end.x;
            }
            else
            {
                fromX = (int)origin.x;
                fromY = (int)origin.y;
                toX = (int)end.x;
                toY = (int)end.y;
            }

            int dx = Math.Abs(toX - fromX);
            int dy = Math.Abs(toY - fromY);
            int error = -dx / 2;
            int xStep = fromX < toX ? 1 : -1;
            int yStep = fromY < toY ? 1 : -1;

            bool currentPixel = true;
            // Loop up until x == toX, but not beyond
            for (int x = fromX, y = fromY; x != toX + xStep; x += xStep)
            {
                // Does current pixel mean we have moved white to black or vice versa?
                // Scanning black in state 0,2 and white in state 1, so if we find the wrong
                // color, advance to next state or end if we are in state 2 already
                int realX = steep ? y : x;
                int realY = steep ? x : y;
                if (matrix[realX, realY] != currentPixel)
                {
                    currentPixel = !currentPixel;
                    switchPoints.Add(new Point(realX, realY));
                    if (switchPoints.Count == length + 1)
                    {
                        break;
                    }
                }
                error += dy;
                if (error > 0)
                {
                    if (y == toY)
                    {
                        break;
                    }
                    y += yStep;
                    error -= dx;
                }
            }
            List<double> distances = new List<double>(length);
            int entries = switchPoints.Count;
            for (int i = 0; i < length; i++)
            {
                if (i + 1 < entries)
                {
                    distances.Add(Point.Distance(switchPoints[i], switchPoints[i + 1]));
                }
                else
                {
                    distances.Add(0);
                }
            }

            return distances;
        }

        // Takes an origin point and an end point and counts the sizes of the black white run in the origin point
        // along the line that intersects with the end point. Returns an array of elements, representing the pixel sizes
        // of the black white run. Takes a length which represents the number of switches from black to white to look for.
        public static double[] CountBlackWhiteRun(Point origin, Point end, BitMatrix matrix, int length)
        {
            double rise = end.y - origin.y;
            double run = end.x - origin.x;

            List<double> towardsEnd = CountBlackWhiteRunTowardsPoint(origin, end, matrix, (int)Math.Ceiling(length / 2.0));
            List<double> awayFromEnd = CountBlackWhiteRunTowardsPoint(origin, new Point(origin.x - run, origin.y - rise), matrix, (int)Math.Ceiling(length / 2.0));

            double middleValue = towardsEnd[0] + awayFromEnd[0] - 1; // Substract one so we don't double count a pixel
            towardsEnd.RemoveAt(0);
            awayFromEnd.RemoveAt(0);
            awayFromEnd.Add(middleValue);
            awayFromEnd.AddRange(towardsEnd);
            return awayFromEnd.ToArray();
        }

        // Takes in a black white run and an array of expected ratios. Returns the average size of the run as well as the "error" -
        // that is the amount the run diverges from the expected ratio
        public static double[] ScoreBlackWhiteRun(double[] sequence, double[] ratios)
        {
            double averageSize = Sum(sequence) / Sum(ratios);
            double error = 0;
            for (int i = 0; i < ratios.Length; i++)
            {
                error += Math.Pow(sequence[i] - ratios[i] * averageSize, 2);
            }

            return new double[] { averageSize, error };
        }

        // Takes an X,Y point and an array of sizes and scores the point against those ratios.
        // For example for a finder pattern takes the ratio list of 1:1:3:1:1 and checks horizontal, vertical and diagonal ratios
        // against that.
        public static double ScorePattern(Point point, double[] ratios, BitMatrix matrix)
        {
            try
            {
                double[] horizontalRun = CountBlackWhiteRun(point, new Point(-1, point.y), matrix, ratios.Length);
                double[] verticalRun = CountBlackWhiteRun(point, new Point(point.x, -1), matrix, ratios.Length);

                Point topLeftPoint = new Point(
                    Math.Max(0, point.x - point.y) - 1,
                    Math.Max(0, point.y - point.x) - 1
                );
                double[] topLeftBottomRightRun = CountBlackWhiteRun(point, topLeftPoint, matrix, ratios.Length);

                Point bottomLeftPoint = new Point(
                    Math.Min(matrix.Width, point.x + point.y) + 1,
                    Math.Min(matrix.Height, point.y + point.x) + 1
                );
                double[] bottomLeftTopRightRun = CountBlackWhiteRun(point, bottomLeftPoint, matrix, ratios.Length);

                double[] horzError = ScoreBlackWhiteRun(horizontalRun, ratios);
                double[] vertError = ScoreBlackWhiteRun(verticalRun, ratios);
                double[] diagDownError = ScoreBlackWhiteRun(topLeftBottomRightRun, ratios);
                double[] diagUpError = ScoreBlackWhiteRun(bottomLeftTopRightRun, ratios);

                double ratioError = Math.Sqrt(horzError[1] * horzError[1] +
                  vertError[1] * vertError[1] +
                  diagDownError[1] * diagDownError[1] +
                  diagUpError[1] * diagUpError[1]);

                double avgSize = (horzError[0] + vertError[0] + diagDownError[0] + diagUpError[0]) / 4;

                double sizeError = (Math.Pow(horzError[0] - avgSize, 2) +
                  Math.Pow(vertError[0] - avgSize, 2) +
                  Math.Pow(diagDownError[0] - avgSize, 2) +
                  Math.Pow(diagUpError[0] - avgSize, 2)) / avgSize;
                return ratioError + sizeError;
            }
            catch
            {
                return double.PositiveInfinity;
            }
        }

        class Line
        {
            public int startX;
            public int endX;
            public int y;

            public Line(int startX, int endX, int y)
            {
                this.startX = startX;
                this.endX = endX;
                this.y = y;
            }
        }

        class Quad
        {
            public Line top;
            public Line bottom;

            public Quad(Line top, Line bottom)
            {
                this.top = top;
                this.bottom = bottom;
            }
        }

        public static Point EstimateAlignmentPattern(Point topLeft, Point topRight, Point bottomLeft, double moduleSize)
        {
            Point bottomRightFinderPattern = new Point(
                topRight.x - topLeft.x + bottomLeft.x,
                topRight.y - topLeft.y + bottomLeft.y
            );
            double modulesBetweenFinderPatterns = (Point.Distance(topLeft, bottomLeft) + Point.Distance(topLeft, topRight)) / 2.0 / moduleSize;
            double correctionToTopLeft = 1 - (3 / modulesBetweenFinderPatterns);
            return new Point(
                topLeft.x + correctionToTopLeft * (bottomRightFinderPattern.x - topLeft.x),
                topLeft.y + correctionToTopLeft * (bottomRightFinderPattern.y - topLeft.y)
            );
        }

        class FinderPatternGroup
        {
            public List<FinderPattern> points;
            public double score;

            public FinderPatternGroup(List<FinderPattern> points, double score)
            {
                this.points = points;
                this.score = score;
            }
        }

        class FinderPattern
        {
            public double score;
            public double x;
            public double y;
            public double size;

            public FinderPattern(double x, double y, double score, double size)
            {
                this.x = x;
                this.y = y;
                this.score = score;
                this.size = size;
            }
        }
    }

    static class Extensions
    {
        public static List<T> Sorted<T>(this List<T> list, Comparison<T> comparison)
        {
            List<T> copy = new List<T>(list);
            copy.Sort(comparison);
            return copy;
        }

        public static T Min<T>(this List<T> list, Comparison<T> comparison)
        {
            if (list.Count == 0)
            {
                return default;
            }
            T res = list[0];
            foreach (T i in list)
            {
                if (comparison(i, res) < 0)
                {
                    res = i;
                }
            }
            return res;
        }

        public static List<T> Snapshot<T>(this List<T> list, out List<T> snapshot)
        {
            snapshot = new List<T>(list);
            return list;
        }
    }

    public static class LinkedListExtensions
    {
        public static T Find<T>(this LinkedList<T> items, Func<T, bool> match)
        {
            foreach (T item in items)
            {
                if (match(item))
                {
                    return item;
                }
            }
            return default;
        }

        public static LinkedList<T> FindAll<T>(this LinkedList<T> items, Func<T, bool> match)
        {
            LinkedList<T> res = new LinkedList<T>();
            foreach (T item in items)
            {
                if (match(item))
                {
                    res.AddLast(item);
                }
            }
            return res;
        }

        public static void DeleteAll<T>(this LinkedList<T> items, Func<T, bool> match)
        {
            /*foreach (T item in items)
            {
                if (match(item))
                {
                    items.Remove(item);
                }
            }*/
            for (LinkedListNode<T> it = items.First; it != null;)
            {
                LinkedListNode<T> next = it.Next;
                if (match(it.Value))
                    items.Remove(it);
                it = next;
            }
        }

        public static void ForEach<T>(this LinkedList<T> items, Action<T> action)
        {
            foreach (T item in items)
            {
                action(item);
            }
        }

        public static void AppendRange<T>(this LinkedList<T> source,
                                          IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                source.AddLast(item);
            }
        }

        public static void PrependRange<T>(this LinkedList<T> source,
                                           IEnumerable<T> items)
        {
            LinkedListNode<T> first = source.First;
            foreach (T item in items)
            {
                source.AddBefore(first, item);
            }
        }
    }
}