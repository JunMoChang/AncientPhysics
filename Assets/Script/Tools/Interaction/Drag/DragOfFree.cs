using Script.Tools.Interface;
using UnityEngine;

namespace Script.Tools.Interaction.Drag
{
    public class DragOfFree : IDragInteraction
    {
        private Transform transform;
        private int obstacleLayer;
        private float halfHigh;
        public DragOfFree(Transform trans, int obstacleLayer)
        {
            transform = trans;
            this.obstacleLayer = obstacleLayer;
            halfHigh = transform.TryGetComponent(out Collider coll) ? coll.bounds.extents.y : 0;
        }
        public void Drag(Vector3 offset, Vector3 mouseWorldPos)
        {
            transform.TryGetComponent(out Rigidbody rb);
            if(rb == null)return;
            
            rb.angularVelocity = Vector3.zero;
            rb.useGravity = false;
            rb.velocity = Vector3.zero;

            //RayTestOfGround();
            
            rb.MovePosition(mouseWorldPos + offset);
        }
        /*private void RayTestOfGround()
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, halfHigh, obstacleLayer))
            {
                float minY = hit.point.y + halfHigh;
                transform.position = new Vector3(transform.position.x, minY, transform.position.z);
            }
        }*/
    }
}