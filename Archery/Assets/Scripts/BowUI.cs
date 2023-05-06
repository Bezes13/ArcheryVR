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
            power.text = Math.Round(100 * bow.GetBowForce()).ToString();
            Vector3 projectorvec = bow.GetArrowWoodPosition() - bow.GetArrowStringPosition();
            Vector3 projectorvecdir = projectorvec.normalized;
            angle.text =
                Math.Round(Vector3.Angle(projectorvecdir, new Vector3(projectorvecdir.x, 0, projectorvecdir.z))) + "°";
        }
    }
}