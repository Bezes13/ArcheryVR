using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Voronoi
{
    public class DelaunayNode
    {
        public readonly Tetrahedra Tetrahedrons;
        public List<DelaunayNode> neighbor;
        private bool HasFacet(Triangle t) => Tetrahedrons.ContainsFace(t);

        public DelaunayNode(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            Tetrahedrons = new Tetrahedra(a, b, c, d);
            neighbor = new List<DelaunayNode>(4);
        }

        public  List<DelaunayNode> Split(Vector3 p)
        {
            var abc = new DelaunayNode(p, Tetrahedrons.a, Tetrahedrons.b, Tetrahedrons.c);
            var abcTriangle = new Triangle(Tetrahedrons.a, Tetrahedrons.b, Tetrahedrons.c);
            var bcd = new DelaunayNode(p, Tetrahedrons.b, Tetrahedrons.c, Tetrahedrons.d);
            var bcdTriangle =  new Triangle(Tetrahedrons.b, Tetrahedrons.c, Tetrahedrons.d);

            var cda = new DelaunayNode(p, Tetrahedrons.c, Tetrahedrons.d, Tetrahedrons.a);
            var cdaTriangle =  new Triangle(Tetrahedrons.c, Tetrahedrons.d, Tetrahedrons.a);
            var dab = new DelaunayNode(p, Tetrahedrons.d, Tetrahedrons.a, Tetrahedrons.b);
            var dabTriangle = new Triangle(Tetrahedrons.d, Tetrahedrons.a, Tetrahedrons.b);
            abc.neighbor = new List<DelaunayNode> {bcd, cda, dab};
            bcd.neighbor = new List<DelaunayNode> {cda, dab, abc};
            cda.neighbor = new List<DelaunayNode> {dab, abc, bcd};
            dab.neighbor = new List<DelaunayNode> {abc, bcd, cda};
            SetNeighbor(abc, abcTriangle);
            SetNeighbor(bcd, bcdTriangle);
            SetNeighbor(cda, cdaTriangle);
            SetNeighbor(dab, dabTriangle);
            return new List<DelaunayNode>(){ abc, bcd, cda, dab};
        }
    
        public void SetNeighbor(DelaunayNode n, Triangle t)
        {
            var pair = GetFacingNode(t);
            if (pair == null)
            {
                return;
            }

            n.neighbor.Add(pair);
            pair.ReplaceFacingNode(t, n);
        }
        private void ReplaceFacingNode(Triangle t, DelaunayNode replacer)
        {
            if (!replacer.HasFacet(t))
            {
                return;
            }
            neighbor = neighbor.Select(n => n.HasFacet(t) ? replacer : n).ToList();
        }

        public DelaunayNode GetFacingNode(Triangle t)
        {
            return HasFacet(t) ? neighbor.Find(n => n.HasFacet(t)) : null;
        }
    }
}