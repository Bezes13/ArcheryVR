using System.Collections.Generic;
using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    [SerializeField] private List<Transform> positions;
    [SerializeField] private List<ShatterObject> prefabs;
    private List<(ShatterObject, int)> _targets;

    private int _nextPrefab;

    private const int TargetsOnField = 5;

    // Start is called before the first frame update
    void Start()
    {
        _targets = new List<(ShatterObject, int)>();
        for (var i = 0; i < TargetsOnField; i++)
        {
            var target = Instantiate(prefabs[i % prefabs.Count], positions[i]);
            target.gameObject.SetActive(true);
            _targets.Add((target, i));
        }
    }

    // Update is called once per frame
    private void Update()
    {
        (ShatterObject, int) delete = (null, 0);
        (ShatterObject, int) toAdd = (null, 0);
        foreach (var (target, i) in _targets)
        {
            if (target.gameObject.activeSelf)
            {
                continue;
            }

            var nextSpot = GetNextSpot();
            var newOne = Instantiate(prefabs[_nextPrefab], positions[nextSpot]);
            _nextPrefab = (_nextPrefab + 1) % prefabs.Count;
            newOne.gameObject.SetActive(true);
            toAdd = (newOne, nextSpot);
            delete = (target, i);
            break;
        }

        if (delete.Item1 == null) return;
        _targets.Remove(delete);
        _targets.Add(toAdd);
    }

    private int GetNextSpot()
    {
        var biggestSpot = 0;
        var givenSpots = new List<int>();
        foreach (var (_, i) in _targets)
        {
            if (i > biggestSpot)
            {
                biggestSpot = i;
            }

            givenSpots.Add(i);
        }

        if (biggestSpot + 1 < positions.Count)
        {
            return biggestSpot + 1;
        }

        for (var j = 0; j < positions.Count; j++)
        {
            if (givenSpots.Contains(j))
            {
                continue;
            }

            return j;
        }

        return 0;
    }
}