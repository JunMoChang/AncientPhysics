using System.Collections;
using System.Collections.Generic;
using Script.Tools.Managers;
using UnityEngine;

namespace Script.Tools.DialogueTriggerWays.BaseClass
{
    public abstract class BaseDialogueTrigger : MonoBehaviour
    {
        [Header("对话设置")]
        public string dialogueID;
        public float triggerDelay;//触发延迟时间
        public bool requireKeyPress;
        public KeyCode triggerKey = KeyCode.F;//触发对话的方式
        public bool canRetrigger = true;//标记是否可以触发对话
        protected bool hasTriggered;//标记是否触发了对话
        private Dictionary<string, DialogueManager.DialogueData> dialogueDatas;
        void Start()
        {
            Initialize();
        }
    
        protected virtual void Initialize() {}
    
        protected void TriggerDialogue()
        {
            if (!canRetrigger && hasTriggered) return;
            if (DialogueManager.Instance.isTyping) return;
            
            StartCoroutine(TriggerWithDelay());
            hasTriggered = true;
        }
    
        IEnumerator TriggerWithDelay()
        {
            yield return new WaitForSeconds(triggerDelay);
            yield return DialogueManager.Instance.StartDialogue(dialogueID);
        }
    }
}