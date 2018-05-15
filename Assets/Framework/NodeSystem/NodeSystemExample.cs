using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AKBFramework;

public class NodeSystemExample : MonoBehaviour 
{
	SequenceNode sequenceNode = null;
	void Start ()
	{
//		int count = 0;
//		Debug.Log ("Sequence Begin~");
//		this.Sequence ()
//			.Delay (1.0f)
//			.Event (() => Debug.Log ("延迟了1秒！"))
//			.Until (() => {count += 3; Debug.Log(count); return count > 10;})
//			.Delay(1.0f)
//			.Event(() => Debug.Log ("延迟了2秒！"))
//			.Begin()
//			.DisposeWhenFinished()
//			.OnDisposed(() => Debug.Log("Dispose!"));


		this.StartCoroutineChain (TestA, TestB, () => Debug.Log("Completed."));
	}


	IEnumerator TestA()
	{
		yield return new WaitForSeconds (1f);
		Debug.Log ("A");
	}

	IEnumerator TestB()
	{
		yield return new WaitForSeconds (1f);
		Debug.Log ("B");
	}

	void Update ()
	{
		
	}
}
