using System.Collections.Generic;
using UnityEngine;

public class ArrowSpawner : MonoBehaviour
{
    [SerializeField] private List<Transform> positions;
    [SerializeField] private Arrow prefab;
    public List<(Arrow, Transform)> arrows;

    private void Start()
    {
        arrows = new List<(Arrow, Transform)>();
        foreach (var pos in positions)
        {
            var arr = Instantiate(prefab, pos);
            arr.gameObject.SetActive(true);
            arrows.Add((arr, pos));
        }
    }

    public void Update()
    {
        (Arrow, Transform) delete = (null, null);
        Arrow newOne = null;
        foreach (var (arrow, trans) in arrows)
        {
            if (!arrow.fired) continue;
            newOne = Instantiate(prefab, trans);
            newOne.gameObject.SetActive(true);
            delete = (arrow, trans);
        }

        if (delete.Item1 == null) return;
        arrows.Remove(delete);
        arrows.Add((newOne, delete.Item2));
    }
}