using UnityEngine;

public class WaitForAnimatorEnd : CustomYieldInstruction
{
    private Animator _animator;
    private string _name;

    public WaitForAnimatorEnd(Animator animator, string name)
    {
        _animator = animator;
        _name = name;
    }

    // To keep coroutine suspended return true.
    // To let coroutine proceed with execution return false.
    // keepWaiting property is queried each frame after MonoBehaviour.Update and before MonoBehaviour.LateUpdate.
    public override bool keepWaiting
    {
        get
        {
            if (!_animator.GetCurrentAnimatorStateInfo(0).IsName(_name))
                return true;
            
            return _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < _animator.GetCurrentAnimatorStateInfo(0).length;
        }
    }
}
