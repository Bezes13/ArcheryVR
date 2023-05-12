using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowSpawner : MonoBehaviour
{
    [SerializeField] private List<Transform> positions;
    [SerializeField] private Arrow prefab;
    public List<(Arrow, Transform)> arrows;
    // Start is called before the first frame update
    void Start()
    {
        arrows = new List<(Arrow, Transform)>();
        foreach(var pos in positions)
        {
            Arrow arr = Instantiate(prefab, pos);
            arr.gameObject.SetActive(true);
            arrows.Add((arr, pos));
        }
    }

    // Update is called once per frame
    void Update()
    {
        (Arrow, Transform) delete = (null, null);
        Arrow newOne = null;
        foreach(var (arrow, trans) in arrows)
        {
            if (arrow.fired)
            {
                newOne = Instantiate(prefab, trans);
                newOne.gameObject.SetActive(true);
                delete = (arrow, trans);
                
            }
        }

        if (delete.Item1 != null)
        {
            arrows.Remove(delete);
            arrows.Add((newOne, delete.Item2));
        }
    }
}