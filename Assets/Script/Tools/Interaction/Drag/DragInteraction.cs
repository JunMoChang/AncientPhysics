using Script.Tools.Interface;
using UnityEngine;
using Zenject;

namespace Script.Tools.Interaction.Drag
{
    public class DragInteraction : MonoBehaviour
    {
        [Inject]private MouseWorldPosition mouseWorldPosition;
        [Inject]private DragInitialSetting dragInitialSetting;
        [Inject]private IDragFactory dragFactory;
        private Vector3 offset;//物体与地面的偏移量
        private float halfHigh;//物体半高
        private int obstacleLayer;
        private IDragInteraction dragInteraction;
    
        private Camera playerCamera;
        private Rigidbody rb;
        private Collider coll;
        private float objDepth;//深度
        private float detectionDistance = 5.2f;
        private bool canDrag;
        
        void Start()
        {
            playerCamera = Camera.main;
            TryGetComponent(out rb);
            TryGetComponent(out coll);
        
            halfHigh = coll ? coll.bounds.extents.y : 0;
            obstacleLayer = LayerMask.GetMask("Ground") | LayerMask.GetMask("Stuff");
        }
        private void OnMouseDown()
        {
            dragInteraction = dragFactory?.CreateDrag(transform, obstacleLayer);
            
            dragInitialSetting.Initialize(playerCamera, transform, detectionDistance, ref objDepth, ref offset, ref canDrag, obstacleLayer);
        } 
        private void OnMouseDrag()
        {
            if(dragInteraction == null || !canDrag)return;
            
            ResetRotation();
            dragInteraction.Drag(offset, mouseWorldPosition.GetMouseWorldPosition(playerCamera, objDepth, halfHigh, obstacleLayer));
        }
        private void OnMouseUp()
        {
            canDrag = false;
            
            if(rb == null)return;
            rb.useGravity = true;
            
        }
        private void ResetRotation()
        {
            transform.rotation = Quaternion.Euler(0,0,0);
        }
    }
}
