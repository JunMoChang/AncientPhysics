using Script.Tools.Interface;
using UnityEngine;

namespace Script.Tools.Interaction.Drag
{
    public class DragOfLimitXY : IDragInteraction
    {
        Transform transform;
        public DragOfLimitXY(Transform trans)
        {
            transform = trans;
        }
        public void Drag(Vector3 offset, Vector3 mousePosition)
        {
            transform.TryGetComponent(out Rigidbody rb);
            Vector3 mouseWorldPos = mousePosition;
        
            if(rb != null)
            {
                rb.MovePosition(new Vector3(transform.position.x, transform.position.y, mouseWorldPos.z));
            }
            else
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, mouseWorldPos.z);
            }
        }
    }
}