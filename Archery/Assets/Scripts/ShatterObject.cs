using System.Collections.Generic;
using UnityEngine;
using System;

public class ShatterObject : MonoBehaviour
{
    [SerializeField] protected PhysicMaterial phy;
    [SerializeField] private ParticleSystem starExplosion;
    [SerializeField] private AudioSource audioSource; 
    [SerializeField] private AudioClip deadSound;
    [SerializeField] protected Material mat;
    [SerializeField] protected GameObject tgt;
    [SerializeField] protected int num;
    [SerializeField, Range(0.1f, 10f)] float scale = 1f;
    [SerializeField, Range(1, 10)] int points = 1;
    private List<Rigidbody> objects = new List<Rigidbody>();

    void Break() {
        var delaunay = new Delaunay(num, scale, tgt);
        var voronoi = new VoronoiGraph(delaunay.Nodes.ToArray());
        var realMesh = MeshCreater.GenCsgTree(tgt.transform);

        foreach (var node in voronoi.nodes) {
            var mesh = node.Value.Meshilify();
            var t1 = Clone(realMesh);
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
            meshRenderer.sharedMaterial = _mat;
            item.gameObject.transform.position += tgt.transform.position;
            objects.Add(item);
        }
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
        var obj = other.gameObject.GetComponent<Arrow>();
            
        if(obj != null){
            starExplosion.gameObject.SetActive(true);
            starExplosion.Play();
            
            Break();
        }
    }

    public int GetPoints(){
        Bow bow =  UnityEngine.Object.FindObjectOfType<Bow>();
        audioSource = bow.gameObject.GetComponent<AudioSource>();
        audioSource.clip = deadSound;
        audioSource.Play();
        return (int) Math.Round(Vector3.Distance(transform.position, bow.transform.position));
    }
}