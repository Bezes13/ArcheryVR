using UnityEngine;

[CreateAssetMenu(fileName = "Model", menuName = "ScriptableObjects/Model", order = 1)]
public class Model : ScriptableObject
{
    private int Points = 0;
    private Vector3 _windVelocity;

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
        return _windVelocity;
    }
}