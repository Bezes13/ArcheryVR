using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Voronoi
{
    public static class MeshCreater
    {
        public static Polygon[] Intersection(Polygon[] first, Polygon[] second)
        {
            var tn = new Node(new List<Polygon>(first));
            var pn = new Node(new List<Polygon>(second));
            tn.Invert();
            pn.ClipTo(tn);
            pn.Invert();
            tn.ClipTo(pn);
            pn.ClipTo(tn);
            tn.Build(pn.GetPolygonData());
            tn.Invert();
            return tn.GetPolygonData().ToArray();
        }

        public class Plane
        {
            public Vector3 N { get; private set; }
            private double W { get; set; }

            static readonly double EPSILON = 1e-5d;
            static readonly int ONPLANE = 0;
            static readonly int FACE = 1;
            static readonly int BACK = 2;
            static readonly int SPAN = 3;

            public Plane(Plane src)
            {
                N = src.N;
                W = src.W;
            }

            public Plane(Vector3 a, Vector3 b, Vector3 c)
            {
                N = Vector3.Normalize(Vector3.Cross(b - a, c - a));
                W = Vector3.Dot(N, a);
            }

            public void Flip()
            {
                N *= -1;
                W *= -1;
            }

            int GetType(Vector3 p)
            {
                var v = Vector3.Dot(N, p) - W;
                var isNearPlane = Math.Abs(v) < EPSILON;
                var isFacingSide = v > 0;
                if (isNearPlane)
                {
                    return ONPLANE;
                }

                return isFacingSide ? FACE : BACK;
            }

            public (Polygon onPF, Polygon onPB, Polygon face, Polygon back) SplitPolygon(Polygon p)
            {
                var l = p.Verts.Length;
                var pType = 0;
                var vType = new int[l];

                for (var i = 0; i < l; i++)
                {
                    var t = GetType(p.Verts[i]);
                    pType |= t;
                    vType[i] = t;
                }

                switch (pType)
                {
                    default: throw new Exception();
                    case 0: return (Vector3.Dot(N, p.Plane.N) > 0) ? (p, null, null, null) : (null, p, null, null);
                    case 1: return (null, null, p, null);
                    case 2: return (null, null, null, p);
                    case 3:
                        var faces = new List<Vector3>();
                        var backs = new List<Vector3>();
                        for (var i = 0; i < l; i++)
                        {
                            var j = (i + 1) % l;
                            var si = vType[i];
                            var sj = vType[j];
                            var vi = p.Verts[i];
                            var vj = p.Verts[j];

                            if (si == FACE) faces.Add(vi);
                            else if (si == BACK) backs.Add(vi);
                            else
                            {
                                faces.Add(vi);
                                backs.Add(vi);
                            }

                            if ((si | sj) == SPAN)
                            {
                                var t = (W - Vector3.Dot(N, vi)) / Vector3.Dot(N, vj - vi);
                                var v = Vector3.Lerp(vi, vj, (float) t);
                                faces.Add(v);
                                backs.Add(v);
                            }
                        }

                        return (null, null, new Polygon(faces.ToArray()), new Polygon(backs.ToArray()));
                }
            }
        }

        public class Polygon
        {
            public Vector3[] Verts { get; }
            public Plane Plane { get; }

            public Polygon(Vector3[] vs)
            {
                Verts = vs;
                Plane = new Plane(Verts[0], Verts[1], Verts[2]);
            }

            public Polygon(Polygon src)
            {
                Verts = Clone(src.Verts);
                Plane = new Plane(Verts[0], Verts[1], Verts[2]);
            }

            public void Flip()
            {
                Plane.Flip();
                Array.Reverse(Verts);
            }
        }

        public static Polygon[] GenCsgTree(Transform t, bool isFragmented = false)
        {
            var matrix = Matrix4x4.TRS(Vector3.zero, t.rotation, t.localScale);
            var mesh = t.GetComponent<MeshFilter>().sharedMesh;
            return GenCsgTree(matrix, mesh, isFragmented);
        }

        public static Polygon[] GenCsgTree(Matrix4x4 mtx, Mesh msh, bool isFragmented = false)
        {
            var vertices = new List<Vector3>();
            var normals = new List<Vector3>();
            if (isFragmented)
            {
                msh.GetVertices(vertices);
                msh.GetNormals(normals);
            }
            else
            {
                Fragmentize(msh, out vertices, out normals);
            }

            var verts = new (Vector3 pos, Vector3 nrm)[vertices.Count];
            var polys = new Polygon[vertices.Count / 3];

            for (var i = 0; i < verts.Length; i++)
                verts[i] = (mtx.MultiplyPoint(vertices[i]), mtx.rotation * normals[i]);

            for (var i = 0; i < polys.Length; i++)
            {
                var (a, nrm) = verts[i * 3 + 0];
                var (b, _) = verts[i * 3 + 1];
                var (c, _) = verts[i * 3 + 2];
                var crs = Vector3.Cross(b - a, c - a);
                if (Vector3.Dot(crs, nrm) > 0)
                    polys[i] = new Polygon(new[] {a, b, c});
                else
                    polys[i] = new Polygon(new[] {a, c, b});
            }

            return polys;
        }

        public static Mesh Meshing(Polygon[] tree)
        {
            var vs = new List<Vector3>();
            var ns = new List<Vector3>();
            var dst = new Mesh
            {
                indexFormat = UnityEngine.Rendering.IndexFormat.UInt32
            };
            var num = 0;
            foreach (var p in tree)
            {
                for (var i = 3; i <= p.Verts.Length; i++)
                {
                    var n = p.Plane.N;
                    vs.Add(p.Verts[0]);
                    vs.Add(p.Verts[i - 2]);
                    vs.Add(p.Verts[i - 1]);
                    ns.Add(n);
                    ns.Add(n);
                    ns.Add(n);
                    num += 3;
                }
            }

            dst.SetVertices(vs);
            dst.SetNormals(ns);
            dst.SetTriangles(Enumerable.Range(0, num).ToArray(), 0);
            dst.RecalculateNormals();
            dst.RecalculateBounds();
            return dst;
        }

        static void Fragmentize(Mesh src, out List<Vector3> outVertices, out List<Vector3> outNormals)
        {
            // Get vertices and normals from Mesh
            var triangles = src.triangles;
            var vertices = new Vector3[triangles.Length];
            var normals = new Vector3[triangles.Length];
            for (var i = 0; i < triangles.Length; i++)
            {
                vertices[i] = src.vertices[triangles[i]];
                normals[i] = src.normals[triangles[i]];
            }

            outVertices = vertices.ToList();
            outNormals = normals.ToList();
        }

        public class Node
        {
            private Node _nf;
            private Node _nb;
            private Plane _pl;
            private List<Polygon> _polygons;

            private Node()
            {
                _polygons = new List<Polygon>();
            }

            public Node(List<Polygon> src)
            {
                _polygons = new List<Polygon>();
                Build(src);
            }

            public void Invert()
            {
                foreach (var t in _polygons)
                {
                    t.Flip();
                }

                _pl?.Flip();
                _nf?.Invert();
                _nb?.Invert();
                (_nf, _nb) = (_nb, _nf);
            }

            private List<Polygon> ClipPolygons(List<Polygon> src)
            {
                if (_pl == null) return new List<Polygon>(src);
                var pf = new List<Polygon>();
                var pb = new List<Polygon>();
                foreach (var p in src)
                {
                    var o = _pl.SplitPolygon(p);
                    if (o.onPF != null) pf.Add(o.onPF);
                    if (o.onPB != null) pb.Add(o.onPB);
                    if (o.face != null) pf.Add(o.face);
                    if (o.back != null) pb.Add(o.back);
                }

                if (_nf != null) pf = _nf.ClipPolygons(pf);
                if (_nb != null) pb = _nb.ClipPolygons(pb);
                else pb.Clear();
                pf.AddRange(pb);
                return pf;
            }

            public void ClipTo(Node pair)
            {
                _polygons = pair.ClipPolygons(_polygons);
                _nf?.ClipTo(pair);
                _nb?.ClipTo(pair);
            }

            public List<Polygon> GetPolygonData()
            {
                var clone = Clone(_polygons);
                if (_nf != null) clone.AddRange(_nf.GetPolygonData());
                if (_nb != null) clone.AddRange(_nb.GetPolygonData());
                return clone;
            }

            public void Build(List<Polygon> src)
            {
                if (src.Count == 0) return;
                if (_pl == null) _pl = new Plane(src[0].Plane);
                var pf = new List<Polygon>();
                var pb = new List<Polygon>();
                for (var i = 0; i < src.Count; i++)
                {
                    var o = _pl.SplitPolygon(src[i]);
                    if (o.onPF != null) _polygons.Add(o.onPF);
                    if (o.onPB != null) _polygons.Add(o.onPB);
                    if (o.face != null) pf.Add(o.face);
                    if (o.back != null) pb.Add(o.back);
                }

                if (pf.Count > 0)
                {
                    if (_nf == null)
                    {
                        _nf = new Node();
                    }

                    _nf.Build(pf);
                }

                if (pb.Count > 0)
                {
                    if (_nb == null)
                    {
                        _nb = new Node();
                    }

                    _nb.Build(pb);
                }
            }
        }

        private static T[] Clone<T>(T[] src)
        {
            var l = src.Length;
            var d = new T[l];
            Array.Copy(src, d, l);
            return d;
        }

        private static List<Polygon> Clone(List<Polygon> src)
        {
            var l = src.Count;
            var d = new List<Polygon>();
            for (var i = 0; i < l; i++) d.Add(new Polygon(src[i]));
            return d;
        }
    }
}