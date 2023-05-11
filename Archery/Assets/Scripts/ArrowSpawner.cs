using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Template.VR;
namespace Unity.Template.VR
{
public class ArrowSpawner : MonoBehaviour
{
    [SerializeField] private List<Transform> positions;
    [SerializeField] private Arrow prefab;
    public List<Arrow> arrows;
    // Start is called before the first frame update
    void Start()
    {
        foreach(var pos in positions)
        {
            Arrow arr = Instantiate(prefab, pos);
            arr.gameObject.SetActive(true);
            arrows.Add(arr);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}}
