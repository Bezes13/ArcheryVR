using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Voronoi
{
    public class VoronoiFace
    {
        public Vector3 Key { get; }
        private List<Vector3> _vertices;

        public VoronoiFace(Vector3 key)
        {
            Key = key;
            _vertices = new List<Vector3>();
        }

        public void TryAddVertices(Line s, Vector3 center)
        {
            var sA = s.a - center;
            var sB = s.b - center;

            if (!_vertices.Contains(sA))
            {
                _vertices.Add(sA);
            }

            if (!_vertices.Contains(sB))
            {
                _vertices.Add(sB);
            }
        }


        public Vector3[] Meshilify()
        {
            var v0 = _vertices[0];
            var v1 = _vertices[1];
            var o = new Vector3[(_vertices.Count - 2) * 3];

            _vertices = _vertices.Skip(2)
                .OrderBy(v => Vector3.Dot(v0 - v1, Vector3.Normalize(v - v1)))
                .Prepend(v1)
                .Prepend(v0)
                .ToList();

            for (var i = 1; i < _vertices.Count - 1; i++)
            {
                var va = _vertices[i];
                var vb = _vertices[i + 1];
                var f = Vector3.Dot(Vector3.Cross(vb - va, v0 - va), v0) > 0;

                o[(i - 1) * 3 + 0] = v0;
                o[(i - 1) * 3 + 1] = f ? va : vb;
                o[(i - 1) * 3 + 2] = f ? vb : va;
            }

            return o;
        }
    }
}