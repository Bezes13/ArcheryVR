using System.Collections.Generic;
using UnityEngine;

public class ShatterObject : MonoBehaviour
{
    [SerializeField] protected PhysicMaterial phy;
    [SerializeField] protected Material mat;
    [SerializeField] protected GameObject tgt;
    [SerializeField] protected int num;
    [SerializeField, Range(0.1f, 10f)] float scale = 1f;
    [SerializeField, Range(1, 10)] int points = 1;
    private List<Rigidbody> objects = new List<Rigidbody>();

    void Break() {
        var stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();

        var delaunay = new Delaunay(num, scale, tgt);
        var voronoi = new VoronoiGraph(delaunay.Nodes.ToArray());
        var tree = MeshCreater.GenCsgTree(tgt.transform);

        foreach (var node in voronoi.nodes) {
            var mesh = node.Value.Meshilify();
            var t1 = Clone(tree);
            var t2 = MeshCreater.GenCsgTree(Matrix4x4.TRS(node.Value.center * 1.01f, Quaternion.identity, Vector3.one), mesh, true);
            var o = MeshCreater.Meshing(MeshCreater.Intersection(t1, t2));
            var g = new GameObject();
            var filter = g.AddComponent<MeshFilter>();
            var meshRenderer = g.AddComponent<MeshRenderer>();
            var meshCollider = g.AddComponent<MeshCollider>();
            var item = g.AddComponent<Rigidbody>();
            meshCollider.convex = true;
            meshCollider.sharedMaterial = phy;
            meshCollider.sharedMesh = o;
            item.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            item.useGravity = true;
            item.isKinematic = false;
            filter.mesh = o;
            var _mat = new Material(mat);
            // _mat.SetColor("_Color", Color.HSVToRGB(Random.value, 1, 1));
            meshRenderer.sharedMaterial = _mat;
            objects.Add(item);
        }

        stopwatch.Stop();
        Debug.Log("generate csg: " + stopwatch.ElapsedMilliseconds + "ms");
        tgt.SetActive(false);
    }

    private static MeshCreater.Polygon[] Clone(MeshCreater.Polygon[] src)
    {
        var l = src.Length;
        var d = new MeshCreater.Polygon[l];
        for (var i = 0; i < l; i++) d[i] = new MeshCreater.Polygon(src[i]);
        return d;
    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log("koll");
        var obj = other.gameObject.GetComponent<Arrow>();
            
        if(obj != null){
            Break();
        }
    }

    public int GetPoints(){
        return points;
    }
}