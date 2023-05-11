using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        if( bow.GetArrowWoodPosition().y < bow.GetArrowStringPosition().y){
            angle.text = "-";
        }
        angle.text = (bow.GetArrowWoodPosition().y < bow.GetArrowStringPosition().y ? "-" : "") +
                     Math.Round(Vector3.Angle(projectorvecdir, new Vector3(projectorvecdir.x, 0, projectorvecdir.z))) + "°";
    }
}