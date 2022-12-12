using System;
using Oculus.Interaction;
using UnityEngine;

namespace Unity.Template.VR
{
    public class Arrow : MonoBehaviour
    {
        [SerializeField]
        private Transform stingTransform;
        [SerializeField]
        private Bow bow;
        
        private bool _isGrabbed;
        private bool _isAttachedToBow;

        private const float ArrowStingOffset = 1.0f;

        private void Start()
        {
            
        }

        private void Update()
        {
            var arrowStringPosition = bow.GetArrowStringPosition();
            if (_isGrabbed && Vector3.Distance(stingTransform.position, arrowStringPosition) <= ArrowStingOffset)
            {
                _isAttachedToBow = true;
            }

            if (_isAttachedToBow)
            {
                transform.position = arrowStringPosition + transform.position - stingTransform.position;
            }
        }

        public void Grab()
        {
            _isGrabbed = true;
        }
        public void UnGrab()
        {
            _isGrabbed = false;
        }
    }
}