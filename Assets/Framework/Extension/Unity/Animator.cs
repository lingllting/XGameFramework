using System;
using System.Collections;
using UnityEngine;

namespace AKBFramework
{
    public static class AnimatorExtension
    {
        public static void Play(this Animator animator, string name, Action callback)
        {
            CustomCoroutineManager.Instance.StartCoroutine(AnimatorCallback(animator, name, callback));
        }
        
        private static IEnumerator AnimatorCallback(Animator animator, string name, Action callback)
        {
            animator.Play(name);
            yield return new WaitForAnimatorEnd(animator, name);
            callback();
        }
    }
}
