using UnityEngine;

namespace AKBFramework
{
    public class WaitForAnimatorEnd : CustomYieldInstruction
    {
        private Animator _animator;
        private string _name;
        private int _loopTime;

        public WaitForAnimatorEnd(Animator animator, string name, int loopTime)
        {
            _animator = animator;
            _name = name;
            _loopTime = loopTime;
        }

        // To keep coroutine suspended return true.
        // To let coroutine proceed with execution return false.
        // keepWaiting property is queried each frame after MonoBehaviour.Update and before MonoBehaviour.LateUpdate.
        public override bool keepWaiting
        {
            get
            {
                if (_animator != null)
                {
                    if (!_animator.GetCurrentAnimatorStateInfo(0).IsName(_name))
                    {
                        return true;
                    }

                    return _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < _loopTime;
                }
                return false;
            }
        }
    }
}
