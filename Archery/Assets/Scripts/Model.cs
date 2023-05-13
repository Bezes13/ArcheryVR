using UnityEngine;
using System;
public class Model : MonoBehaviour {
    
    private int Points = 0;
    private Vector3 _windDirection;
    private const float windForce = 10.0f;

    private void Start() {
        _windDirection = new Vector3(UnityEngine.Random.value -0.5f, UnityEngine.Random.value-0.5f, UnityEngine.Random.value-0.5f);
        
    }

    public void AddPoints(int points)
    {
        Points += points;
    }

    public int GetPoints()
    {
        return Points;
    }

    public Vector3 GetWind()
    {
        return (_windDirection * (float)windForce);
    }
}