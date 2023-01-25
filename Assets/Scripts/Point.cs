using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Point
{
    public int x;
    public int y;

    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public void Mult(int m)
    {
        x *= m;
        y *= m;
    }

    public void Add(Point p)
    {
        x += p.x;
        y += p.y;
    }

    public Vector2 ToVector() 
        => new Vector2(x, y);

    public bool Equals(Point p) 
        => (x == p.x && y == p.y);


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

    public static Point FromVector(Vector2 v) 
        => new Point((int)v.x, (int)v.y);

    public static Point FromVector(Vector3 v) 
        => new Point((int)v.x, (int)v.y); 

    public static Point Mult(Point p, int m) 
        => new Point(p.x * m, p.y * m);

    public static Point Add(Point p, Point o) 
        => new Point(p.x + o.x, p.y + o.y);

    public static Point Clone(Point p) 
        => new Point(p.x, p.y);

    public static Point Zero => new Point(0, 0);
    public static Point One => new Point(1, 1);

    public static Point Up => new Point(1, 1);
    public static Point Down => new Point(-1, -1);

    public static Point RightUp => new Point(1, 0);
    public static Point LeftUp => new Point(0, 1);

    public static Point RightDown => new Point(0, -1);
    public static Point LeftDown => new Point(-1, 0);
}