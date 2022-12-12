using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour
{
    private Animator _animator;

    [SerializeField] private Transform arrowStringPosition;
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 GetArrowStringPosition()
    {
        return arrowStringPosition.position;
    }
}
