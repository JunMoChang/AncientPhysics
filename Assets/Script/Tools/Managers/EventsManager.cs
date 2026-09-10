using System;
using UnityEngine;

namespace Script.Tools.Managers
{
    public class EventsManager
    {
        private EventsManager(){}
        
        public static event Action OnLevelCompleted;//完成关卡 
        public static event Action<GameObject> OnPryStable;//杠杠平衡
        public static event Action<GameObject> OnPryUnstable;//杠杆非平衡
        public static event Action<GameObject> OnClick;//点击事件
        public static event Action<GameObject> OnRayTest;
        public static event Action<GameObject> OnPulleyStable;
        public static event Action OnDialogueManager;
        public static event Action OnEndTyping;
        public static void TriggerLevelCompleted()
        {
            OnLevelCompleted?.Invoke();
        }
        public static void TriggerPryStable(GameObject obj)
        {
            OnPryStable?.Invoke(obj);
        }
        public static void TriggerPryUnstable(GameObject obj)
        {
            OnPryUnstable?.Invoke(obj);
        }
        public static void TriggerOnClick(GameObject obj)//触发点击事件
        {
            OnClick?.Invoke(obj);
        }

        public static void TriggerOnRayTest(GameObject obj)
        {
            OnRayTest?.Invoke(obj);
        }

        public static void TriggerOnPulleyStable(GameObject obj)
        {
            OnPulleyStable?.Invoke(obj);
        }

        public static void TriggerOnDialogueManager()
        {
            OnDialogueManager?.Invoke();
        }

        public static void TriggerOnEndTyping()
        {
            OnEndTyping?.Invoke();
        }
    }
}