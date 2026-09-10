using Script.Tools.Managers;
using UnityEngine;

namespace Script.Pulley
{
    public class PulleyRotation : MonoBehaviour
    {
        private Rigidbody rb;
        private HingeJoint hj;
        private GameObject maxMassObject;
        void Start()
        {
            rb = GetComponent<Rigidbody>();
            hj = GetComponent<HingeJoint>();
           
        }
        
        //施加扭矩
        private void AddTorque(Vector3 torqueDirection, float torqueSize)
        {
            rb.freezeRotation = false;
            rb.AddRelativeTorque(torqueDirection * torqueSize, ForceMode.Impulse);
        }
        //扭矩方向
        private Vector3 DirectionForTorQue()
        {
            if (maxMassObject == null)
            {
                return Vector3.zero;
            }
            return maxMassObject.CompareTag("FreePlatform") ? -hj.axis : hj.axis;
        }
        //重置旋转速度
        private void ResetRotationSpeed(GameObject obj = null)
        {
            rb.angularVelocity = Vector3.zero;
            rb.freezeRotation = true;
        }
        private void GetMaxMassObject(GameObject obj)
        {
            maxMassObject = obj;
            AddTorque(DirectionForTorQue(), rb.mass);
        }

        void OnEnable()
        {
            EventsManager.OnPulleyStable += GetMaxMassObject;
            EventsManager.OnRayTest += ResetRotationSpeed;
        }

        void OnDisable()
        {
            EventsManager.OnPulleyStable -= GetMaxMassObject;
            EventsManager.OnRayTest -= ResetRotationSpeed;
        }
    }
}
