using UnityEngine;

namespace Script.Tools.Interface
{
    public interface IState
    {
        public void EnterState(Animator animator, int conditionHash);
        public void UpdateState(Animator animator, int conditionHash);
        public void ExitState(Animator animator, int conditionHash);
    }
}