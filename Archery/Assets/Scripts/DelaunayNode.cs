using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DelaunayNode {
    public readonly Tetrahedra Tetrahedra;
    public List<DelaunayNode> neighbor;
    public bool HasFacet(Triangle t) => Tetrahedra.ContainsFace(t);

    public DelaunayNode(Tetrahedra t) : this(t.a, t.b, t.c, t.d) { }

    private DelaunayNode(Vector3 a, Vector3 b, Vector3 c, Vector3 d) {
        Tetrahedra = new Tetrahedra(a, b, c, d);
        neighbor = new List<DelaunayNode>(4);
    }

    public (Triangle t1, Triangle t2, Triangle t3, Triangle t4, DelaunayNode n1, DelaunayNode n2, DelaunayNode n3, DelaunayNode n4) Split(Vector3 p) {
        (DelaunayNode n, Triangle t) abc = (new DelaunayNode(p, Tetrahedra.a, Tetrahedra.b, Tetrahedra.c), new Triangle(Tetrahedra.a, Tetrahedra.b, Tetrahedra.c));
        (DelaunayNode n, Triangle t) bcd = (new DelaunayNode(p, Tetrahedra.b, Tetrahedra.c, Tetrahedra.d), new Triangle(Tetrahedra.b, Tetrahedra.c, Tetrahedra.d));
        (DelaunayNode n, Triangle t) cda = (new DelaunayNode(p, Tetrahedra.c, Tetrahedra.d, Tetrahedra.a), new Triangle(Tetrahedra.c, Tetrahedra.d, Tetrahedra.a));
        (DelaunayNode n, Triangle t) dab = (new DelaunayNode(p, Tetrahedra.d, Tetrahedra.a, Tetrahedra.b), new Triangle(Tetrahedra.d, Tetrahedra.a, Tetrahedra.b));
        abc.n.neighbor = new List<DelaunayNode> { bcd.n, cda.n, dab.n };
        bcd.n.neighbor = new List<DelaunayNode> { cda.n, dab.n, abc.n };
        cda.n.neighbor = new List<DelaunayNode> { dab.n, abc.n, bcd.n };
        dab.n.neighbor = new List<DelaunayNode> { abc.n, bcd.n, cda.n };
        SetNeighbor(abc.n, abc.t);
        SetNeighbor(bcd.n, bcd.t);
        SetNeighbor(cda.n, cda.t);
        SetNeighbor(dab.n, dab.t);
        return (abc.t, bcd.t, cda.t, dab.t, abc.n, bcd.n, cda.n, dab.n);
    }

    public static (Triangle t1, Triangle t2, Triangle t3, Triangle t4, Triangle t5, Triangle t6, DelaunayNode n1, DelaunayNode n2, DelaunayNode n3)
        Flip23(DelaunayNode n1, DelaunayNode n2, Vector3 p1, Vector3 p2, Triangle t) {
        var nab = new DelaunayNode(p1, p2, t.a, t.b);
        var nbc = new DelaunayNode(p1, p2, t.b, t.c);
        var nca = new DelaunayNode(p1, p2, t.c, t.a);
        nab.neighbor = new List<DelaunayNode> { nbc, nca };
        nbc.neighbor = new List<DelaunayNode> { nca, nab };
        nca.neighbor = new List<DelaunayNode> { nab, nbc };
        var t_ab_p1 = new Triangle(t.a, t.b, p1); n1.SetNeighbor(nab, t_ab_p1);
        var t_ab_p2 = new Triangle(t.a, t.b, p2); n2.SetNeighbor(nab, t_ab_p2);
        var t_bc_p1 = new Triangle(t.b, t.c, p1); n1.SetNeighbor(nbc, t_bc_p1);
        var t_bc_p2 = new Triangle(t.b, t.c, p2); n2.SetNeighbor(nbc, t_bc_p2);
        var t_ca_p1 = new Triangle(t.c, t.a, p1); n1.SetNeighbor(nca, t_ca_p1);
        var t_ca_p2 = new Triangle(t.c, t.a, p2); n2.SetNeighbor(nca, t_ca_p2);
        return (t_ab_p1, t_ab_p2, t_bc_p1, t_bc_p2, t_ca_p1, t_ca_p2, nab, nbc, nca);
    }

    public static (Triangle t1, Triangle t2, Triangle t3, Triangle t4, Triangle t5, Triangle t6, DelaunayNode n1, DelaunayNode n2)
        Flip32(DelaunayNode n1, DelaunayNode n2, DelaunayNode n3, Vector3 p31, Vector3 p12, Vector3 p23, Vector3 apex_x, Vector3 apex_y) {
        var a = p31;
        var b = p12;
        var c = p23;
        var nx = new DelaunayNode(a, b, c, apex_x);
        var ny = new DelaunayNode(a, b, c, apex_y);
        nx.neighbor.Add(ny);
        ny.neighbor.Add(nx);
        var xab = new Triangle(apex_x, a, b);
        var yab = new Triangle(apex_y, a, b);
        var xbc = new Triangle(apex_x, b, c);
        var ybc = new Triangle(apex_y, b, c);
        var xca = new Triangle(apex_x, c, a);
        var yca = new Triangle(apex_y, c, a);
        n1.SetNeighbor(nx, xab);
        n2.SetNeighbor(nx, xbc);
        n3.SetNeighbor(nx, xca);
        n1.SetNeighbor(ny, yab);
        n2.SetNeighbor(ny, ybc);
        n3.SetNeighbor(ny, yca);
        return (xab, xbc, xca, yab, ybc, yca, nx, ny);
    }

    private void SetNeighbor(DelaunayNode n, Triangle t) {
        var pair = GetFacingNode(t);
        if (pair == null)
        {
            return;
        }
        n.neighbor.Add(pair);
        pair.ReplaceFacingNode(t, n);
    }

    private void ReplaceFacingNode(Triangle t, DelaunayNode replacer) {
        if (!replacer.HasFacet(t))
        {
            return;
        }
        neighbor = neighbor.Select(n => n.HasFacet(t) ? replacer : n).ToList();
    }

    public DelaunayNode GetFacingNode(Triangle t)
    {
        return !HasFacet(t) ? null : neighbor.Find(n => n.HasFacet(t));
    }
}