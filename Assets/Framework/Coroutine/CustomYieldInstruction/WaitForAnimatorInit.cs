using UnityEngine;

namespace XGameFramework
{
    public class WaitForAnimatorInit : CustomYieldInstruction
    {
        private Animator _animator;

        public WaitForAnimatorInit(Animator animator)
        {
            _animator = animator;
        }

        // To keep coroutine suspended return true.
        // To let coroutine proceed with execution return false.
        // keepWaiting property is queried each frame after MonoBehaviour.Update and before MonoBehaviour.LateUpdate.
        public override bool keepWaiting
        {
            get { return !_animator.isInitialized; }
        }
    }
}
