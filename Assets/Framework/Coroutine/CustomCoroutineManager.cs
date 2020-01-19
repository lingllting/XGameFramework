using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AKBFramework
{
    public class CustomCoroutineManager : MonoSingleton<CustomCoroutineManager>
    {
        private HashSet<CustomCoroutine> _coroutines = new HashSet<CustomCoroutine>();
        
        /// <summary>
        /// convert coroutine invoked in CustomCoroutineManager to customCoroutine
        /// </summary>
        public new CustomCoroutine StartCoroutine(IEnumerator coroutine)
        {
            return this.StartCustomCoroutine(coroutine);
        }

        public void AddCoroutine(CustomCoroutine customCoroutine)
        {
            _coroutines.Add(customCoroutine);
        }

        public void RemoveCoroutine(CustomCoroutine customCoroutine)
        {
            _coroutines.Remove(customCoroutine);
        }

        /// <summary>
        /// Stop all the custom coroutines
        /// </summary>
        public void Pause()
        {
            HashSet<CustomCoroutine>.Enumerator it = _coroutines.GetEnumerator();
            while (it.MoveNext())
            {
                it.Current.Pause();
            }
        }

        /// <summary>
        /// Resume all the custom coroutines
        /// </summary>
        public void Resume()
        {
            HashSet<CustomCoroutine>.Enumerator it = _coroutines.GetEnumerator();
            while (it.MoveNext())
            {
                it.Current.Resume();
            }
        }
    }
}

