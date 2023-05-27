using UnityEngine;

public class Line
{
    public Vector3 a;
    public Vector3 b;

    public Line(Vector3 a, Vector3 b)
    {
        this.a = a;
        this.b = b;
    }
    
    public static bool IsIntersecting(Line e1, Line e2)
    {
        var v1 = e1.b - e1.a;
        var v2 = e2.b - e2.a;
        var n1 = Vector3.Normalize(v1);
        var n2 = Vector3.Normalize(v2);

        var alpha = Vector3.Dot(n1, n2);
        var r = e1.a - e2.a;
        var rho = Vector3.Dot(r, n1 - alpha * n2) / (alpha * alpha - 1d);
        var tau = Vector3.Dot(r, alpha * n1 - n2) / (alpha * alpha - 1d);
        var pos1 = e1.a + (float) rho * n1;
        var pos2 = e2.a + (float) tau * n2;
        var f1 = Vector3.SqrMagnitude(pos1 - pos2) < Delaunay.Threshold;

        rho /= Vector3.Magnitude(v1);
        tau /= Vector3.Magnitude(v2);
        var f2 = rho is >= 0 and <= 1 && tau is >= 0 and <= 1;
        return f1 && f2;
    }
}