using UnityEngine;
using System;

public class Model : MonoBehaviour
{
    private int _points;
    private Vector3 _windDirection;
    private const float WindForce = 10.0f;

    private void Start()
    {
        _windDirection = new Vector3(UnityEngine.Random.value - 0.5f, UnityEngine.Random.value - 0.5f,
            UnityEngine.Random.value - 0.5f);
    }

    public void AddPoints((int, int) points)
    {
        _points += points.Item1;
        _points += points.Item2;
    }

    public int GetPoints()
    {
        return _points;
    }

    public Vector3 GetWind()
    {
        return _windDirection * WindForce;
    }
}