using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VoronoiNode
{
    public Vector3 center;
    public List<(Segment sg, Vector3 pair)> Segments;
    private List<VoronoiFace> _faces;

    public VoronoiNode(Vector3 c)
    {
        center = c;
        Segments = new List<(Segment, Vector3)>();
    }

    public Mesh Meshilify()
    {
        _faces = Segments.Select(s => s.pair)
            .Distinct()
            .Select(p => new VoronoiFace(p))
            .ToList();

        foreach (var t1 in Segments)
        {
            foreach (var t in _faces)
            {
                var s = t1;
                var f = t;
                if (s.pair.Equals(f.Key))
                {
                    f.TryAddVertices(s.sg, center);
                }
            }
        }

        var nums = 0;
        var verts = new List<Vector3>();

        foreach (var f in _faces)
        {
            var v = f.Meshilify();
            nums += v.Length;
            verts.AddRange(v.Select(v => v));
        }

        var m = new Mesh();
        m.SetVertices(verts);
        m.SetTriangles(Enumerable.Range(0, nums).ToArray(), 0);
        m.RecalculateNormals();
        m.RecalculateTangents();
        m.RecalculateBounds();
        return m;
    }
}