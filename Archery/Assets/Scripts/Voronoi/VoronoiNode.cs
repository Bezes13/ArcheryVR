using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Voronoi
{
    public class VoronoiNode
    {
        public Vector3 center;
        public readonly List<(Line sg, Vector3 pair)> Segments;
        private List<VoronoiFace> _faces;

        public VoronoiNode(Vector3 c)
        {
            center = c;
            Segments = new List<(Line, Vector3)>();
        }

        public Mesh Meshilify()
        {
            _faces = new List<VoronoiFace>();

            foreach ((_, Vector3 pair) in Segments)
            {
                var voronoiFace = new VoronoiFace(pair);
                if (!_faces.Contains(voronoiFace))
                {
                    _faces.Add(voronoiFace);
                }
            }
            foreach (var t1 in Segments)
            {
                foreach (var t in _faces)
                {
                    var (sg, pair) = t1;
                    if (pair.Equals(t.Key))
                    {
                        t.TryAddVertices(sg, center);
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
}