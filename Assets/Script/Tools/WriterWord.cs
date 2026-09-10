using System.Collections;
using TMPro;
using UnityEngine;

namespace Script.Tools
{
    public class WriterWord : MonoBehaviour
    {
        [SerializeField]
        private string targetText;

        private float interval = 0.13f;
        public float Interval => interval;

        private TextMeshProUGUI textComponent;
        
        void Awake()
        {
            TryGetComponent(out textComponent);
        }
    
        IEnumerator WriteText()
        {
            if (!textComponent)
            {
                #if UNITY_EDITOR
                    Debug.Log("textComponent is null");
                #endif
                yield break;
            }

            textComponent.text = "";
            for (int i = 0; i < targetText.Length; i++)
            {
                textComponent.text += targetText[i];
                yield return new WaitForSeconds(interval);
            }
        }

        void OnEnable()
        {
            StartCoroutine(WriteText());
        }
        public string GetTargetText()
        {
            return targetText;
        }
    }
}
