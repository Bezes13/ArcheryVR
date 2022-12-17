using UnityEngine;

namespace Unity.Template.VR
{
    public struct ArrowFiredEvent
    {
        public Vector3 Direction;
        public float Force;
        public Quaternion StartRotation;
    }
}