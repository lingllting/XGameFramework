using System;
using System.Collections;
using UnityEngine;

namespace XGameFramework
{
    public static class AnimatorExtension
    {
        public static void Play(this Animator animator, string name, Action callback, int loopTime = 1)
        {
            if (animator.isActiveAndEnabled)
            {
                MonoBehaviour monoBehaviour = animator.gameObject.GetActiveMonoBehaviour();
                Debug.Assert(monoBehaviour != null);
                monoBehaviour.StartCoroutine(AnimatorCallback(animator, name, callback, loopTime));
            }
        }
        
        private static IEnumerator AnimatorCallback(Animator animator, string name, Action callback, int loopTime)
        {
            animator.Play(name);
            yield return new WaitForAnimatorEnd(animator, name, loopTime);
            if (animator != null)
            {
                callback?.Invoke();
            }
        }
    }
}
