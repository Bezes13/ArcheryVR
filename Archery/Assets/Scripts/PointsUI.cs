using System;
using TMPro;
using UnityEngine;

/// <summary>
/// The Ui which shows the current Points and the wind to the player.
/// </summary>
public class PointsUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pointsText;
    [SerializeField] private TextMeshProUGUI windText;
    [SerializeField] private TextMeshProUGUI windText2;
    [SerializeField] private Model model;

    private void Update()
    {
        pointsText.text = model.GetPoints().ToString();
        var dir = model.GetWind();
        var side = Math.Abs(Math.Round(dir.x, 1)) + "ms " + (dir.x < 0 ? "right" : "left");
        var high = Math.Abs(Math.Round(dir.y, 1)) + "ms " + (dir.y < 0 ? "down" : "up");
        if (Math.Round(dir.y, 2) == 0)
        {
            windText2.text = side;
        }
        else
        {
            windText.text = side;
            windText2.text = high;
        }
    }
}