using Script.Tools.Managers;
using UnityEngine;

namespace Script.Pry
{
    public class PryController : MonoBehaviour
    {
        private int totalPries = 3;
        private int currentStablePries;
        private GameObject[] stablePries;
        void Start()
        {
            stablePries = new GameObject[totalPries];
            EventsManager.OnPryStable += Handledstable;
            EventsManager.OnPryUnstable += HandledUnstable;
            Debug.Log(currentStablePries);
        }

        private void Handledstable(GameObject obj)
        {
            currentStablePries++;
            obj.GetComponent<PlankController>().Light.enabled = true;
            Debug.Log("平衡事件:" + currentStablePries);
            stablePries[currentStablePries-1] = obj;
            CheckAllPriesBalance();
        }

        private void HandledUnstable(GameObject obj)
        {
            foreach (GameObject pry in stablePries)
            {
                if (pry == obj)
                {
                    currentStablePries--;
                    obj.GetComponent<PlankController>().Light.enabled = false;
                    Debug.Log("不平衡事件:"+currentStablePries);
                }
            }
        }
        private void CheckAllPriesBalance()
        {
            if (currentStablePries >= totalPries)
            {
                GameManager.Instance.LeveCompleted();
            }
        }

        private void OnDestroy()
        {
            EventsManager.OnPryStable -= Handledstable;
            EventsManager.OnPryUnstable -= HandledUnstable;
        }
    }
}
