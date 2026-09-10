using System.Collections;
using Script.Tools.DialogueTriggerWays.BaseClass;
using UnityEngine;

namespace Script.Tools.DialogueTriggerWays
{
    public class TimedDialogueTrigger : BaseDialogueTrigger
    {
        public float startDelay = 5f;//触发对话延迟时间
    
        protected override void Initialize()
        {
            StartCoroutine(StartTimedTrigger());
        }
    
        IEnumerator StartTimedTrigger()
        {
            yield return new WaitForSeconds(startDelay);
            TriggerDialogue();
        }
    }
}