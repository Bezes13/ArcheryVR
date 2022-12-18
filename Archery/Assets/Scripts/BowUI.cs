using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.Template.VR
{
    public class BowUI : MonoBehaviour
    {
        [SerializeField] private Bow bow;
        [SerializeField] private TextMeshProUGUI power;
        [SerializeField] private TextMeshProUGUI angle;

        private void Update()
        {
            power.text = bow.GetBowForce().ToString();
            angle.text = Vector3.Angle(bow.GetArrowStringPosition(), bow.GetArrowWoodPosition()) + "°";
        }
    }
}