using UnityEngine;

namespace Voronoi
{
    /// <summary>
    /// Represents a tetrahedron which is the main part of the delaunay algorithm.
    /// </summary>
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
            return new Sphere(a, b, c, d);
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