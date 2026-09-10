using UnityEngine;

namespace Script.StartMenu
{
    public class QuitGameButton : MonoBehaviour
    {
        public void OnClick()
        {
            QuitGame();
        }

        private void QuitGame()
        {
            //Debug.Log("退出游戏");
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif 
            
        }
    }
}
