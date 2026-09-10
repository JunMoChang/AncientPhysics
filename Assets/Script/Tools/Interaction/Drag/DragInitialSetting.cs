using UnityEngine;
using Zenject;

namespace Script.Tools.Interaction.Drag
{
    public class DragInitialSetting
    {
        [Inject]MouseWorldPosition mouseWorldPosition;
        public void Initialize(Camera playerCamera, Transform transform, float detectionDistance, ref float objDepth, ref Vector3 offset,ref bool canDrag, int groundMask)
        {
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, detectionDistance))
            {
                transform.TryGetComponent(out Collider col);
                if (hit.collider == col)
                {                
                    float halfHigh = col.bounds.extents.y;
                    objDepth = playerCamera.WorldToScreenPoint(transform.position).z;
                    Vector3 mouseWorldPos = mouseWorldPosition.GetMouseWorldPosition(playerCamera, objDepth, halfHigh, groundMask);
                
                    //如果该物体是能跟随玩家移动的，限制物体与玩家之间的距离
                    /*if (!hit.transform.gameObject.CompareTag("NoFollowPlayer") && hit.distance > maxDistance)
                    {
                        #if UNITY_EDITOR
                            Debug.Log(hit.distance);
                        #endif
                        transform.position = new Vector3(player.position.x, player.position.y - playerHalfHeight/2, player.position.z - playerHalfWide / 2);
                        mouseWorldPos = transform.position;
                        objDepth = playerCamera.WorldToScreenPoint(transform.position).z;
                    }*/

                    offset = transform.position - mouseWorldPos;
                    canDrag = true;
                }
                else
                {
                    canDrag = false;
                }
            }
            
        }
    }
}