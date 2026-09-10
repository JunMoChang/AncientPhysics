using UnityEngine;

namespace Script.Pulley
{
    public class LoadCalculation : MonoBehaviour
    {
        public float Load{get; private set;}
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Weight"))
            {
                collision.gameObject.TryGetComponent(out Rigidbody rb);
                Load += rb?.mass ?? 0;
            }
        }

        void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.CompareTag("Weight"))
            {
                collision.gameObject.TryGetComponent(out Rigidbody rb);
                Load -= rb?.mass ?? 0;
            }
        }
    }
}
