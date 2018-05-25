using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AKBFramework;

public class Destroyer : MonoBehaviour 
{
	public GameObject Target;

	void Start()
	{
		RFCObject.ManualRegister(this);
	}

	[RFC(RFCType.DestroyListener, typeof(DestoryChild))]
	void Listener()
	{
		Debug.Log("监听到了销毁消息！");
	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Delete))
		{
			Destroy(Target);
		}
	}
}
