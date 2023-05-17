using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class Model : MonoBehaviour
{
    private int _points;
    private Vector3 _windDirection;
    public float WindForce = 10.0f;

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

    public void NextScene(){
        Debug.Log("Pressed Button");
        SceneManager.LoadScene("Game_Scene");
    }
}