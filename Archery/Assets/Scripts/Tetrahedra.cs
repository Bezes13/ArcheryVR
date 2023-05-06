using System;
using UnityEngine;

public class Tetrahedra
{
    public Vector3 a;
    public Vector3 b;
    public Vector3 c;
    public Vector3 d;

    private readonly Triangle abc;
    private readonly Triangle bcd;
    private readonly Triangle cda;
    private readonly Triangle dab;

    public Tetrahedra(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        this.a = a;
        this.b = b;
        this.c = c;
        this.d = d;
        abc = new Triangle(a, b, c);
        bcd = new Triangle(b, c, d);
        cda = new Triangle(c, d, a);
        dab = new Triangle(d, a, b);
    }
    public bool Contains(Vector3 p, bool includeOnFacet) {
        // if Point is on the inner side of each face, then it is in the tetraeder
        var f1 = abc.IsSameSide(d, p, includeOnFacet);
        var f2 = bcd.IsSameSide(a, p, includeOnFacet);
        var f3 = cda.IsSameSide(b, p, includeOnFacet);
        var f4 = dab.IsSameSide(c, p, includeOnFacet);
        return f1 && f2 && f3 && f4;
    }
        
    public Sphere GetCircumscribedSphere() {
        var a2 = new Vector3(a.x * a.x, a.y * a.y, a.z * a.z); 
        var a2Ex = a2.x + a2.y + a2.z; 
        var b2 = new Vector3(b.x * b.x, b.y * b.y, b.z * b.z); 
        var b2Ex = b2.x + b2.y + b2.z;
        var c2 = new Vector3(c.x * c.x, c.y * c.y, c.z * c.z); 
        var c2Ex = c2.x + c2.y + c2.z;
        var d2 = new Vector3(d.x * d.x, d.y * d.y, d.z * d.z); 
        var d2Ex = d2.x + d2.y + d2.z;
        var detA = new Matrix4x4(
            new Vector4(a.x, a.y, a.z, 1),
            new Vector4(b.x, b.y, b.z, 1),
            new Vector4(c.x, c.y, c.z, 1),
            new Vector4(d.x, d.y, d.z, 1)).determinant;
        var detX = new Matrix4x4(
            new Vector4( a2Ex, a.y, a.z, 1),
            new Vector4( b2Ex, b.y, b.z, 1),
            new Vector4(c2Ex, c.y, c.z, 1),
            new Vector4( d2Ex, d.y, d.z, 1)).determinant;
        var detY = -new Matrix4x4(
            new Vector4(a2Ex, a.x, a.z, 1),
            new Vector4( b2Ex, b.x, b.z, 1), 
            new Vector4( c2Ex, c.x, c.z, 1), 
            new Vector4( d2Ex, d.x, d.z, 1)).determinant;
        var detZ = new Matrix4x4(
            new Vector4( a2Ex, a.x, a.y, 1),
            new Vector4( b2Ex, b.x, b.y, 1),
            new Vector4( c2Ex, c.x, c.y, 1),
            new Vector4( d2Ex, d.x, d.y, 1)).determinant;
        var center = new Vector3(detX, detY, detZ) / (2 * detA);
        return new Sphere(center, Vector3.Distance(center, a));
    }
        
    public Vector3 RemainingPoint(Triangle t) {
        if (t.Equals(abc))
        {
            return d;
        }
        if (t.Equals(bcd))
        {
            return a;
        }
        if (t.Equals(cda))
        {
            return b;
        }
        if (t.Equals(dab))
        {
            return c;
        }
        throw new Exception();
    }
        
    public bool ContainsFace(Triangle t)
    {
        return t.Equals(abc) || t.Equals(bcd) || t.Equals( cda) || t.Equals(dab);
    }

}