using Script.Tools.Managers;
using UnityEngine;

namespace Script.Tools.Buttons
{
    public class BackButton : MonoBehaviour
    {
        public void GoBackUI()
        {
            UIManager.Instance.GoBackUI();
        }

        public void OpenUI(GameObject newUI)
        {
            UIManager.Instance.OpenUI(newUI);
        }

        public void BackToStartMenu()
        {
            GameManager.Instance.LoadScene0();
            Time.timeScale = 1;
        }
    }
}
