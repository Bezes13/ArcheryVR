using System;
using UnityEngine;

public class Bow : MonoBehaviour
{
    private const float MaxStingRange = 0.0738f;
    
    private Animator _animator;
    private bool isArrowAttached;

    [SerializeField] private Transform arrowStringPosition;
    [SerializeField] private Transform arrowWoodPosition;

    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame

    void Update()
    {
        arrowStringPosition.localPosition = new Vector3(0,Mathf.Max(-MaxStingRange, arrowStringPosition.localPosition.y),0);
        var value = babadam(arrowStringPosition.localPosition.y);
        Debug.Log(arrowStringPosition.localPosition.y + " - " + value);
        _animator.SetFloat("bow", value);
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
        return Mathf.Min(1.0f,(arrowStringPosition.localPosition.y + 0.01075402f) / -(MaxStingRange-0.01075402f));
    }

    public void ResetSting()
    {
        arrowStringPosition.localPosition = new Vector3(0,-0.01075402f,0);
    }

    public void ShotFired()
    {
        _animator.SetTrigger("shot");
    }

    public float babadam(float y)
    {
        float[] myNum = {
            -0.01075402f,
            -0.01096038f,
            -0.01180249f,
            -0.01364458f,
            -0.01671125f,
            -0.02091927f,
            -0.02597514f,
            -0.03288396f,
            -0.04263402f,
            -0.0542632f,
            -0.06590831f,
            -0.07389119f};
        
        if (y >= myNum[0])
        {
            return 0;
        }
        
        for (int i = 0; i < myNum.Length-1; i++)
        {
            if (y > myNum[i])
            {
                var f = helper(myNum[i],myNum[i+1],i);
                
                return (f/18.0f);
            }
        }
        return 11f/18f;
        
        float helper(float lower, float higher, float time)
        {
            return time +  (lower - y)/ (lower - higher);
        }
    }
}
