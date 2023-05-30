using UnityEngine;

namespace Voronoi
{
    public struct Sphere
    {
        public Vector3 center;
        private readonly double _radius;

        public Sphere(Vector3 c, double r)
        {
            center = c;
            _radius = r;
        }

        public bool Contains(Vector3 p)
        {
            return Vector3.Distance(center, p) <= _radius;
        }
    }
}