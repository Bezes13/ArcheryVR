using UnityEngine;

public struct Sphere {
    public Vector3 center;
    private readonly double _radius;

    public Sphere(Vector3 c, double r) {
        center = c;
        _radius = r;
    }

    public bool Contains(Vector3 p, bool inclusive = true) {
        var length = Vector3.SqrMagnitude(p - center);
        if (inclusive)
        {
            return length <= _radius * _radius;
        }
        return length <  _radius * _radius;
    }
}