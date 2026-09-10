using Script.Tools.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script.StartMenu
{
    public class StartGameButton : MonoBehaviour
    {
        public void OnClick()
        {
            StartGame();
        }
        private void StartGame()
        {
            //Debug.Log("开始游戏");
            GameManager.Instance.LoadScene1();
        }
    }
}
