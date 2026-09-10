using UnityEngine;

namespace Script.Tools
{
    [CreateAssetMenu(fileName = "PlayerAnimationParameters", menuName = "Assets/Animation/PlayerAnimationParameters")]
    public class AnimationParameter : ScriptableObject
    {
        public enum State
        {
            Idle,
            Walk,
            Run
        }
        
        [Header("Animation Parameter Names")]
        [SerializeField]private string walkBool = "Walk";
        [SerializeField]private string runBool = "Run";
        
        private int walkBoolHash;
        private int runBoolHash;

        public int WalkBoolHash => walkBoolHash;
        public int RunBoolHash => runBoolHash;

        public void Initialize()
        {
            walkBoolHash = Animator.StringToHash(walkBool);
            runBoolHash = Animator.StringToHash(runBool);
        }
        
#if UNITY_EDITOR
        private void OnValidate() => Initialize();
#endif
    }
}