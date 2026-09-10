using Obi;
using Script.Tools.Managers;
using UnityEngine;

namespace Script.Pulley
{
    public class RopeAttachmentType : MonoBehaviour
    {
        [SerializeField]
        private ObiRope rope;
        [SerializeField] 
        private Transform attachmentObject;
        private ObiParticleAttachment[] attachments;
        private ObiParticleAttachment targetAttachment;
        private void Start()
        {
            attachments = rope.GetComponents<ObiParticleAttachment>();
            if (attachments.Length == 0)
            {
                targetAttachment = null;
            }
            else
            {
                foreach (ObiParticleAttachment attachment in attachments)
                {
                    if(attachment.target == attachmentObject)
                    {
                        targetAttachment = attachment;
                    }
                }
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (targetAttachment == null) return;
            if (other.gameObject.CompareTag("FreePlatform"))
            {
                targetAttachment.attachmentType = ObiParticleAttachment.AttachmentType.Static;
            }
        }
        
        private void ChangeAttachment(GameObject obj)
        {
            if (obj == attachmentObject.gameObject)
            {
                targetAttachment.attachmentType = ObiParticleAttachment.AttachmentType.Dynamic;
            }
        }
        private void OnEnable()
        {
            EventsManager.OnRayTest += ChangeAttachment;
        }

        void OnDisable()
        {
            EventsManager.OnRayTest -= ChangeAttachment;
        }

        void OnDestroy()
        {
            EventsManager.OnRayTest -= ChangeAttachment;
        }
    }
}
