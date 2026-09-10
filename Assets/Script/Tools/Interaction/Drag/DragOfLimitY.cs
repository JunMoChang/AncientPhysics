using Script.Tools.Interface;
using UnityEngine;

namespace Script.Tools.Interaction.Drag
{
    public class DragOfLimitY : IDragInteraction
    {
        Transform transform;
        public DragOfLimitY(Transform trans)
        {
            transform = trans;
        }
        public void Drag(Vector3 offset, Vector3 mousePosition)
        {
            transform.TryGetComponent(out Rigidbody rb);
            
            rb.MovePosition(new Vector3(mousePosition.x + offset.x, transform.position.y, mousePosition.z + offset.z));
        }
    }
}