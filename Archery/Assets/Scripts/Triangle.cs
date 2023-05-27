using System;
using UnityEngine;

public class Triangle
{
    public Vector3 a;
    public Vector3 b;
    public Vector3 c;
    private readonly Vector3 _n;

    public Triangle(Vector3 a, Vector3 b, Vector3 c)
    {
        this.a = a;
        this.b = b;
        this.c = c;
        _n = Vector3.Normalize(Vector3.Cross(b - a, c - a));
    }

    public Line Remaining(Vector3 p)
    {
        if (p.Equals(a))
        {
            return new Line(b, c);
        }

        if (p.Equals(b))
        {
            return new Line(c, a);
        }

        if (p.Equals(c))
        {
            return new Line(a, b);
        }

        throw new Exception();
    }

    public bool Equals(Triangle t)
    {
        return Equals(t.a, a) && Equals(t.b, b) && Equals(t.c, c) ||
               Equals(t.b, a) && Equals(t.c, b) && Equals(t.a, c) ||
               Equals(t.c, a) && Equals(t.a, b) && Equals(t.b, c) ||
               Equals(t.a, a) && Equals(t.c, b) && Equals(t.b, c) ||
               Equals(t.b, a) && Equals(t.a, b) && Equals(t.c, c) ||
               Equals(t.c, a) && Equals(t.b, b) && Equals(t.a, c);
    }

    public Triangle(Line line, Vector3 c)
    {
        a = line.a;
        b = line.b;
        this.c = c;
    }

    public bool Intersects(Line e, out Vector3 p)
    {
        if (!CramersRule(e.a, Vector3.Normalize(e.b - e.a), out var d, out p))
        {
            return false;
        }

        var f1 = d.x is >= 0 and <= 1 && d.y is >= 0 and <= 1 && d.x + d.y <= 1;
        var f2 = d.z >= 0 && d.z <= Vector3.Magnitude(e.b - e.a);
        return f1 && f2;
    }

    public bool SameSide(Vector3 p1, Vector3 p2)
    {
        double d = Vector3.Dot(_n, p1 - a) * Vector3.Dot(_n, p2 - a);
        return d >= 0;
    }

    bool CramersRule(Vector3 ogn, Vector3 ray, out Vector3 det, out Vector3 pos)
    {
        var e1 = b - a;
        var e2 = c - a;
        var denominator = new Matrix4x4(new Vector4(e1.x, e1.y, e1.z, 1), new Vector4(e2.x, e2.y, e2.z, 1),
            new Vector4(-ray.x, -ray.y, -ray.z, 1), new Vector4(0, 0, 0, 1)).determinant;
        if (Math.Abs(denominator) < Delaunay.Threshold)
        {
            det = default;
            pos = default;
            return false;
        }

        var d = ogn - a;
        var u = new Matrix4x4(new Vector4(d.x, d.y, d.z, 1), new Vector4(e2.x, e2.y, e2.z, 1),
            new Vector4(-ray.x, -ray.y, -ray.z, 1), new Vector4(0, 0, 0, 1)).determinant / denominator;
        var v = new Matrix4x4(new Vector4(e1.x, e1.y, e1.z, 1), new Vector4(d.x, d.y, d.z, 1),
            new Vector4(-ray.x, -ray.y, -ray.z, 1), new Vector4(0, 0, 0, 1)).determinant / denominator;
        var t = new Matrix4x4(new Vector4(e1.x, e1.y, e1.z, 1), new Vector4(e2.x, e2.y, e2.z, 1),
            new Vector4(d.x, d.y, d.z, 1), new Vector4(0, 0, 0, 1)).determinant / denominator;
        pos = ogn + ray * t;
        det = new Vector3(u, v, t);
        return true;
    }
}