using UnityEngine;

namespace Script
{
    public class StartPryRotation : MonoBehaviour
    {
        private HingeJoint hinge;
        private Transform fulcrum;
        private JointMotor motor;
        private float velocity = 10;
        private float force = 1;
    
        void Start()
        {
            hinge = GetComponent<HingeJoint>();
            hinge.anchor = transform.GetChild(0).localPosition;
            fulcrum = hinge.connectedBody.transform;
            hinge.connectedAnchor = fulcrum.GetChild(0).localPosition ;
            motor = hinge.motor;
            motor.force = force;
            motor.targetVelocity = velocity;
        
            hinge.motor = motor;
        }

        void FixedUpdate()
        {
            HandleRotation();
        }

        void HandleRotation()
        {
            float currentAngle = hinge.angle;

            if (currentAngle >= hinge.limits.max * 0.95)
            {
                motor.targetVelocity = -velocity;
            }
            else if (currentAngle <= hinge.limits.min * 0.95)
            {
                motor.targetVelocity = velocity;
            }
        
            hinge.motor = motor;
        }
    }
}
