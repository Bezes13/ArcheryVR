using System;
using UnityEngine;

namespace Voronoi
{
    public class Tetrahedron
    {
        public Vector3 a;
        public Vector3 b;
        public Vector3 c;
        public Vector3 d;

        private readonly Triangle _abc;
        private readonly Triangle _bcd;
        private readonly Triangle _cda;
        private readonly Triangle _dab;

        public Tetrahedron(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
            _abc = new Triangle(a, b, c);
            _bcd = new Triangle(b, c, d);
            _cda = new Triangle(c, d, a);
            _dab = new Triangle(d, a, b);
        }

        public bool IsInside(Vector3 p)
        {
            // if Point is on the inner side of each face, then it is in the tetraeder
            var f1 = _abc.InSide(d, p);
            var f2 = _bcd.InSide(a, p);
            var f3 = _cda.InSide(b, p);
            var f4 = _dab.InSide(c, p);
            return f1 && f2 && f3 && f4;
        }

        public Sphere GetSphere()
        {
            var a2 = a.x * a.x + a.y * a.y + a.z * a.z;
            var b2 = b.x * b.x + b.y * b.y + b.z * b.z;
            var c2 = c.x * c.x + c.y * c.y + c.z * c.z;
            var d2 = d.x * d.x + d.y * d.y + d.z * d.z;
            var detA = new Matrix4x4(new Vector4(a.x, a.y, a.z, 1), new Vector4(b.x, b.y, b.z, 1),
                new Vector4(c.x, c.y, c.z, 1), new Vector4(d.x, d.y, d.z, 1)).determinant;

            var detX = new Matrix4x4(new Vector4(a2, a.y, a.z, 1), new Vector4(b2, b.y, b.z, 1),
                new Vector4(c2, c.y, c.z, 1), new Vector4(d2, d.y, d.z, 1)).determinant;

            var detY = -new Matrix4x4(new Vector4(a2, a.x, a.z, 1), new Vector4(b2, b.x, b.z, 1),
                new Vector4(c2, c.x, c.z, 1), new Vector4(d2, d.x, d.z, 1)).determinant;

            var detZ = new Matrix4x4(new Vector4(a2, a.x, a.y, 1), new Vector4(b2, b.x, b.y, 1), 
                new Vector4(c2, c.x, c.y, 1), new Vector4(d2, d.x, d.y, 1)).determinant;
            
            var center = new Vector3(detX, detY, detZ) / (2 * detA);
            var radius = Vector3.Distance(center, a);

            return new Sphere(center, radius);
        }

        public Vector3 RemainingPoint(Triangle t)
        {
            if (t.Equals(_bcd))
            {
                return a;
            }

            if (t.Equals(_cda))
            {
                return b;
            }

            if (t.Equals(_dab))
            {
                return c;
            }

            return d;
        }

        public Triangle RemainingTriangle(Vector3 t)
        {
            if (t.Equals(a))
            {
                return _bcd;
            }
            if (t.Equals(b))
            {
                return _cda;
            }
            if (t.Equals(c))
            {
                return _dab;
            }
           
            return _abc;
         
        }
        public bool ContainsFace(Triangle t)
        {
            return t.Equals(_abc) || t.Equals(_bcd) || t.Equals(_cda) || t.Equals(_dab);
        }
    }
}