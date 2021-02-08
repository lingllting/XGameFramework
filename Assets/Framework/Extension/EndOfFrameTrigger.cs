using System;
using System.Collections;
using UnityEngine;

namespace AKBFramework
{
    public class EndOfFrameTrigger : MonoBehaviour
    {
        public Action trigger;
        private void Awake()
        {
            StartCoroutine(EndOfFrameCoroutine());
        }

        private IEnumerator EndOfFrameCoroutine()
        {
            yield return new WaitForEndOfFrame();
            trigger?.Invoke();
            Destroy(this);
        }
    }
}
