using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script.Tools.Managers
{
    public class UIManager : MonoBehaviour
    {
        private Stack<GameObject> uiStack = new();
        private Canvas canvas;
        private GameObject defaultPanel;
        private GameObject completeHint;
        private GameObject completePanel;
        private GameObject pausePanel;
        private GameObject dialogueBoxPanel;
        private bool isPaused;
        private bool isStartCoroutine;
        private float waitTime;
        public static UIManager Instance{get; private set;}
        private GameManager gameManager;
        void Awake()
        {
            KeepSingleInstance();
        }
        void Start()
        {
            defaultPanel = GameObject.FindGameObjectWithTag("DefaultPanel");
            uiStack.Push(defaultPanel);
            gameManager = GameManager.Instance;
        }
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && gameManager.SceneIndex != 0 && gameManager.SceneIndex != 1)
            {
                PauseGame();
            }
        }
        private void KeepSingleInstance()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(this);
            }
        }
        private void PauseGame()
        {
            isPaused = !isPaused;
            Time.timeScale = isPaused ? 0 : 1;
            pausePanel?.SetActive(isPaused);
        }
        #region back_last_UI
        public void OpenUI(GameObject newUI)
        {
            if (uiStack.Count > 0)
            {
                uiStack.Peek().SetActive(false);
            }
            uiStack.Push(newUI);
            newUI.SetActive(true);
        }
        public void GoBackUI()
        {
            if (uiStack.Count <= 1)
            {
                return;
            }
            
            GameObject currentUI = uiStack.Pop();
            currentUI.SetActive(false);
            
            GameObject previousUI = uiStack.Peek();
            previousUI.SetActive(true);
        }
        #endregion

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            StopCoroutine();
            uiStack.Clear();
            defaultPanel = GameObject.FindGameObjectWithTag("DefaultPanel");
            if(defaultPanel != null)
            {
                uiStack.Push(defaultPanel);
            }
            canvas = GameObject.FindGameObjectWithTag("Canvas")?.GetComponent<Canvas>();
            pausePanel = canvas? canvas.transform.Find("PausePanel")?.gameObject : null;
            completeHint = canvas? canvas.transform.Find("completeHint")?.gameObject : null;
            completePanel = canvas ? canvas.transform.Find("completePanel")?.gameObject : null;
            dialogueBoxPanel = canvas? canvas.transform.Find("DialogueBox")?.gameObject : null;
            if (completeHint != null && completeHint.TryGetComponent(out WriterWord word))
            {
                waitTime = word.GetTargetText().Length * word.Interval;
            }
        }
        
        private void LevelCompleteUI()
        {
            
            if(!isStartCoroutine)
            {
                EventsManager.OnEndTyping += CompletePanelActive;
                StartCoroutine(LevelComplete());
            }
            EventsManager.TriggerOnDialogueManager();
        }

        IEnumerator LevelComplete()
        {
            isStartCoroutine = true;
            if (completeHint != null)
            {
                completeHint.SetActive(true);
                yield return new WaitForSeconds(waitTime + 3);
                completeHint.TryGetComponent(out TextMeshProUGUI textComponent);
                textComponent.text = "";
                completeHint.SetActive(false);
            }
            
            if (!dialogueBoxPanel.activeSelf)
            {
                completePanel.SetActive(true);
            }
            
            isStartCoroutine = false;
            
        }
       
        private void StopCoroutine()
        {
            StopCoroutine(LevelComplete());
            isStartCoroutine = false;
            if (completeHint != null)
            {
                completeHint.TryGetComponent(out TextMeshProUGUI textComponent);
                textComponent.text = "";
                completeHint.SetActive(false);
            }
            
            if (completePanel != null)
            {
                completePanel?.SetActive(false);
            }
        }
        private void CompletePanelActive()
        {
            completePanel?.SetActive(true);
            EventsManager.OnEndTyping -= CompletePanelActive;
        }
        void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            EventsManager.OnLevelCompleted += LevelCompleteUI;
            
        }
        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            EventsManager.OnLevelCompleted -= LevelCompleteUI;
            EventsManager.OnEndTyping -= CompletePanelActive;
        }
    }
}
