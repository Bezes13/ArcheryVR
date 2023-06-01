using System;
using UnityEngine;

namespace Voronoi
{
    /// <summary>
    /// Represents a Triangle 
    /// </summary>
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

        public bool CheckLineTriangleIntersection(Vector3 linePoint, Vector3 lineDirection, out Vector3 intersectionPoint)
        {
            Vector3 lineDirectionNormalized = lineDirection.normalized;

            // Check if direction and normal are parallel
            if (Math.Abs(Vector3.Dot(_n, lineDirectionNormalized)) < 0.0001f)
            {
                intersectionPoint = default;
                return false;
            }
            // Calculate intersectionPoint
            var t = Vector3.Dot(_n, a - linePoint) / Vector3.Dot(_n, lineDirectionNormalized);

            // Check if intersection point is in triangle
            intersectionPoint = linePoint + lineDirectionNormalized * t;
            return IsPointInsideTriangle(intersectionPoint, a, b, c);
        }
        
        static bool IsPointInsideTriangle(Vector3 point, Vector3 a, Vector3 b, Vector3 c)
        {
            var normal1 = Vector3.Cross(b - a, point - a);
            var normal2 = Vector3.Cross(c - b, point - b);
            var normal3 = Vector3.Cross(a - c, point - c);
            
            return Vector3.Dot(normal1, normal2) >= 0 && Vector3.Dot(normal1, normal3) >= 0;
        }

        public bool InSide(Vector3 p1, Vector3 p2)
        {
            double d = Vector3.Dot(_n, p1 - a) * Vector3.Dot(_n, p2 - a);
            return d >= 0;
        }
    }
}