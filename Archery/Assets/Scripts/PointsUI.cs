using System;
using TMPro;
using UnityEngine;

public class PointsUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pointsText;
    [SerializeField] private TextMeshProUGUI windText;
    [SerializeField] private Model model;

    private void Update()
    {
        pointsText.text = model.GetPoints().ToString();
        windText.text = model.GetWind().ToString();
    }
}