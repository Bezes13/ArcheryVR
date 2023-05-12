using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSpawner : MonoBehaviour
{

    [SerializeField] private List<Transform> positions;
    [SerializeField] private List<ShatterObject> prefabs;
    public List<(ShatterObject, int)> targets;

    private int nextPrefab = 0;
    private const int TargetsOnField = 5;
    // Start is called before the first frame update
    void Start()
    {
        targets = new List<(ShatterObject, int)>();
        for(var i = 0; i < TargetsOnField; i++)
        {
            ShatterObject target = Instantiate(prefabs[i % prefabs.Count], positions[i]);
            target.gameObject.SetActive(true);
            targets.Add((target,  i));
        }
    }

    // Update is called once per frame
    void Update()
    {
       
        ShatterObject newOne = null;
        (ShatterObject, int) delete = (null,0);
        (ShatterObject, int) toAdd = (null,0);
        foreach(var (target, i) in targets)
        {
            if (target.gameObject.activeSelf)
            {
               continue;
            }
            var nextSpot = GetNextSpot();
            newOne = Instantiate(prefabs[nextPrefab], positions[nextSpot]);
            nextPrefab = (nextPrefab + 1) % prefabs.Count;
            newOne.gameObject.SetActive(true);
            toAdd = (newOne, nextSpot);
            delete = (target,i);
            break;
        }
        if (delete.Item1 != null){
            targets.Remove(delete);
            targets.Add(toAdd);
        }
        
    }

    private int GetNextSpot(){
        var biggestSpot = 0;
        var givenSpots = new List<int>();
        foreach(var (target, i) in targets){
            if (i> biggestSpot){
                biggestSpot = i;
            }
            givenSpots.Add(i);
        }
        if(biggestSpot + 1 < positions.Count){
            return biggestSpot + 1;
        }
        var samllestSpot = 0;
        for(int j = 0; j<positions.Count; j++){
            if(givenSpots.Contains(j)){
                continue;
            }
            return j;
        }
        return 0;
    }
}
