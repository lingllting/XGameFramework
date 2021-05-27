using System.Collections;
using UnityEngine;
using XGameFramework;

public class NodeSystemExample : MonoBehaviour 
{
	SequenceNode sequenceNode = null;
	void Start ()
	{
		int count = 0;
		Debug.Log ("Sequence Begin~");
		
		// Sequence Chain
		this.Sequence ()
			.Delay (1.0f)
			.Event (() => Debug.Log ("Delayed One Second！"))
			.Until (() => {count += 3; Debug.Log(count); return count > 10;})
			.Delay(1.0f)
			.Event(() => Debug.Log ("Delayed Two Seconds！"))
			.Begin()
			.DisposeWhenFinished()
			.OnDisposed(() => Debug.Log("Dispose!"));

		// Repeat Chain
		this.Repeat(3).Delay(1.0f).Event(() => Debug.LogError("Repeat!")).Begin();
		
		// Combination of Sequence Chain and Repeat Chain
		this.Sequence()
			.Delay(1.0f, () => Debug.Log("Delay Over."))
			.Event(() => Debug.Log("Repeat Chain Begin:"))
			.Repeat(this.Repeat(3).Delay(1.0f).Event(() => Debug.LogError("Repeat!")))
			.Event(() => Debug.Log("Repeat Chain End.."))
			.Begin();
	}
}
