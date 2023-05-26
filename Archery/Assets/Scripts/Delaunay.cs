using System;
using System.Collections.Generic;
using UnityEngine;

public class Delaunay
{
    public const float Threshold = 0.0001f;
    private readonly Stack<Triangle> _stack;
    public List<DelaunayNode> Nodes { get; }

    private Vector3 offset;
    private float scl;

    public Delaunay(int num, float scl, GameObject target)
    {
        _stack = new Stack<Triangle>();
        // The tetrahedra which contains the object
        // offset = target.transform.position;
        offset = Vector3.zero;
        this.scl = 1;
        var root = new Tetrahedra(
            new Vector3(0, 0, 0),
            new Vector3(scl * 3, 0, 0),
            new Vector3(0, scl * 3, 0),
            new Vector3(0, 0, scl * 3));
        Nodes = new List<DelaunayNode> {new(root)};
        Debug.Log(target.transform.localScale.x);
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

    void AddPoint(Vector3 p)
    {
        p = p * scl + offset;
        var n = Nodes.Find(tetra => tetra.Tetrahedra.Contains(p));
        var splitTuple = n.Split(p);
        Nodes.Remove(n);
        Nodes.Add(splitTuple.n1);
        Nodes.Add(splitTuple.n2);
        Nodes.Add(splitTuple.n3);
        Nodes.Add(splitTuple.n4);
        _stack.Push(splitTuple.t1);
        _stack.Push(splitTuple.t2);
        _stack.Push(splitTuple.t3);
        _stack.Push(splitTuple.t4);
        // Go through the new added tetras
        while (_stack.Count > 0)
        {
            var t = _stack.Pop();
            // Find tetras which contain the triangle face
            if (!FindNodes(t, out DelaunayNode n1, out DelaunayNode n2)) continue;
            // get the remaining points
            var p1 = n1.Tetrahedra.RemainingPoint(t);
            var p2 = n2.Tetrahedra.RemainingPoint(t);
            // if the tetra sphere contains the dot
            //      if the line between the remaining points intersects the tetra
            //            do flip, remove triangles from stack and add tetras to nodes
            //      else find Intersecting Point
            //          do flip on other things
            if (!n1.Tetrahedra.GetSphere().Contains(p2)) continue;
            if (t.Intersects(new Line(p1, p2), out Vector3 i, out var onEdge))
            {
                var o = DelaunayNode.Flip23(n1, n2, p1, p2, t);
                _stack.Push(o.t1);
                _stack.Push(o.t2);
                _stack.Push(o.t3);
                _stack.Push(o.t4);
                _stack.Push(o.t5);
                _stack.Push(o.t6);
                Nodes.Remove(n1);
                Nodes.Remove(n2);
                Nodes.Add(o.n1);
                Nodes.Add(o.n2);
                Nodes.Add(o.n3);
            }
            else if (onEdge)
            {
                Debug.LogWarning("point is on edge");
            }
            else
            {
                Vector3 far;
                if (IsIntersecting(new Line(i, t.a), new Line(t.b, t.c))) far = t.a;
                else if (IsIntersecting(new Line(i, t.b), new Line(t.c, t.a))) far = t.b;
                else if (IsIntersecting(new Line(i, t.c), new Line(t.a, t.b))) far = t.c;
                else throw new Exception();

                var cm = t.Remaining(far);
                var t3 = new Triangle(cm, p1);
                var n3 = n1.GetFacingNode(t3);

                if (!Equals(n3.Tetrahedra.RemainingPoint(t3), p2)) continue;

                var p12 = far;
                var p23 = p2;
                var p31 = p1;
                var o = DelaunayNode.Flip32(n1, n2, n3, p31, p12, p23, cm.a, cm.b);
                _stack.Push(o.t1);
                _stack.Push(o.t2);
                _stack.Push(o.t3);
                _stack.Push(o.t4);
                _stack.Push(o.t5);
                _stack.Push(o.t6);
                Nodes.Remove(n1);
                Nodes.Remove(n2);
                Nodes.Remove(n3);
                Nodes.Add(o.n1);
                Nodes.Add(o.n2);
            }
        }
    }

    private static bool IsIntersecting(Line e1, Line e2)
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
        var f1 = Vector3.SqrMagnitude(pos1 - pos2) < Threshold;

        rho /= Vector3.Magnitude(v1);
        tau /= Vector3.Magnitude(v2);
        var f2 = rho >= 0 && rho <= 1 && tau >= 0 && tau <= 1;
        return f1 && f2;
    }

    bool FindNodes(Triangle t, out DelaunayNode n1, out DelaunayNode n2)
    {
        var o = Nodes.FindAll(n => n.HasFacet(t));
        if (o.Count == 2)
        {
            n1 = o[0];
            n2 = o[1];
            return true;
        }

        n1 = n2 = default;
        return false;
    }
}