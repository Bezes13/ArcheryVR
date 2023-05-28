using System;
using System.Collections.Generic;
using UnityEngine;

public class Delaunay
{
    public const float Threshold = 0.0001f;
    private readonly Stack<Tetrahedra> _stack;
    public List<DelaunayNode> Nodes { get; }

    public Delaunay(int num, float scl, GameObject target)
    {
        _stack = new Stack<Tetrahedra>();
        // The tetrahedra which contains the object
        // offset = target.transform.position;
        var root = new Tetrahedra(
            new Vector3(0, 0, 0),
            new Vector3(scl * 3, 0, 0),
            new Vector3(0, scl * 3, 0),
            new Vector3(0, 0, scl * 3));
        Nodes = new List<DelaunayNode> {new(root.a, root.b, root.c, root.d)};
        // Generate Random Points and add them to Delaunay 
        for (var i = 0; i < num; i++)
        {
            var p = new Vector3(UnityEngine.Random.value * scl, UnityEngine.Random.value * scl,
                UnityEngine.Random.value * scl);
            //p = target.GetComponent<MeshFilter>().mesh.bounds.ClosestPoint(p);
            // Split(p*target.transform.localScale.x); 

            AddPoint(p);
        }
    }

    private void AddPoint(Vector3 p)
    {
        var n = Nodes.Find(tetra => tetra.Tetrahedra.Contains(p));
        var nodes = n.Split(p);
        Nodes.Remove(n);
        Nodes.AddRange(nodes);
        foreach (var triangle in nodes)
        {
            _stack.Push(triangle.Tetrahedra);
        }

        // Go through the new added tetras
        while (_stack.Count > 0)
        {
            var t = _stack.Pop();
            // Find tetras which contain the triangle face
            var triangle = t.RemainingTriangle(p);
            var t2 = FindNodeWithSameFace(triangle, p);
            if (t2 == null) continue;
            // get the remaining points
            var p2 = t2.Tetrahedra.RemainingPoint(triangle);
            // If is Delaunay then we can skip this
            if (t.GetSphere().Contains(p2)) continue;
            
            if (!triangle.Intersects(new Line(p, p2), out var i))
            {
                Vector3 far;
                if (Line.IsIntersecting(new Line(i, t.a), new Line(t.b, t.c))) far = t.a;
                else if (Line.IsIntersecting(new Line(i, t.b), new Line(t.c, t.a))) far = t.b;
                else if (Line.IsIntersecting(new Line(i, t.c), new Line(t.a, t.b))) far = t.c;
                else throw new Exception();

                var cm = triangle.Remaining(far);
                var t3 = new Triangle(cm, p);
                var n3 = nodes[0].GetFacingNode(t3);

                if (!Equals(n3.Tetrahedra.RemainingPoint(t3), p2)) continue;
                
                var o = Flip32(nodes[0], nodes[1], n3, p, far, p2, cm.a, cm.b);
                FlipResult(o);
                Nodes.Remove(nodes[0]);
                Nodes.Remove(nodes[1]);
                Nodes.Remove(n3);
                Nodes.AddRange(o);
            }
            else
            {
                var o = Flip23(nodes[0], nodes[1], p, p2, triangle);
                FlipResult(o);
                Nodes.Remove(nodes[0]);
                Nodes.Remove(nodes[1]);
                Nodes.AddRange(o);
            }
            
            void FlipResult(List<DelaunayNode> flip)
            {
                foreach (var node in flip)
                {
                    _stack.Push(node.Tetrahedra);
                }

            }
        }
    }

    private DelaunayNode FindNodeWithSameFace(Triangle triangle, Vector3 p)
    {
        var o = Nodes.FindAll(n => n.HasFacet(triangle));
        if (o.Count != 2) return null;

        foreach (var node in o)
        {
            if (node.Tetrahedra.RemainingPoint(triangle) != p)
            {
                return node;
            }
        }

        return null;
    }
    
    private static List<DelaunayNode> Flip23(DelaunayNode n1, DelaunayNode n2, Vector3 p1, Vector3 p2, Triangle t)
    {
        var nab = new DelaunayNode(p1, p2, t.a, t.b);
        var nbc = new DelaunayNode(p1, p2, t.b, t.c);
        var nca = new DelaunayNode(p1, p2, t.c, t.a);
    
        nab.neighbor = new List<DelaunayNode> { nbc, nca };
        nbc.neighbor = new List<DelaunayNode> { nca, nab };
        nca.neighbor = new List<DelaunayNode> { nab, nbc };
    
        var triangles = new List<Triangle>
        {
            new(t.a, t.b, p1), new(t.a, t.b, p2), new(t.b, t.c, p1),
            new(t.b, t.c, p2), new(t.c, t.a, p1), new(t.c, t.a, p2)
        };
    
        n1.SetNeighbor(nab, triangles[0]);
        n2.SetNeighbor(nab, triangles[1]);
        n1.SetNeighbor(nbc, triangles[2]);
        n2.SetNeighbor(nbc, triangles[3]);
        n1.SetNeighbor(nca, triangles[4]);
        n2.SetNeighbor(nca, triangles[5]);
    
        return  new List<DelaunayNode> { nab, nbc, nca };
    }

    private static List<DelaunayNode> Flip32(DelaunayNode n1, DelaunayNode n2, DelaunayNode n3, Vector3 a, Vector3 b, Vector3 c, Vector3 x, Vector3 y)
    {
        var nx = new DelaunayNode(a, b, c, x);
        var ny = new DelaunayNode(a, b, c, y);
        nx.neighbor.Add(ny);
        ny.neighbor.Add(nx);
    
        var triangles = new List<Triangle>
        {
            new(x, a, b), new(x, b, c), new(x, c, a),
            new(y, a, b), new(y, b, c), new(y, c, a)
        };
    
        n1.SetNeighbor(nx, triangles[0]);
        n2.SetNeighbor(nx, triangles[1]);
        n3.SetNeighbor(nx, triangles[2]);
    
        n1.SetNeighbor(ny, triangles[3]);
        n2.SetNeighbor(ny, triangles[4]);
        n3.SetNeighbor(ny, triangles[5]);
    
        return new List<DelaunayNode> { nx, ny };
    }
}