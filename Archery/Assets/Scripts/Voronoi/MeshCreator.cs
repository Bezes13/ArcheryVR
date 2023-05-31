using System;
using System.Collections.Generic;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Voronoi
{
    /// <summary>
    /// MeshCreator Class is used to Create the Intersection between Meshes by using csg trees.
    /// </summary>
    public static class MeshCreator
    {
       public static Polygon[] Intersection(Polygon[] first, Polygon[] second)
        {
            var treeFirst = new Node(new List<Polygon>(first));
            var treeSecond = new Node(new List<Polygon>(second));

            treeFirst.Invert();
            treeSecond.ClipTo(treeFirst);
            treeSecond.Invert();
            treeFirst.ClipTo(treeSecond);
            treeSecond.ClipTo(treeFirst);

            treeFirst.Build(treeSecond.GetPolygonData());
            treeFirst.Invert();

            return treeFirst.GetPolygonData().ToArray();
        }

        public class Plane
        {
            public Vector3 Normal { get; private set; }
            private float Distance { get; set; }

            private const float Epsilon = 1e-5f;

            [Flags]
            public enum PlaneType
            {
                OnPlane, Front,Back,Spanning
            }

            public Plane(Plane src)
            {
                Normal = src.Normal;
                Distance = src.Distance;
            }

            public Plane(Vector3 a, Vector3 b, Vector3 c)
            {
                Normal = Vector3.Normalize(Vector3.Cross(b - a, c - a));
                Distance = Vector3.Dot(Normal, a);
            }

            public void Flip()
            {
                Normal = -Normal;
                Distance = -Distance;
            }

            public PlaneType GetType(Vector3 point)
            {
                var distance = Vector3.Dot(Normal, point) - Distance;
                var isNearPlane = Mathf.Abs(distance) < Epsilon;
                var isFront = distance > 0;

                if (isNearPlane)
                    return PlaneType.OnPlane;

                return isFront ? PlaneType.Front : PlaneType.Back;
            }

            public (Polygon onFront, Polygon onBack, Polygon front, Polygon back) SplitPolygon(Polygon polygon)
            {
                var verticesCount = polygon.Vertices.Length;
                var polygonType = PlaneType.OnPlane;
                var vertexTypes = new PlaneType[verticesCount];

                for (var i = 0; i < verticesCount; i++)
                {
                    var type = GetType(polygon.Vertices[i]);
                    polygonType |= type;
                    vertexTypes[i] = type;
                }

                switch (polygonType)
                {
                    default: throw new Exception();
                    case PlaneType.OnPlane: return (Vector3.Dot(Normal, polygon.Plane.Normal) > 0)
                        ? (polygon, null, null, null)
                        : (null, polygon, null, null);
                    case PlaneType.Front: return (null, null, polygon, null);
                    case PlaneType.Back: return (null, null, null, polygon);
                    case PlaneType.Spanning:
                        var frontVertices = new List<Vector3>();
                        var backVertices = new List<Vector3>();

                        for (var i = 0; i < verticesCount; i++)
                        {
                            var j = (i + 1) % verticesCount;
                            var currentType = vertexTypes[i];
                            var nextType = vertexTypes[j];
                            var currentVertex = polygon.Vertices[i];
                            var nextVertex = polygon.Vertices[j];

                            if (currentType == PlaneType.Front)
                                frontVertices.Add(currentVertex);
                            else if (currentType == PlaneType.Back)
                                backVertices.Add(currentVertex);
                            else
                            {
                                frontVertices.Add(currentVertex);
                                backVertices.Add(currentVertex);
                            }

                            if ((currentType | nextType) == PlaneType.Spanning)
                            {
                                var t = (Distance - Vector3.Dot(Normal, currentVertex)) / Vector3.Dot(Normal, nextVertex - currentVertex);
                                var vertex = Vector3.Lerp(currentVertex, nextVertex, t);
                                frontVertices.Add(vertex);
                                backVertices.Add(vertex);
                            }
                        }

                        return (null, null, new Polygon(frontVertices.ToArray()), new Polygon(backVertices.ToArray()));
                }
            }
        }

        public class Polygon
        {
            public Vector3[] Vertices { get; }
            public Plane Plane { get; }

            public Polygon(Vector3[] vertices)
            {
                Vertices = vertices;
                Plane = new Plane(Vertices[0], Vertices[1], Vertices[2]);
            }

            public Polygon(Polygon src)
            {
                Vertices = Clone(src.Vertices);
                Plane = new Plane(Vertices[0], Vertices[1], Vertices[2]);
            }

            public void Flip()
            {
                Plane.Flip();
                Array.Reverse(Vertices);
            }
        }

        public static Polygon[] GenerateCsgTree(Transform transform, bool isFragmented = false)
        {
            var matrix = Matrix4x4.TRS(Vector3.zero, transform.rotation, transform.localScale);
            var mesh = transform.GetComponent<MeshFilter>().sharedMesh;
            return GenerateCsgTree(matrix, mesh, isFragmented);
        }

        public static Polygon[] GenerateCsgTree(Matrix4x4 matrix, Mesh mesh, bool isFragmented = false)
        {
            var vertices = new List<Vector3>();
            var normals = new List<Vector3>();

            if (isFragmented)
            {
                mesh.GetVertices(vertices);
                mesh.GetNormals(normals);
            }
            else
            {
                Fragmentize(mesh, out vertices, out normals);
            }

            var transformedVertices = new (Vector3 position, Vector3 normal)[vertices.Count];
            var polygons = new Polygon[vertices.Count / 3];

            for (var i = 0; i < transformedVertices.Length; i++)
                transformedVertices[i] = (matrix.MultiplyPoint(vertices[i]), matrix.rotation * normals[i]);

            for (var i = 0; i < polygons.Length; i++)
            {
                var (vertexA, normal) = transformedVertices[i * 3 + 0];
                var (vertexB, _) = transformedVertices[i * 3 + 1];
                var (vertexC, _) = transformedVertices[i * 3 + 2];
                var crossProduct = Vector3.Cross(vertexB - vertexA, vertexC - vertexA);

                if (Vector3.Dot(crossProduct, normal) > 0)
                    polygons[i] = new Polygon(new[] { vertexA, vertexB, vertexC });
                else
                    polygons[i] = new Polygon(new[] { vertexA, vertexC, vertexB });
            }

            return polygons;
        }

        public static Mesh CreateMesh(Polygon[] tree)
        {
            var vertices = new List<Vector3>();
            var normals = new List<Vector3>();
            var triangles = new List<int>();
            var currentIndex = 0;

            foreach (var polygon in tree)
            {
                var normal = polygon.Plane.Normal;
                var vertexCount = polygon.Vertices.Length;

                for (var i = 2; i < vertexCount; i++)
                {
                    vertices.Add(polygon.Vertices[0]);
                    vertices.Add(polygon.Vertices[i - 1]);
                    vertices.Add(polygon.Vertices[i]);

                    normals.Add(normal);
                    normals.Add(normal);
                    normals.Add(normal);

                    triangles.Add(currentIndex);
                    triangles.Add(currentIndex + 1);
                    triangles.Add(currentIndex + 2);

                    currentIndex += 3;
                }
            }

            var mesh = new Mesh
            {
                indexFormat = UnityEngine.Rendering.IndexFormat.UInt32
            };

            mesh.SetVertices(vertices);
            mesh.SetNormals(normals);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

        private static void Fragmentize(Mesh sourceMesh, out List<Vector3> vertices, out List<Vector3> normals)
        {
            var triangles = sourceMesh.triangles;
            var sourceVertices = sourceMesh.vertices;
            var sourceNormals = sourceMesh.normals;

            vertices = new List<Vector3>(sourceVertices.Length);
            normals = new List<Vector3>(sourceNormals.Length);

            foreach (var t in triangles)
            {
                vertices.Add(sourceVertices[t]);
                normals.Add(sourceNormals[t]);
            }
        }

        public class Node
        {
            private Node _negativeFront;
            private Node _negativeBack;
            private Plane _plane;
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

                _plane?.Flip();
                _negativeFront?.Invert();
                _negativeBack?.Invert();
                (_negativeFront, _negativeBack) = (_negativeBack, _negativeFront);
            }

            private List<Polygon> ClipPolygons(List<Polygon> polygons)
            {
                if (_plane == null) return new List<Polygon>(polygons);
                var pf = new List<Polygon>();
                var pb = new List<Polygon>();
                foreach (var p in polygons)
                {
                    var o = _plane.SplitPolygon(p);
                    if (o.onFront != null) pf.Add(o.onFront);
                    if (o.onBack != null) pb.Add(o.onBack);
                    if (o.front != null) pf.Add(o.front);
                    if (o.back != null) pb.Add(o.back);
                }

                if (_negativeFront != null) pf = _negativeFront.ClipPolygons(pf);
                if (_negativeBack != null) pb = _negativeBack.ClipPolygons(pb);
                else pb.Clear();
                pf.AddRange(pb);
                return pf;
            }

            public void ClipTo(Node pair)
            {
                _polygons = pair.ClipPolygons(_polygons);
                _negativeFront?.ClipTo(pair);
                _negativeBack?.ClipTo(pair);
            }

            public List<Polygon> GetPolygonData()
            {
                var clone = Clone(_polygons);
                if (_negativeFront != null) clone.AddRange(_negativeFront.GetPolygonData());
                if (_negativeBack != null) clone.AddRange(_negativeBack.GetPolygonData());
                return clone;
            }

            public void Build(List<Polygon> polygons)
            {
                if (polygons.Count == 0) return;
                _plane ??= new Plane(polygons[0].Plane);
                var pf = new List<Polygon>();
                var pb = new List<Polygon>();
                foreach (var t in polygons)
                {
                    var o = _plane.SplitPolygon(t);
                    if (o.onFront != null) _polygons.Add(o.onFront);
                    if (o.onBack != null) _polygons.Add(o.onBack);
                    if (o.front != null) pf.Add(o.front);
                    if (o.back != null) pb.Add(o.back);
                }

                if (pf.Count > 0)
                {
                    _negativeFront ??= new Node();

                    _negativeFront.Build(pf);
                }

                if (pb.Count <= 0) return;
                _negativeBack ??= new Node();

                _negativeBack.Build(pb);
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
