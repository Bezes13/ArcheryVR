using System;
using UnityEngine;

namespace Unity.Template.VR
{
    public class Target : MonoBehaviour
    {
        private Rigidbody _rigidbody;
        public Model Model;
        public int points;

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        public int GetPoints()
        {
            return points;
        }
    }
}