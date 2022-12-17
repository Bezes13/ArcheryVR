using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour
{
    private const float MaxStingRange = 0.0738f;
    
    private Animator _animator;
    private bool isArrowAttached;

    [SerializeField] private Transform arrowStringPosition;
    [SerializeField] private Transform arrowWoodPosition;


    // Start is called before the first frame update

    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame

    void Update()
    {
        arrowStringPosition.localPosition = new Vector3(0,Mathf.Max(-MaxStingRange, arrowStringPosition.localPosition.y),0);
    }

    public Vector3 GetArrowStringPosition()
    {
        arrowStringPosition.localPosition = new Vector3(0,arrowStringPosition.localPosition.y,0);
        return arrowStringPosition.position;
    }

    public Vector3 GetArrowWoodPosition()
    {
        return arrowWoodPosition.position;
    }

    public void attachArrow()
    {
        isArrowAttached = true;
    }

    public bool IsBowTensed()
    {
        return arrowStringPosition.localPosition.y < 0;
    }

    public float GetBowForce()
    {
        return -arrowStringPosition.localPosition.y / MaxStingRange;
    }
}
