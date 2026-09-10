using System.Collections;
using UnityEngine;

namespace Script.NPC
{
    public class NpcController : MonoBehaviour
    {
        public TMPro.TextMeshProUGUI dialogueText; // 对话框UI
        public GameObject dialoguePanel; 
        private NpcDialogueManager dialogueManager;

        void Start()
        {
            dialogueManager = GetComponent<NpcDialogueManager>();
        }

        // 玩家靠近NPC时触发
        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                dialoguePanel.SetActive(true);
                //StartCoroutine(StartDialogue("你好旅行者"));
            }
        }

        IEnumerator StartDialogue(string firstMessage)
        {
            dialogueText.text = "神秘人：思考中...";
            yield return dialogueManager.SendChatRequest(firstMessage, (reply) => {
                dialogueText.text = "骑士：" + reply;
            });
        }

        // 玩家输入UI发送按钮调用
        public void OnPlayerInputSent(string inputText)
        {
            StartCoroutine(dialogueManager.SendChatRequest(inputText, (reply) => {
                dialogueText.text = "神秘人：" + reply;
            }));
        }
    }
}