using TMPro;
using UnityEngine;

namespace Script.NPC
{
    public class Send : MonoBehaviour
    {

        public TextMeshProUGUI inputField;

        public NpcController npcController;
        // 在发送按钮的OnClick事件中
        public void SendMessageToNPC()
        {
            string text = inputField.text;
            npcController.OnPlayerInputSent(text);
            inputField.text = "";
        }
    }
}