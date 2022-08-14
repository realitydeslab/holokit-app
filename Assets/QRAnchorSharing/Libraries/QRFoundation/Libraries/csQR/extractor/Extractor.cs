using System;
using ZXing.Common;

// Largely a C# port of https://github.com/cozmo/jsQR under Apache-2.0 License
namespace CSQR
{
    public class PerspectiveTransform
    {
        public double a11;
        public double a12;
        public double a13;
        public double a21;
        public double a22;
        public double a23;
        public double a31;
        public double a32;
        public double a33;

        public PerspectiveTransform(double a11, double a12, double a13, double a21, double a22, double a23, double a31, double a32, double a33)
        {
            this.a11 = a11;
            this.a12 = a12;
            this.a13 = a13;
            this.a21 = a21;
            this.a22 = a22;
            this.a23 = a23;
            this.a31 = a31;
            this.a32 = a32;
            this.a33 = a33;
        }
    }

    public class Extractor
    {
        PerspectiveTransform SquareToQuadrilateral(Point p1, Point p2, Point p3, Point p4)
        {
            double dx3 = p1.x - p2.x + p3.x - p4.x;
            double dy3 = p1.y - p2.y + p3.y - p4.y;
            if (dx3 == 0 && dy3 == 0)
            { // Affine
                return new PerspectiveTransform(
                    p2.x - p1.x,
                    p2.y - p1.y,
                    0,
                    p3.x - p2.x,
                    p3.y - p2.y,
                    0,
                    p1.x,
                    p1.y,
                    1);
            }
            else
            {
                double dx1 = p2.x - p3.x;
                double dx2 = p4.x - p3.x;
                double dy1 = p2.y - p3.y;
                double dy2 = p4.y - p3.y;
                double denominator = dx1 * dy2 - dx2 * dy1;
                double a13 = (dx3 * dy2 - dx2 * dy3) / denominator;
                double a23 = (dx1 * dy3 - dx3 * dy1) / denominator;
                return new PerspectiveTransform(
                    p2.x - p1.x + a13 * p2.x,
                    p2.y - p1.y + a13 * p2.y,
                    a13,
                    p4.x - p1.x + a23 * p4.x,
                    p4.y - p1.y + a23 * p4.y,
                    a23,
                    p1.x,
                    p1.y,
                    1);
            }
        }

        PerspectiveTransform QuadrilateralToSquare(Point p1, Point p2, Point p3, Point p4)
        {
            PerspectiveTransform sToQ = SquareToQuadrilateral(p1, p2, p3, p4);
            return new PerspectiveTransform(
                sToQ.a22 * sToQ.a33 - sToQ.a23 * sToQ.a32,
                sToQ.a13 * sToQ.a32 - sToQ.a12 * sToQ.a33,
                sToQ.a12 * sToQ.a23 - sToQ.a13 * sToQ.a22,
                sToQ.a23 * sToQ.a31 - sToQ.a21 * sToQ.a33,
                sToQ.a11 * sToQ.a33 - sToQ.a13 * sToQ.a31,
                sToQ.a13 * sToQ.a21 - sToQ.a11 * sToQ.a23,
                sToQ.a21 * sToQ.a32 - sToQ.a22 * sToQ.a31,
                sToQ.a12 * sToQ.a31 - sToQ.a11 * sToQ.a32,
                sToQ.a11 * sToQ.a22 - sToQ.a12 * sToQ.a21
            );
        }

        PerspectiveTransform Times(PerspectiveTransform a, PerspectiveTransform b)
        {
            return new PerspectiveTransform(
                a.a11 * b.a11 + a.a21 * b.a12 + a.a31 * b.a13,
                a.a12 * b.a11 + a.a22 * b.a12 + a.a32 * b.a13,
                a.a13 * b.a11 + a.a23 * b.a12 + a.a33 * b.a13,
                a.a11 * b.a21 + a.a21 * b.a22 + a.a31 * b.a23,
                a.a12 * b.a21 + a.a22 * b.a22 + a.a32 * b.a23,
                a.a13 * b.a21 + a.a23 * b.a22 + a.a33 * b.a23,
                a.a11 * b.a31 + a.a21 * b.a32 + a.a31 * b.a33,
                a.a12 * b.a31 + a.a22 * b.a32 + a.a32 * b.a33,
                a.a13 * b.a31 + a.a23 * b.a32 + a.a33 * b.a33
            );
        }

        public void Extract(BitMatrix image, QRLocation location, out BitMatrix outMatrix, out Func<Point, Point> outMappingFunction)
        {
            PerspectiveTransform qToS = QuadrilateralToSquare(
                new Point(3.5f, 3.5f),
                new Point(location.dimension - 3.5f, 3.5f),
                new Point(location.dimension - 6.5f, location.dimension - 6.5f),
                new Point(3.5f, location.dimension - 3.5f)
            );

            PerspectiveTransform sToQ = SquareToQuadrilateral(
                location.topLeft,
                location.topRight,
                location.alignmentPattern,
                location.bottomLeft
            );
            PerspectiveTransform transform = Times(sToQ, qToS);

            BitMatrix matrix = new BitMatrix(location.dimension);
            Func<Point, Point> mappingFunction = (Point v) =>
            {
                double denominator = transform.a13 * v.x + transform.a23 * v.y + transform.a33;
                return new Point(
                    (transform.a11 * v.x + transform.a21 * v.y + transform.a31) / denominator,
                    (transform.a12 * v.x + transform.a22 * v.y + transform.a32) / denominator
                );
            };

            for (int y = 0; y < location.dimension; y++)
            {
                for (int x = 0; x < location.dimension; x++)
                {
                    double xValue = x + 0.5f;
                    double yValue = y + 0.5f;
                    Point sourcePixel = mappingFunction(new Point(xValue, yValue));
                    matrix[x, y] = image[(int)sourcePixel.x, (int)sourcePixel.y];
                }
            }

            outMatrix = matrix;
            outMappingFunction = mappingFunction;
        }
    }
}