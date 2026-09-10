using System;
using Cinemachine;
using Script.Tools.Interface;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script.PLayer
{
    public class PlayerController : MonoBehaviour
    {   
        public static PlayerController Instance{get; private set;}
        
        private CharacterController characterController;
        [SerializeField]
        private Transform groundCheck;
        private IState currentState;
        private float gravity = -9.81f;
        private Vector3 gravityVelocity = Vector3.zero;
        private float speed = 3.0f;
        private float accelerationRate= 3.6f;
        private float sensitivityX = 2f;
        private float sensitivityY = 2f;
        
        private CinemachineVirtualCamera playerCamera;
        private CinemachineVirtualCamera focusCamera;
        private float rotationX;

        private float zoomSpeed = 20f;//视野缩放速率
        private Collider playerCollider;
        
        private float detectGroundRadius = 0.1f;
        private float detectionDistance = 2f;
        private float offsetDistance = 2f;
        private LayerMask wallMask;
        private LayerMask stuffMask;
        private LayerMask groundMask;
        
        private Action moveDelegate;
        
        void Start()
        {
            characterController = GetComponent<CharacterController>();
            playerCamera = transform.GetChild(0).GetComponent<CinemachineVirtualCamera>();
            playerCollider = transform.GetComponent<Collider>();
            Cursor.lockState = CursorLockMode.Confined;
            wallMask = LayerMask.GetMask("Wall");
            stuffMask = LayerMask.GetMask("Stuff");
            groundMask = LayerMask.GetMask("Ground");
        }
       
        void Update()
        {
            if(moveDelegate != null)
            {
                moveDelegate();
            }
            if(Input.GetKey(KeyCode.Mouse1))
            {
                Rotate();
            }
            MouseReveal();
            SettingFiledOfView();
        }
        
        //人物移动
        private void Move()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            Vector3 movement = transform.right * horizontal + transform.forward * vertical;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                characterController.Move(speed *accelerationRate * Time.deltaTime * movement.normalized);
            }
            else
            {
                characterController.Move(speed * Time.deltaTime * movement.normalized);
            }
            
            ApplyGravity();
        }
        private void CantMove(){}
        private void ApplyGravity()
        {
            bool isGrounded = Physics.CheckSphere(groundCheck.position, detectGroundRadius, groundMask|stuffMask);
            
            if (isGrounded && gravityVelocity.y < 0)
            {
                gravityVelocity.y = 0f;
                return;
            }
            gravityVelocity.y += gravity * Time.deltaTime * Time.deltaTime;
            characterController.Move(gravityVelocity);
        }
        
        //视角旋转
        private void Rotate()
        {
            float mouseX = Input.GetAxis("Mouse X") * sensitivityX;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivityY;
            
            rotationX -= mouseY;
            rotationX = Mathf.Clamp(rotationX, -90, 74);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.Rotate(Vector3.up * mouseX);
        }
        
        //鼠标显隐
        private void MouseReveal()
        {
            Cursor.lockState = Input.GetKey(KeyCode.Mouse1) ? CursorLockMode.Locked : CursorLockMode.Confined;
        }
        
        //切换视野
        private void SettingFiledOfView()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            scroll = scroll > 0 ? scroll/scroll : scroll < 0 ? scroll/-scroll : 0 ;
            
            float targetFov = playerCamera.m_Lens.FieldOfView - scroll * zoomSpeed;
            targetFov = Mathf.Clamp(targetFov, 60, 75);
            playerCamera.m_Lens.FieldOfView = Mathf.Lerp(playerCamera.m_Lens.FieldOfView, targetFov,0.2f);
        }
        
        //检测障碍物
        private void DetectionObstacle(Vector3 direction)
        {
            RaycastHit hit;
            if (Physics.Raycast(playerCollider.bounds.center, direction,out hit,detectionDistance,wallMask|stuffMask))
            {
                Vector3 offsetDirection = -direction;
                Vector3 newPosition = hit.point + offsetDirection * offsetDistance;
                newPosition.y = transform.position.y;
                transform.position = newPosition;
            }
        }
        
        private void SceneLoad(Scene scene, LoadSceneMode mode)
        {
            SingleInstance();
            OriginalPosition(scene, mode);
        }
        //单一玩家
        private void SingleInstance()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                if (Instance != this)
                {
                    Destroy(gameObject);
                }
            }
#if UNITY_EDITOR
            Debug.Log(Instance.name);
#endif
        }
        //角色初始位置
        private void OriginalPosition(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != "LevelsSelection" && scene.name != "StartMenu")
            {
                //gameObject.SetActive(false);
                GameObject obj = GameObject.FindWithTag("OriginalPosition");
                gravityVelocity = Vector3.zero;
                if (obj != null)
                {
                    transform.position =  obj.transform.position ;
                }
                else
                {
                    transform.position = Vector3.zero;
#if UNITY_EDITOR
                Debug.Log("The GameObject is named OriginalPosition not found");    
#endif
                }
                transform.rotation = Quaternion.Euler(0, 0, 0);
                moveDelegate = Move;
                //gameObject.SetActive(true);
            }
            else
            {
                moveDelegate = CantMove;
            }
        }
        //用于注视物体的摄像机
        private void CreateFocusCamera(Scene scene, LoadSceneMode mode)
        {
            focusCamera = GameObject.FindWithTag("FocusCamera")?.GetComponent<CinemachineVirtualCamera>();
            if(focusCamera == null)
            {
                GameObject focusCameraObj = new GameObject("FocusCamera");
                focusCameraObj.tag = "FocusCamera";
                CinemachineVirtualCamera focusCam = focusCameraObj.AddComponent<CinemachineVirtualCamera>(); 
                focusCam.Priority = 0;
                focusCam.m_Lens.FieldOfView = 73;
                focusCamera = focusCam;   
            }
        }
        private void OnEnable()
        {
            SceneManager.sceneLoaded += CreateFocusCamera;
            SceneManager.sceneLoaded += SceneLoad;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= CreateFocusCamera;
            SceneManager.sceneLoaded -= SceneLoad;
        }
        void OnDestroy()
        {
            SceneManager.sceneLoaded -= SceneLoad;
            SceneManager.sceneLoaded -= CreateFocusCamera;
        }
        
    }
}
