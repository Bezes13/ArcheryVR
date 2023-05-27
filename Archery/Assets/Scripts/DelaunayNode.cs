using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DelaunayNode
{
    public readonly Tetrahedra Tetrahedra;
    public List<DelaunayNode> neighbor;
    public bool HasFacet(Triangle t) => Tetrahedra.ContainsFace(t);

    public DelaunayNode(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        Tetrahedra = new Tetrahedra(a, b, c, d);
        neighbor = new List<DelaunayNode>(4);
    }

    public (List<Triangle>, List<DelaunayNode>) Split(Vector3 p)
    {
        var abc = new DelaunayNode(p, Tetrahedra.a, Tetrahedra.b, Tetrahedra.c);
        var abcTriangle = new Triangle(Tetrahedra.a, Tetrahedra.b, Tetrahedra.c);
        var bcd = new DelaunayNode(p, Tetrahedra.b, Tetrahedra.c, Tetrahedra.d);
        var bcdTriangle =  new Triangle(Tetrahedra.b, Tetrahedra.c, Tetrahedra.d);

        var cda = new DelaunayNode(p, Tetrahedra.c, Tetrahedra.d, Tetrahedra.a);
        var cdaTriangle =  new Triangle(Tetrahedra.c, Tetrahedra.d, Tetrahedra.a);
        var dab = new DelaunayNode(p, Tetrahedra.d, Tetrahedra.a, Tetrahedra.b);
        var dabTriangle = new Triangle(Tetrahedra.d, Tetrahedra.a, Tetrahedra.b);
        abc.neighbor = new List<DelaunayNode> {bcd, cda, dab};
        bcd.neighbor = new List<DelaunayNode> {cda, dab, abc};
        cda.neighbor = new List<DelaunayNode> {dab, abc, bcd};
        dab.neighbor = new List<DelaunayNode> {abc, bcd, cda};
        SetNeighbor(abc, abcTriangle);
        SetNeighbor(bcd, bcdTriangle);
        SetNeighbor(cda, cdaTriangle);
        SetNeighbor(dab, dabTriangle);
        return (new List<Triangle>(){abcTriangle, bcdTriangle, cdaTriangle, dabTriangle}, new List<DelaunayNode>(){ abc, bcd, cda, dab});
    }
    
    public void SetNeighbor(DelaunayNode n, Triangle t)
    {
        var pair = GetFacingNode(t);
        if (pair == null)
        {
            return;
        }

        n.neighbor.Add(pair);
        if (!n.HasFacet(t))
        {
            return;
        }

        neighbor = neighbor.Select(node => node.HasFacet(t) ? n : node).ToList();
    }

    public DelaunayNode GetFacingNode(Triangle t)
    {
        return HasFacet(t) ? neighbor.Find(n => n.HasFacet(t)) : null;
    }
}