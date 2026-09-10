using UnityEngine;

namespace Script.Tools.Interaction.Drag
{
    public class MouseWorldPosition
    {
        public Vector3 GetMouseWorldPosition(Camera playerCamera, float objDepth, float halfHigh, int obstacleLayer)
        {
            Vector3 mouseScreenPos = Input.mousePosition;
            mouseScreenPos.z = objDepth;
            
            Vector3 mouseWorldPos = playerCamera.ScreenToWorldPoint(mouseScreenPos);
            
            RaycastHit hit;
            float maxDistance = 5f;
            Vector3 originPosition = mouseWorldPos + Vector3.up * 2;
            if (Physics.Raycast(originPosition, Vector3.down, out hit, maxDistance, obstacleLayer))
            {
                float minY = hit.point.y + halfHigh;
                mouseWorldPos.y =  Mathf.Max(mouseWorldPos.y, minY);
            }
            
            return mouseWorldPos;
        }
    }
}