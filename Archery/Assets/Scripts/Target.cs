using System;
using UnityEngine;

namespace Unity.Template.VR
{
    public class Target : MonoBehaviour
    {
        [SerializeField]
        private Transform middle;
        [SerializeField]
        private Transform first;
        [SerializeField]
        private Transform second;
        [SerializeField]
        private Transform third;
        [SerializeField]
        private Transform fourth;
        [SerializeField]
        private Transform fifth;

        private Rigidbody _rigidbody;
        public Model Model;
        public int points;

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        public int GetPoints(Vector3 hit)
        {
            float disToMiddle = Vector3.Distance(middle.position,hit);
            if(disToMiddle <= Vector3.Distance(middle.position,first.position)){
                return 50;
            }
            if(disToMiddle <= Vector3.Distance(middle.position,second.position)){
                return 40;
            }
            if(disToMiddle <= Vector3.Distance(middle.position,third.position)){
                return 30;
            }
            if(disToMiddle <= Vector3.Distance(middle.position,fourth.position)){
                return 20;
            }
            if(disToMiddle <= Vector3.Distance(middle.position,fifth.position)){
                return 10;
            }
            return 0;
        }
    }
}