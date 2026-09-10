using System.Collections;
using Script.Tools.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script.PLayer
{
    public class RayInteraction : MonoBehaviour
    {
        private Ray ray;
        private int mask;
        private Camera playerCamera;
        private float detectionDistance = 3f;
        private float detectionInterval = 0.5f;
        
        private RaycastHit hit;
        private bool isRayTesting;//是否正在检测，避免多次开启协程
        void Start()
        {
            mask = LayerMask.GetMask("Interaction");
        }

        void Update()
        {
            if (!isRayTesting && Input.GetMouseButtonDown(0))
            {
                StartCoroutine(RayTest());
            }
        }
        
        private void GetMainCamera(Scene scene, LoadSceneMode mode)
        {
            playerCamera = Camera.main;
        }
        //检测点击的物体
        IEnumerator RayTest()
        {
            isRayTesting = true;
            ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, detectionDistance,mask))
            {
                EventsManager.TriggerOnClick(hit.collider.gameObject);//触发物体的点击事件   
            }
            yield return new WaitForSeconds(detectionInterval);
            isRayTesting = false;
        }

        void OnEnable()
        {
            SceneManager.sceneLoaded += GetMainCamera;
            //GetMainCamera(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        }
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= GetMainCamera;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= GetMainCamera;
        }
        
    }
}