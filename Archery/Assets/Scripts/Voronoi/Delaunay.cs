using System;
using System.Collections.Generic;
using UnityEngine;

namespace Voronoi
{
    public class Delaunay
    {
        public const float Threshold = 0.0001f;
        private readonly Stack<DelaunayNode> _stack;
        public List<DelaunayNode> Nodes { get; }

        public Delaunay(int num, float scl, GameObject target)
        {
            _stack = new Stack<DelaunayNode>();
            // The tetrahedron which contains the object
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
                Debug.Log(p);
                p = target.GetComponent<MeshFilter>().mesh.bounds.ClosestPoint(p);
                Debug.Log(p);
                // Split(p*target.transform.localScale.x); 

                AddPoint(p);
            }
        }

        private void AddPoint(Vector3 p)
        {
            // Find Node which contains p
            var n = Nodes.Find(tetra => tetra.Tetrahedrons.Contains(p));
            // Split n into 4 nodes and add them to stack
            var splitNode = n.Split(p);
            Nodes.Remove(n);
            Nodes.AddRange(splitNode);
            foreach (var tetra in splitNode)
            {
                _stack.Push(tetra);
            }

            // Go through the stacked tetras
            while (_stack.Count > 0)
            {
                // tetra with (a,b,c,p)
                var t = _stack.Pop();
                // triangle (a,b,c)
                var triangle = t.Tetrahedrons.RemainingTriangle(p);
                // find neighbour (a,b,c,d)
                var node = t.GetFacingNode(triangle);
                if (node == null) continue;
                // get the remaining points
                var p1 = p;
                // get d
                var p2 = node.Tetrahedrons.RemainingPoint(triangle);
            
                // check if d is inside sphere of (a,b,c,p)
                if (!t.Tetrahedrons.GetSphere().Contains(p2)) continue;
            
            
                if (!triangle.Intersects(new Line(p1, p2), out var i))
                {
                    // case 2: two sides are visible
                    // find the conlficting tetra (a,b,p,d)
                    Vector3 far;
                    if (Line.IsIntersecting(new Line(i, triangle.a), new Line(triangle.b, triangle.c))) far = triangle.a;
                    else if (Line.IsIntersecting(new Line(i, triangle.b), new Line(triangle.c, triangle.a))) far = triangle.b;
                    else if (Line.IsIntersecting(new Line(i, triangle.c), new Line(triangle.a, triangle.b))) far = triangle.c;
                    else throw new Exception();

                    var ab = triangle.Remaining(far);
                    var t3 = new Triangle(ab, p1);
                    var conflictingNode = t.GetFacingNode(t3);
                    // if the remaining point is not d, then we can continue else do the flip
                    if (!Equals(conflictingNode.Tetrahedrons.RemainingPoint(t3), p2)) continue;
                
                    var o = Flip32(t, node, conflictingNode, p1, far, p2, ab.a, ab.b);
                    FlipResult(o);
                    Nodes.Remove(t);
                    Nodes.Remove(node);
                    Nodes.Remove(conflictingNode);
                    Nodes.AddRange(o);
                }
                else
                {
                    // case 1: only one face is visible
                    var o = Flip23(t, node, p1, p2, triangle);
                    FlipResult(o);
                    Nodes.Remove(t);
                    Nodes.Remove(node);
                    Nodes.AddRange(o);
                }
            
                void FlipResult(List<DelaunayNode> flip)
                {
                    foreach (var item in flip)
                    {
                        _stack.Push(item);
                    }

                }
            }
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

        private static List<DelaunayNode> Flip32(DelaunayNode n1, DelaunayNode n2, DelaunayNode n3, Vector3 p1, Vector3 far, Vector3 p2, Vector3 x, Vector3 y)
        {
            var nx = new DelaunayNode(p1, far, p2, x);
            var ny = new DelaunayNode(p1, far, p2, y);
            nx.neighbor.Add(ny);
            ny.neighbor.Add(nx);
    
            var triangles = new List<Triangle>
            {
                new(x, p1, far), new(x, far, p2), new(x, p2, p1),
                new(y, p1, far), new(y, far, p2), new(y, p2, p1)
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
}