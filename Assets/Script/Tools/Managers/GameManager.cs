using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script.Tools.Managers
{
    public class GameManager : MonoBehaviour
    {
        private GameObject player;
        public static GameManager Instance{get; private set;}
        
        public int SceneIndex{get; private set;}
        private void Awake()
        {
            KeepSingleInstance();
        }
        private void KeepSingleInstance()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        private void GetCurrentSceneIndex(Scene scene, LoadSceneMode mode)
        {
            SceneIndex = scene.buildIndex;
        }
        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
        public void LoadScene0()
        {
            SceneManager.LoadScene(0);
        }
        public void LoadScene1()
        {
            SceneManager.LoadScene(1);
        }
     
        public void LeveCompleted()
        {
            EventsManager.TriggerLevelCompleted();
        }

        void OnEnable()
        {
            SceneManager.sceneLoaded += GetCurrentSceneIndex;
        }
        void OnDisable()
        {
            SceneManager.sceneLoaded -= GetCurrentSceneIndex;
        }
    }
}