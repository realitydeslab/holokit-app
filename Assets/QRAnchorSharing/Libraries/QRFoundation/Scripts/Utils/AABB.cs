using UnityEngine;

namespace QRFoundation
{
    public class AABB
    {
        public int x, y, x2, y2;

        public int w
        {
            get
            {
                return x2 - x;
            }
        }

        public int h
        {
            get
            {
                return y2 - y;
            }
        }

        public AABB()
        {
        }

        public AABB(int x, int y)
        {
            this.x = x;
            this.y = y;
            this.x2 = x;
            this.y2 = y;
        }

        public AABB(int x, int y, int x2, int y2)
        {
            this.x = x;
            this.y = y;
            this.x2 = x2;
            this.y2 = y2;
        }

        public AABB(AABB source)
        {
            this.x = source.x;
            this.y = source.y;
            this.x2 = source.x2;
            this.y2 = source.y2;
        }

        public void Encapsulate(int x, int y)
        {
            if (x < this.x)
            {
                this.x = x;
            }
            else if (x > this.x2)
            {
                this.x2 = x;
            }

            if (y < this.y)
            {
                this.y = y;
            }
            else if (y > this.y2)
            {
                this.y2 = y;
            }
        }

        public Rect ToRect()
        {
            return new Rect(this.x, this.y, this.w, this.h);
        }

        public static AABB operator /(AABB box, float div)
        {
            return new AABB(
                (int)(box.x / div),
                (int)(box.y / div),
                (int)(box.x2 / div),
                (int)(box.y2 / div)
            );
        }

        public static AABB operator *(AABB box, float factor)
        {
            return new AABB(
                (int)(box.x * factor),
                (int)(box.y * factor),
                (int)(box.x2 * factor),
                (int)(box.y2 * factor)
            );
        }

        public override string ToString()
        {
            return "x: " + x + ", y: " + y + ", w: " + w + ", h: " + h;
        }
    }
}
