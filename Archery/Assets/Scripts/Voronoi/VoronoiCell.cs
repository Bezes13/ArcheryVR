using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Voronoi
{
    /// <summary>
    /// Represents a Voronoi cell, which has a center and a list of edges which belong to the cell
    /// </summary>
    public class VoronoiCell
    {
        public Vector3 center;
        public readonly List<(Line sg, Vector3 neighbour)> Edges;

        public VoronoiCell(Vector3 c)
        {
            center = c;
            Edges = new List<(Line, Vector3)>();
        }

        public Mesh CreateMesh()
        {
             var faces = new List<VoronoiFace>();

            foreach ((_, Vector3 pair) in Edges)
            {
                var voronoiFace = new VoronoiFace(pair);
                if (!faces.Exists(x=> x.Key.Equals(pair)))
                {
                    faces.Add(voronoiFace);
                }
            }
            foreach (var (s, neighbour) in Edges)
            {
                foreach (var t in faces)
                {
                    if (!neighbour.Equals(t.Key)) continue;
                    var sA = s.a - center;
                    var sB = s.b - center;

                    if (!t.Vertices.Contains(sA))
                    {
                        t.Vertices.Add(sA);
                    }

                    if (!t.Vertices.Contains(sB))
                    {
                        t.Vertices.Add(sB);
                    }
                }
            }

            var nums = 0;
            var verts = new List<Vector3>();

            foreach (var f in faces)
            {
                var v = f.Meshilify();
                nums += v.Length;
                verts.AddRange(v.Select(vec => vec));
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