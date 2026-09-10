using Script.Tools.Managers;
using UnityEngine;

namespace Script.KeyholeImage
{
    public class LightPlatform : MonoBehaviour
    {
        private Vector3 centerPosition;
        
        private int lightLayer;
        void Start()
        {
            lightLayer = LayerMask.NameToLayer("Light");
        }
        
        
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == lightLayer)
            {
                centerPosition = TryGetComponent(out Collider coll) ? coll.bounds.center : transform.position;
                Physics.SyncTransforms();
                collision.transform.position = centerPosition;
                collision.transform.SetParent(transform,true);
                Physics.SyncTransforms();
                GameManager.Instance.LeveCompleted();
            }
        }
        
        
        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.layer == lightLayer)
            {
                collision.transform.SetParent(null);
            }
        }
        
    }
}
