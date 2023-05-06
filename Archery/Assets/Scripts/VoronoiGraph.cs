using System;
using System.Collections.Generic;
using UnityEngine;

public class VoronoiGraph {
    public Dictionary<Vector3, VoronoiNode> nodes;

    public VoronoiGraph(DelaunayNode[] delaunayGraphNode3Ds) {
        nodes = new Dictionary<Vector3, VoronoiNode>();
        foreach (var d in delaunayGraphNode3Ds)
        {
            var t = d.Tetrahedra;
            AddToNodes(t.a);
            AddToNodes(t.b);
            AddToNodes(t.c);
            AddToNodes(t.d);
            var centerTetra = t.GetCircumscribedSphere().center;
            foreach (var n in d.neighbor)
            {
                var centerNeighbor = n.Tetrahedra.GetCircumscribedSphere().center;
                var centerVec  = centerNeighbor - centerTetra;
                var centerSegment  = new Segment(centerTetra, centerNeighbor);
                AssignSegment(t.b, t.a, centerVec, centerSegment);
                AssignSegment(t.c, t.a, centerVec, centerSegment);
                AssignSegment(t.d, t.a, centerVec, centerSegment);
                AssignSegment(t.c, t.b, centerVec, centerSegment);
                AssignSegment(t.d, t.b, centerVec, centerSegment);
                AssignSegment(t.a, t.b, centerVec, centerSegment);
                AssignSegment(t.d, t.c, centerVec, centerSegment);
                AssignSegment(t.a, t.c, centerVec, centerSegment);
                AssignSegment(t.b, t.c, centerVec, centerSegment);
                AssignSegment(t.a, t.d, centerVec, centerSegment);
                AssignSegment(t.b, t.d, centerVec, centerSegment);
                AssignSegment(t.c, t.d, centerVec, centerSegment);
            }

            void AddToNodes(Vector3 tetra)
            {
                if (!nodes.ContainsKey(tetra))
                {
                    nodes.Add(tetra, new VoronoiNode(tetra));
                }
            }
            
            void AssignSegment(Vector3 pair, Vector3 center, Vector3 v1, Segment sg)
            {
                // if vecs are vertical add segment
                if (!(Math.Abs(Vector3.Dot(pair - center, v1)) < Delaunay.Threshold))
                {
                    return;
                }
                nodes.TryGetValue(center, out var v);
                v?.Segments.Add((sg, pair));
            }
        }
        

    }
}