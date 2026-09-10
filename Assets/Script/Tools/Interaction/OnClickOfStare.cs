using Cinemachine;
using Script.Tools.Managers;
using UnityEngine;

namespace Script.Tools.Interaction
{
    public class OnClickOfStare : MonoBehaviour
    {
        private CinemachineVirtualCamera playerCamera;
        private CinemachineVirtualCamera focusCamera;
        private float focusDistance = 0.5f;
        void Start()
        {
            playerCamera = GameObject.FindWithTag("PlayerCamera").GetComponent<CinemachineVirtualCamera>();
            focusCamera = GameObject.FindWithTag("FocusCamera").GetComponent<CinemachineVirtualCamera>();
            EventsManager.OnClick += StareTarget;
        }

        void Update()
        {
            EndClickOfStare();
        }
        private void StareTarget(GameObject target)
        {
            if (target != gameObject) return;
            target.TryGetComponent(out Collider focusCollider);
            GameObject focusObject = new GameObject();
            focusObject.transform.position = focusCollider.bounds.center;
            
            Vector3 cameraPos = focusObject.transform.position + Vector3.up * focusDistance;
            Quaternion cameraRot = Quaternion.LookRotation(Vector3.down - transform.forward * 0.01f); 
            focusCamera.transform.position = cameraPos;
            focusCamera.transform.rotation = cameraRot;
            
            playerCamera.Priority = 0;
            focusCamera.Priority = 10;
        }

        private void EndClickOfStare()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                playerCamera.Priority = 10;
                focusCamera.Priority = 0;
                focusCamera.LookAt = null;
            }
        }
        private void OnDestroy()
        {
            EventsManager.OnClick -= StareTarget;
        }
    }
}
