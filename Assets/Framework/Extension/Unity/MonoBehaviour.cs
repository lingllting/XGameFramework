using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;

namespace AKBFramework
{
	public static class MonoBehaviourExtension 
	{
		public static void StartCoroutineChain(this MonoBehaviour selfMonoBehaviour, Func<IEnumerator> coroutine1, Func<IEnumerator> coroutine2, Action FinishCallBack)
		{
			Observable.FromCoroutine (coroutine1).Concat (Observable.FromCoroutine (coroutine2)).DoOnCompleted (FinishCallBack).Subscribe();
		}

		public static void StartCoroutineChain(this MonoBehaviour selfMonoBehaviour, Func<IEnumerator> coroutine1, Func<IEnumerator> coroutine2, Func<IEnumerator> coroutine3, Action FinishCallBack)
		{
			Observable.FromCoroutine (coroutine1).Concat (Observable.FromCoroutine (coroutine2)).Concat (Observable.FromCoroutine (coroutine3)).DoOnCompleted (FinishCallBack).Subscribe();
		}
	}
}
