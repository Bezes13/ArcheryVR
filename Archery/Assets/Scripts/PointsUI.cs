using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.Template.VR
{
    public class PointsUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI pointsText;
        [SerializeField] private TextMeshProUGUI windText;
        [SerializeField] private TextMeshProUGUI distance;
        [SerializeField] private Model model;
        [SerializeField] private Target target;
        [SerializeField] private Bow bow;

        private void Update()
        {
            pointsText.text = model.GetPoints().ToString();
            windText.text = model.GetWind().ToString();
            distance.text = Math.Round(Vector3.Distance(target.transform.position, bow.transform.position)) +" Meter";
        }
    }
}