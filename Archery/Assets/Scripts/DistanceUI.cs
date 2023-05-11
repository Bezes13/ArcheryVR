using System;
using TMPro;
using UnityEngine;

public class DistanceUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI distance;
    [SerializeField] private Bow bow;

    private void Update()
    {
        distance.text = Math.Round(Vector3.Distance(transform.position, bow.transform.position)) + " Meter";
    }
}