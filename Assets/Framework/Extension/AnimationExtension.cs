using System;
using System.Collections;
using UnityEngine;

namespace AKBFramework
{
    public static class AnimationExtension
    {
        public static void PlaySequenced(this Animation animation, params string[] animations)
        {
            //TODO should start the coroutine in a centralized place, will implement it later.
            MonoBehaviour monoBehaviour = animation.gameObject.GetActiveMonoBehaviour();
            Debug.Assert(monoBehaviour != null);
            monoBehaviour.StartCoroutine(AnimationSequence(animation, animations));
        }

        public static void PlaySafely(this Animation animation, string name)
        {
            if (animation[name] == null)
                return;
            
            animation.Play(name);
        }

        public static void Play(this Animation animation, string name, Action callback)
        {
            //TODO should start the coroutine in a centralized place, will implement it later.
            MonoBehaviour monoBehaviour = animation.gameObject.GetActiveMonoBehaviour();
            Debug.Assert(monoBehaviour != null);
            monoBehaviour.StartCoroutine(AnimationCallback(animation, name, callback));
        }
        
        public static void Play(this Animation animation, string name, Action<object> callback, object param)
        {
            //TODO should start the coroutine in a centralized place, will implement it later.
            MonoBehaviour monoBehaviour = animation.gameObject.GetActiveMonoBehaviour();
            Debug.Assert(monoBehaviour != null);
            monoBehaviour.StartCoroutine(AnimationCallback(animation, name, callback, param));
        }

        private static IEnumerator AnimationCallback(Animation animation, string name, Action callback)
        {
            var time = 0f;
            var animState = animation[name];
            animation.PlaySafely(name);
            yield return new WaitWhile(() => { return (time += Time.deltaTime) < animState.length; });
            yield return new WaitForSeconds(0.1f);
            callback();
        }
        
        private static IEnumerator AnimationCallback(Animation animation, string name, Action<object> callback, object param)
        {
            var time = 0f;
            var animState = animation[name];
            animation.PlaySafely(name);
            yield return new WaitWhile(() => { return (time += Time.deltaTime) < animState.length; });
            yield return new WaitForSeconds(0.1f);
            callback(param);
        }

        private static IEnumerator AnimationSequence(Animation animation, string[] animations)
        {
            for (int index = 0; index < animations.Length; index++)
            {
                var time = 0f;
                var animState = animation[animations[index]];
                animation.PlaySafely(animations[index]);
                yield return new WaitWhile(() => { return (time += Time.deltaTime) < animState.length; });
            }
        }
    }
}
