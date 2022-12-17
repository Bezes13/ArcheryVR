using System;
using UnityEngine;

namespace Unity.Template.VR
{
    public class Arrow : MonoBehaviour
    {
        private const float ArrowStingOffset = 0.05f;
        
        [SerializeField]
        private Transform stingTransform;
        [SerializeField]
        private Bow bow;
        
        private bool _isGrabbed;
        private bool _isAttachedToBow;
        private bool _fired;
        private ArrowFiredEvent _firedEvent;
        
        [SerializeField] private Transform TargetObjectTF;
        [Range(1.0f, 15.0f)] public float TargetRadius;
        [Range(20.0f, 75.0f)] public float LaunchAngle;
        [Range(0.0f, 10.0f)] public float TargetHeightOffsetFromGround;
        public bool RandomizeHeightOffset;

        // state
        private bool bTargetReady;
        private bool bTouchingGround;

        // cache
        private Rigidbody rigid;
        private Vector3 initialPosition;
        private Quaternion initialRotation;

        public Model model;
        public bool _shot = false;

        private void Start()
        {
            rigid = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (_shot)
            {
                return;
            }
            var arrowStringPosition = bow.GetArrowStringPosition();
            if(_fired)
            {
                
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
            
            if (_fired)
            {
                // update the rotation of the projectile during trajectory motion
                transform.rotation = Quaternion.LookRotation(rigid.velocity) * initialRotation;
            }
        }
        
        float GetPlatformOffset()
        {
            float platformOffset = 0.0f;
            // 
            //          (SIDE VIEW OF THE PLATFORM)
            //
            //                   +------------------------- Mark (Sprite)
            //                   v
            //                  ___                                          -+-
            //    +-------------   ------------+         <- Platform (Cube)   |  platformOffset
            // ---|--------------X-------------|-----    <- TargetObject     -+-
            //    +----------------------------+
            //

            // we're iterating through Mark (Sprite) and Platform (Cube) Transforms. 
            foreach (Transform childTransform in TargetObjectTF.GetComponentsInChildren<Transform>())
            {
                // take into account the y-offset of the Mark gameobject, which essentially
                // is (y-offset + y-scale/2) of the Platform as we've set earlier through the editor.
                if (childTransform.name == "Mark")
                {
                    platformOffset = childTransform.localPosition.y;
                    break;
                }
            }
            return platformOffset;
        }

        private void FlyingArrow()
        {
                // think of it as top-down view of vectors: 
                //   we don't care about the y-component(height) of the initial and target position.
                Vector3 projectileXZPos = new Vector3(transform.position.x, 0.0f, transform.position.z);
                Vector3 targetXZPos = new Vector3(TargetObjectTF.position.x, 0.0f, TargetObjectTF.position.z);
        
                // rotate the object to face the target
                transform.LookAt(targetXZPos);

                // shorthands for the formula
                float R = Vector3.Distance(projectileXZPos, targetXZPos);
                float G = Physics.gravity.y;
                float tanAlpha = Mathf.Tan(LaunchAngle * Mathf.Deg2Rad);
                float H = (TargetObjectTF.position.y + GetPlatformOffset()) - transform.position.y;

                // calculate the local space components of the velocity 
                // required to land the projectile on the target object 
                float Vz = Mathf.Sqrt(G * R * R / (2.0f * (H - R * tanAlpha)) );
                float Vy = tanAlpha * Vz;

                // create the velocity vector in local space and get it in global space
                Vector3 localVelocity = new Vector3(0f, Vy, Vz);
                Vector3 globalVelocity = transform.TransformDirection(localVelocity);

                // launch the object by setting its initial velocity and flipping its state
                rigid.velocity = globalVelocity;
                bTargetReady = false;
        }

        public void Grab()
        {
            _isGrabbed = true;
        }
        
        public void UnGrab()
        {
            _isGrabbed = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            Target target = other.GetComponent<Target>();

            if (target == null)
            {
                return;
            }
            model.AddPoints(target.GetPoints());
            rigid.velocity = Vector3.zero;
            _shot = true;
        }

        public void FireArrow()
        {
            if(!bow.IsBowTensed() || !_isAttachedToBow)
            {
                return;
            }
            _isAttachedToBow = false;
            _fired = true;
            Vector3 projectorvec = bow.GetArrowWoodPosition().normalized - bow.GetArrowStringPosition().normalized;
            Vector3 projectorvecdir = projectorvec.normalized;
            Quaternion startRot=transform.rotation;
            _firedEvent = new ArrowFiredEvent()
            {
                Direction = projectorvecdir,
                StartRotation = startRot,
                Force = bow.GetBowForce()
            };
            initialPosition = transform.position;
            initialRotation = transform.rotation;
            
            FlyingArrow();
        }

    }
}