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
        private bool _fired;

        private const float ArrowStingOffset = 0.05f;

        private void Start()
        {
            
        }

        private void Update()
        {
            var arrowStringPosition = bow.GetArrowStringPosition();
            if(_fired)
            {
                GameObject.Destroy(this.gameObject);
            }
            if (_isGrabbed && Vector3.Distance(stingTransform.position, arrowStringPosition) <= ArrowStingOffset)
            {
                _isAttachedToBow = true;
            }

            if (_isAttachedToBow)
            {
                transform.rotation = bow.transform.rotation;
                transform.Rotate(0,0,-90);
            
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

        public void FireArrow()
        {
            if(!bow.IsBowTensed() || !_isAttachedToBow)
            {
                return;
            }
            _isAttachedToBow = false;
            _fired = true;
        }

    }
}