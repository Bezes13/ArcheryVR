using UnityEngine;

namespace Voronoi
{
    /// <summary>
    /// Represents a Sphere, is used to check if a arbitrary point is in a Sphere.
    /// </summary>
    public struct Sphere
    {
        public Vector3 center;
        private readonly double _radius;

        public Sphere(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
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
            
            center = new Vector3(detX, detY, detZ) / (2 * detA);
            _radius = Vector3.Distance(center, a);
        }

        public bool Contains(Vector3 p)
        {
            return Vector3.Distance(center, p) <= _radius;
        }
    }
}