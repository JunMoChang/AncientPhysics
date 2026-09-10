using System.Collections;
using Script.Tools.Managers;
using UnityEngine;

namespace Script.Pulley
{
    public class PulleyBalanceController : MonoBehaviour
    {
        [SerializeField] private GameObject mobilePulley;
        [SerializeField] private GameObject carrierPlatform;
        private Rigidbody[] mPulleyRb;
        private float pulleySelfMass;
        [SerializeField] private GameObject freePlatform;
        private Rigidbody freePSelfRb;
        public GameObject MaxMassObject{get;private set;}
        public GameObject MaxHeightObject{get;private set;}
        
        private bool isStartCoroutine;
        void Start()
        { 
            mPulleyRb = mobilePulley.GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody rb in mPulleyRb)
            {
                pulleySelfMass += rb.mass;
            }
            freePlatform.TryGetComponent(out freePSelfRb);
        }

        void Update()
        {
            if (!isStartCoroutine)
            {
                StartCoroutine(Detection());
            }
        }
        //判断较重的一端
        private GameObject DetectionMaxMassObject()
        {
            float carrierMass = 0;
            float freeMass = 0;
            carrierPlatform.TryGetComponent(out LoadCalculation cLoad);
            freePlatform.TryGetComponent(out LoadCalculation fLoad);
            
            carrierMass = cLoad.Load + pulleySelfMass;
            freeMass = fLoad.Load + freePSelfRb.mass;
            
            return MaxMassObject = carrierMass > freeMass ? mobilePulley : freePlatform;
        }
        //判断较高的一端
        private GameObject DetectionMaxHeight()
        {
            if (mobilePulley.transform.position.y > freePlatform.transform.position.y)
            {
                return MaxHeightObject = mobilePulley;
            }
            return MaxHeightObject = freePlatform;
        }
        //判断载物端和自由端是否平衡
        private bool DetectionIsBalance()
        {
            DetectionMaxMassObject();
            DetectionMaxHeight();
            if (MaxMassObject == MaxHeightObject && MaxMassObject != null && MaxHeightObject != null)
            {
                return true;
            }
            return false;
        }
        
        IEnumerator Detection()
        {
            isStartCoroutine = true;
            if (DetectionIsBalance())
            {
                EventsManager.TriggerOnPulleyStable(MaxMassObject);
#if UNITY_EDITOR
                Debug.Log(MaxMassObject.name);
#endif
            }
            yield return new WaitForSeconds(0.5f);
            isStartCoroutine = false;
            
        }
        private void AddForce(Rigidbody rb)
        {
            rb.AddForceAtPosition(-rb.transform.up, rb.transform.position, ForceMode.Impulse);
        }

        

        
    }
}
