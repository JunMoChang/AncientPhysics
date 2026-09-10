using UnityEngine;

namespace Script.Tools
{
    public class Test : MonoBehaviour
    {
        public AnimationParameter animationParameter;

        void Awake()
        {
            animationParameter.Initialize();
        }
    }
}