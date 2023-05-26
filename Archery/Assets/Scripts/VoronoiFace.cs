using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VoronoiFace
{
    public Vector3 Key { get; }
    public List<Vector3> Vertices;

    public VoronoiFace(Vector3 key)
    {
        Key = key;
        Vertices = new List<Vector3>();
    }

    public void TryAddVertices(Line s, Vector3 center)
    {
        var f1 = false;
        var f2 = false;
        foreach (var v in Vertices)
        {
            if (v.Equals(s.a - center))
            {
                f1 = true;
            }

            if (v.Equals(s.b - center))
            {
                f2 = true;
            }
        }

        if (!f1)
        {
            Vertices.Add(s.a - center);
        }

        if (!f2)
        {
            Vertices.Add(s.b - center);
        }
    }


    public Vector3[] Meshilify()
    {
        var v0 = Vertices[0];
        var v1 = Vertices[1];
        var o = new Vector3[(Vertices.Count - 2) * 3];

        Vertices = Vertices.Skip(2)
            .OrderBy(v => UnityEngine.Vector3.Dot(v0 - v1, Vector3.Normalize(v - v1)))
            .Prepend(v1)
            .Prepend(v0)
            .ToList();

        for (var i = 1; i < Vertices.Count - 1; i++)
        {
            var va = Vertices[i];
            var vb = Vertices[i + 1];
            var f = UnityEngine.Vector3.Dot(Vector3.Cross(vb - va, v0 - va), v0) > 0;
            o[(i - 1) * 3 + 0] = v0;
            o[(i - 1) * 3 + 1] = f ? va : vb;
            o[(i - 1) * 3 + 2] = f ? vb : va;
        }

        return o;
    }
}