using Script.Tools.Managers;
using UnityEngine;

namespace Script.Tools.Buttons
{
    public class ChangeScene : MonoBehaviour
    {
        public void EnterScene(string levelName)
        {
            GameManager.Instance.LoadScene(levelName);
        }
    }
}
