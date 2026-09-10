using System.Collections;
using Script.Tools.Interface;
using Script.Tools.Managers;
using UnityEngine;

namespace Script.Pulley
{
    public class PlatformController : MonoBehaviour, IRayDetection
    {
        private int groundLayer;
        private float maxDetectionDistance;
        private Vector3 currentPosition;
        private bool isStartCoroutine;
        
        void Start()
        {
            groundLayer = LayerMask.GetMask("Ground");
            maxDetectionDistance = 0.06f;
            currentPosition = transform.position;
        }

        void Update()
        {
            if (currentPosition != transform.position && !isStartCoroutine)
            {
                currentPosition = transform.position;
                StartCoroutine(GroundTest());
            }
        }
        IEnumerator GroundTest()
        {
            isStartCoroutine = true;
            Raycast(maxDetectionDistance,groundLayer);
            yield return new WaitForSeconds(0.5f);
            isStartCoroutine = false;
        }
        public void Raycast(float maxDistance, int layer)
        {
            if (Physics.Raycast(transform.position, Vector3.down, maxDistance, layer))
            {
                EventsManager.TriggerOnRayTest(gameObject);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, Vector3.down * maxDetectionDistance);
        }
    }
}
