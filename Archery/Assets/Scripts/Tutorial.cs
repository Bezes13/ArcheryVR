using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] GameObject first;
    [SerializeField] GameObject second;
    [SerializeField] GameObject third;
    [SerializeField] GameObject last;
    [SerializeField] Bow bow;
    [SerializeField] private ArrowSpawner spawner;
    // Start is called before the first frame update
    void Start()
    {
        first.SetActive(true);
        second.SetActive(false);
        third.SetActive(false);
        last.SetActive(false);
    }

    public void PickUpBow()
    {
        second.SetActive(true);
    }

    public void KnockArrow()
    {
        third.SetActive(true);
    }

    public void FireArrow()
    {
        last.SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {
        if(last.activeSelf){
            return;
        }
        foreach (var (arrow, _) in spawner.arrows)
        {
            if (arrow.isAttachedToBow)
            {
                KnockArrow();
            }
        }
    }
}
