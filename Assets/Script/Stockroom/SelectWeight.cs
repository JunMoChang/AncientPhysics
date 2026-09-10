using Script.Tools.Managers;
using UnityEngine;

namespace Script.Stockroom
{
    public class SelectWeight : MonoBehaviour
    {
        private int totalWeights = 7;
        private int currentWeight;

     
        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Weight"))
            {
                currentWeight++;
                //Debug.Log(currentWeight);
                CheckCompleteLevel();
            }
        }
        
        void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Weight"))
            {
                currentWeight--;
                //Debug.Log(currentWeight);
            }
        }

        private void CheckCompleteLevel()
        {
            if (currentWeight >= totalWeights)
            {
                //Debug.Log(currentWeight);
                CompleteLevel();
            }
        }
        private void CompleteLevel()
        {
            GameManager.Instance.LeveCompleted();
        }
    }
}
