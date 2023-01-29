using System;
using UnityEngine;

namespace Mentum.HexMatch
{
    [System.Serializable]
    public class Point : IEquatable<Point>
    {
        public int x;
        public int y;

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Point(Point clone)
        {
            x = clone.x;
            y = clone.y;
        }

        public void Add(Point p)
        {
            x += p.x;
            y += p.y;
        }

        #region Operator Overloads
        public static Point operator +(Point a, Point b)
        {
            return new Point(a.x + b.x, a.y + b.y);
        }
        public static Point operator *(Point a, int mult)
        {
            return new Point(a.x * mult, a.y * mult);
        }

        public static Point operator -(Point p1, Point p2)
        {
            return new Point(p1.x - p2.x, p1.y - p2.y);
        }

        public static bool operator ==(Point a, Point b)
        {
            if (Equals(a, null) || Equals(b, null))
                return Equals(a, null) && Equals(b, null);

            return a.x == b.x && a.y == b.y;
        }

        public static bool operator !=(Point a, Point b)
        {
            return !(a == b);
        }

        public static implicit operator Vector2(Point p)
        {
            return new Vector2(p.x, p.y);
        }
        #endregion


        #region Object Overloads
        public override bool Equals(object obj)
        {
            if (obj is Point p)
                return x == p.x && y == p.y;

            return false;
        }

        public bool Equals(Point p)
        {
            return x == p.x && y == p.y;
        }

        public override int GetHashCode()
        {
            return x ^ y;
        }

        public override string ToString()
        {
            return string.Format("({0},{1})", x, y);
        }
        #endregion


        /// <summary>
        /// X 값당 x += 1f, y += 0.75f
        /// Y 값당 x -= 1f, y += 0.75f
        /// </summary>
        public static Vector2 PointToAnchorPosition(int x, int y)
        {
            float pos_x = St.OFFSET_X * (x - y);
            float pos_y = St.OFFSET_Y * (x + y);

            return new Vector2(pos_x * St.CELL, pos_y * St.CELL);
        }

        public Vector2 GetAnchorPosition()
        {
            return PointToAnchorPosition(x, y);
        }

        //public Vector2 ToVector() => new(x, y);

        //public static Point FromVector(Vector2 v)
        //    => new((int)v.x, (int)v.y);

        //public static Point FromVector(Vector3 v)
        //    => new((int)v.x, (int)v.y);

        //public static Point Mult(Point p, int m)
        //    => new(p.x * m, p.y * m);

        //public static Point Add(Point p, Point o)
        //    => new(p.x + o.x, p.y + o.y);

        //public static Point Clone(Point p)
        //    => new(p.x, p.y);

        public static Point Zero => new(0, 0);
        public static Point One => new(1, 1);

        public static Point Up => new(1, 1);
        public static Point Down => new(-1, -1);

        public static Point RightUp => new(1, 0);
        public static Point LeftUp => new(0, 1);

        public static Point RightDown => new(0, -1);
        public static Point LeftDown => new(-1, 0);
    }
}