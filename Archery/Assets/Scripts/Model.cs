using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// Class thath generates the wind for the game and saves the current Points
/// </summary>
public class Model : MonoBehaviour
{
    public const int MaxArrows = 5;
    private int _points;
    private int _arrows;
    private Vector3 _windDirection;
    public float windForce = 10.0f;
    public GameObject Replay;
    public GameObject Resultscreen;
    public List<Rigidbody> toDestroy;
    public float targetTime = 2.0f;
    private bool pause;

    private void Start()
    {
        _windDirection = new Vector3(Random.value - 0.5f, Random.value - 0.5f,
            Random.value - 0.5f);
        toDestroy = new List<Rigidbody>();
    }

    private void Update()
    {
 
        if(_arrows >= MaxArrows){
            targetTime -= Time.deltaTime;
            
            if (targetTime <= 0.0f)
            {
                ShowResult();
            }
        }else
        {
            targetTime = 2.0f;
        }
    }

    public void AddPoints((int, int) points)
    {
         if(pause){return;}
        _points += points.Item1;
        _points += points.Item2;
    }

    public void IncArrow(int count = 1){
        if(pause){return;}
        _arrows += count;
    }

    public int GetArrowCount(){
        return _arrows;
    }
    public int GetMaxArrowCount(){
        return MaxArrows;
    }
    public int GetPoints()
    {
        return _points;
    }

    public Vector3 GetWind()
    {
        return _windDirection * windForce;
    }

    public void NextScene(){
        Debug.Log("Pressed Button");
        SceneManager.LoadScene("Game_Scene");
    }

    public void HideResult()
    {
        Replay.SetActive(false);
        Resultscreen.SetActive(false);
        _points = 0;
        _arrows = 0;
        _windDirection = new Vector3(Random.value - 0.5f, Random.value - 0.5f,
            Random.value - 0.5f);
        pause = false;    
        foreach (var item in toDestroy)
        {
            GameObject.Destroy(item.gameObject);
        }
        toDestroy = new List<Rigidbody>();
    }

    public void ShowResult()
    {
        Replay.SetActive(true);
        Resultscreen.SetActive(true);
        pause = true;  
    }
}