using UnityEngine;

/// <summary>
/// Handles the Tutorial-UI
/// </summary>
public class Tutorial : MonoBehaviour
{
    [SerializeField] private GameObject first;
    [SerializeField] private GameObject second;
    [SerializeField] private GameObject third;
    [SerializeField] private GameObject last;
    [SerializeField] private ArrowSpawner spawner;
    
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
