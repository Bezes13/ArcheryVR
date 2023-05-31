using System;
using System.Collections.Generic;
using UnityEngine;

namespace Voronoi
{
    /// <summary>
    /// Represents a Voronoi Graph with a List of VoronoiCells
    /// </summary>
    public class VoronoiGraph
    {
        public readonly Dictionary<Vector3, VoronoiCell> cells;

        public VoronoiGraph(DelaunayNode[] delaunayGraphNode3Ds)
        {
            cells = new Dictionary<Vector3, VoronoiCell>();

            foreach (var d in delaunayGraphNode3Ds)
            {
                var tetra = d.Tetrahedrons;
                AddCell(tetra.a);
                AddCell(tetra.b);
                AddCell(tetra.c);
                AddCell(tetra.d);

                var centerTetra = tetra.GetSphere().center;

                foreach (var n in d.neighbor)
                {
                    var centerNeighbor = n.Tetrahedrons.GetSphere().center;
                    var centerVec = centerNeighbor - centerTetra;
                    var centerEdge = new Line(centerTetra, centerNeighbor);

                    TryAddEdge(tetra.a, tetra.b, centerVec, centerEdge);
                    TryAddEdge(tetra.a, tetra.c, centerVec, centerEdge);
                    TryAddEdge(tetra.a, tetra.d, centerVec, centerEdge);
                    TryAddEdge(tetra.b, tetra.a, centerVec, centerEdge);
                    TryAddEdge(tetra.b, tetra.c, centerVec, centerEdge);
                    TryAddEdge(tetra.b, tetra.d, centerVec, centerEdge);
                    TryAddEdge(tetra.c, tetra.a, centerVec, centerEdge);
                    TryAddEdge(tetra.c, tetra.b, centerVec, centerEdge);
                    TryAddEdge(tetra.c, tetra.d, centerVec, centerEdge);
                    TryAddEdge(tetra.d, tetra.a, centerVec, centerEdge);
                    TryAddEdge(tetra.d, tetra.b, centerVec, centerEdge);
                    TryAddEdge(tetra.d, tetra.c, centerVec, centerEdge);
                }
            }
        }

        private void AddCell(Vector3 tetra)
        {
            if (!cells.ContainsKey(tetra))
            {
                cells.Add(tetra, new VoronoiCell(tetra));
            }
        }

        private void TryAddEdge(Vector3 pair, Vector3 center, Vector3 v1, Line edge)
        {
            if (!(Math.Abs(Vector3.Dot(pair - center, v1)) < Delaunay.Threshold))
            {
                return;
            }

            if (cells.TryGetValue(center, out var v))
            {
                v.Edges.Add((edge, pair));
            }
        }
    }
}
