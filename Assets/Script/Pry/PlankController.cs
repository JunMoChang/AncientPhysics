using TMPro;
using UnityEngine;

namespace Script.Pry
{
    public class PlankController : MonoBehaviour
    {
        private HingeJoint hinge;
        private Transform fulcrum;
        
        public Light Light{get; private set;}
        
        void Start()
        {
            hinge = GetComponent<HingeJoint>();
            hinge.anchor = transform.GetChild(0).localPosition;
            fulcrum = hinge.connectedBody.transform;
            hinge.connectedAnchor = fulcrum.GetChild(0).localPosition;
            Light = transform.GetChild(1)?.GetComponent<Light>();
            if (Light != null)
            {
                Light.gameObject.transform.localPosition = new Vector3(hinge.anchor.x, 10.5f, hinge.anchor.z);
            }
        }
        
        
    }
}
