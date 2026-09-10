using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Script.Tools.Managers
{
    public class DialogueManager : MonoBehaviour
    {
        private DialogueManager(){}
        public static DialogueManager Instance{get; private set;}
        
        [Serializable]
        public class DialogueData//对话数据
        {
            public string speakerName;
            public List<string> contexts;
            internal int currentContextIndex;
        }
        [Serializable]
        public class DialogueIDData//对话ID数据
        {
            public string dialogueDataID;
            [FormerlySerializedAs("dialogueDatas")] public List<DialogueData> dialogueData;
        }
        
        [FormerlySerializedAs("dialogueIDDatas")] [SerializeField] List<DialogueIDData> dialogueIDData;
        private Dictionary<string, List<DialogueData>> dialogues;
        
        [SerializeField] private Canvas canvas;
        [SerializeField] private GameObject dialoguePanel;//对话框
        [SerializeField] private TextMeshProUGUI speakerNameComponent;//显示说话者名字的组件
        [SerializeField] private TextMeshProUGUI dialogueContentTextComponent;//显示对话内容的组件
        public  bool isTyping;//记录是否正在打字
        private bool cancelTyping;//标记打字是否被打断
        private bool isPassTyping;//标记是否跳过打字
        private bool isEnterNext;//标记是否进入下一段话
        private float typeInterval = 0.2f;//打字速度
        private IEnumerator typingCoroutine;
        private bool isStart;
        private void Start()
        {
            KeepSingleInstance();
            dialogues = new Dictionary<string, List<DialogueData>>();
            int count = dialogueIDData.Count;
            for (int i = 0; i < count; i++)
            {
                dialogues.Add(dialogueIDData[i].dialogueDataID, dialogueIDData[i].dialogueData);
            }
        }
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    cancelTyping = true;
                }
            }
        }
        private void Initialize(Scene scene, LoadSceneMode mode)
        {
            canvas = GameObject.FindGameObjectWithTag("Canvas")?.GetComponent<Canvas>();
            dialoguePanel = canvas?.transform.Find("DialogueBox")?.gameObject;
            dialoguePanel?.SetActive(false);
            speakerNameComponent = dialoguePanel?.transform.Find("speakerName")?.GetComponent<TextMeshProUGUI>();
            dialogueContentTextComponent = dialoguePanel?.transform.Find("dialogueContentText")?.GetComponent<TextMeshProUGUI>();
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
        public IEnumerator StartDialogue(string dialogueID)
        {
            foreach (string key in dialogues.Keys)
            {
                if (key == dialogueID)
                {
                    yield return StartCoroutine(RubStartDialogue(dialogues[dialogueID]));
                }
            }
        }
        public IEnumerator StartDialogue(string dialogueID, Dictionary<string, List<DialogueData>> dialogues)
        {
            foreach (string key in dialogues.Keys)
            {
                if (key == dialogueID)
                {
                    yield return StartCoroutine(RubStartDialogue(dialogues[dialogueID]));
                }
            }
        }
        private IEnumerator RubStartDialogue(List<DialogueData> dialogueDatas)
        {
            int allSpeakers = dialogueDatas.Count;//对话总人数
            string[] names = new string[allSpeakers];
            for (int i = 0; i < allSpeakers; i++)
            {
                names[i] = dialogueDatas[i].speakerName;
            }
            int currentSpeakerNameIndex = 0;//当前对话者的索引
            int contextAllCounts = 0 ;//对话总数
            for (int i = 0; i < allSpeakers; i++)
            {
                contextAllCounts += dialogueDatas[i].contexts.Count;
                dialogueDatas[i].currentContextIndex = 0;
            }
            
            for (int i = 0; i < contextAllCounts; i++)
            {
                string currentSpeakerName = names[currentSpeakerNameIndex];
                DialogueData currentSpeakerData = dialogueDatas[currentSpeakerNameIndex];

                if (currentSpeakerData.currentContextIndex < currentSpeakerData.contexts.Count)
                {
                    yield return StartDialogue(currentSpeakerData, currentSpeakerName);
                }
                currentSpeakerNameIndex = (currentSpeakerNameIndex + 1) % allSpeakers;
                currentSpeakerData = dialogueDatas[currentSpeakerNameIndex];
                if (currentSpeakerData.currentContextIndex < currentSpeakerData.contexts.Count)
                {
                    if (currentSpeakerData.contexts[currentSpeakerData.currentContextIndex] == "")
                    {
                        currentSpeakerData.currentContextIndex++;
                        currentSpeakerNameIndex = (currentSpeakerNameIndex + 1) % allSpeakers;//返回上一个说话者的索引
                    }
                }
            }
            
            EndDialogue();
        }
        private IEnumerator StartDialogue(DialogueData currentDialogueData, string currentSpeakerName)//开始对话
        {
            speakerNameComponent.text = currentSpeakerName;
            dialoguePanel.SetActive(true);
            yield return DisplayLine(currentDialogueData);
        }
        private IEnumerator DisplayLine(DialogueData currentDialogueData)//显示当前对话内容
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }
            
            int index = currentDialogueData.currentContextIndex;//获取当前对话内容的索引
            if (index < currentDialogueData.contexts.Count)
            {
                yield return typingCoroutine = TypeText(currentDialogueData.contexts[index]);
                currentDialogueData.currentContextIndex++;//加载当前说话者的下一句话索引
            }
        }
        IEnumerator TypeText(string text)
        {
            isTyping = true;
            cancelTyping = false;
            dialogueContentTextComponent.text = "";
            for (int i = 0; i < text.Length; i++)
            {
                if (cancelTyping)
                {
                    dialogueContentTextComponent.text = text; //打字被中断时直接显示全部文字
                    break;
                }

                if (!dialogueContentTextComponent.IsDestroyed())
                {
                    dialogueContentTextComponent.text += text[i];
                }

                yield return new WaitForSeconds(typeInterval);
            }

            typingCoroutine = null;
            isTyping = false;
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        }

        private void EndTyping(Scene scene, LoadSceneMode mode)
        {
            isTyping = false;
            StopAllCoroutines();
        }
        private void EndDialogue()//结束对话
        {
            dialoguePanel.SetActive(false);
            EventsManager.TriggerOnEndTyping();
        }
        private void OnEnable()
        {
            SceneManager.sceneLoaded += EndTyping;
            SceneManager.sceneLoaded += Initialize;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= EndTyping;
            SceneManager.sceneLoaded -= Initialize;
        }
    }
}
