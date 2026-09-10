using System.Collections;
using Script.Tools.Managers;
using UnityEngine;

namespace Script.Pry
{
    public class BalanceCondition : MonoBehaviour
    {
        private float rotationError = 3f;
        private float angularSpeedError = 1.5f;
        [SerializeField]
        private Rigidbody rb;
        
        private float interaval = 0.2f;
        float timer;
        private float testTime = 1.6f;//总检测时间
        private bool isBalance;
        private bool hasBalance;
        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Weight") && timer == 0)
            {
                StartCoroutine(ContinuousBalance());
            }
            BalanceEvent();
        }
        
        //杠杆平衡的事件
        private void BalanceEvent()
        {
            if (isBalance)
            {
                if(!hasBalance)
                {
                    EventsManager.TriggerPryStable(gameObject);
                    hasBalance = true;
                }
            }
            else
            {
                if (hasBalance)
                {
                    EventsManager.TriggerPryUnstable(gameObject);
                    hasBalance = false;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Weight") && timer == 0)
            {
                StartCoroutine(ContinuousBalance());
            }
        }
        
        //单次平衡检测
        private bool TestBalance()
        {
            float currentRotation = Vector3.Angle(transform.up, Vector3.up);
            float angularSpeed = rb.angularVelocity.magnitude * Mathf.Rad2Deg;
            bool isStable = currentRotation < rotationError && angularSpeed < angularSpeedError;
            return isStable;
        }
        
        //连续平衡检测
        IEnumerator  ContinuousBalance()
        {
            bool isStable = true;
            while (timer < testTime)
            {
                if (!TestBalance())
                {
                    isStable = false;
                    break;
                }
                timer += interaval;
                yield return new WaitForSeconds(interaval);   
            }
            isBalance = isStable;
            timer = 0;
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, Vector3.up * 5);
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, transform.up * 5);
        }
    }
}
