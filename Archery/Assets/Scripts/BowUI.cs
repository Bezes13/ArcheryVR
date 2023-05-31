using System;
using System.Globalization;
using TMPro;
using UnityEngine;

/// <summary>
/// The BowUI handles the UI for the Bow, it shows the
///     power (how much the string is pulled back) and
///     the angle of the Bow to the Ground. 
/// </summary>
public class BowUI : MonoBehaviour
{
    [SerializeField] private Bow bow;
    [SerializeField] private TextMeshProUGUI power;
    [SerializeField] private TextMeshProUGUI angle;

    private void Update()
    {
        power.text = Math.Round(100 * bow.GetBowForce()).ToString(CultureInfo.InvariantCulture);
        Vector3 projectorVec = bow.GetArrowWoodPosition() - bow.GetArrowStringPosition();
        Vector3 projectorVecNorm = projectorVec.normalized;
        if (bow.GetArrowWoodPosition().y < bow.GetArrowStringPosition().y)
        {
            angle.text = "-";
        }

        angle.text = (bow.GetArrowWoodPosition().y < bow.GetArrowStringPosition().y ? "-" : "") +
                     Math.Round(Vector3.Angle(projectorVecNorm, new Vector3(projectorVecNorm.x, 0, projectorVecNorm.z)))+ "°";
    }
}