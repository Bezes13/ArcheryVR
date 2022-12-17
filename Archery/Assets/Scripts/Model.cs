using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

namespace Unity.Template.VR
{
    [CreateAssetMenu(fileName = "Model", menuName = "ScriptableObjects/Model", order = 1)]
    public class Model : ScriptableObject
    {
        private int Points = 0;

        public void AddPoints(int points)
        {
            Points += points;
        }
    }
}