using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestoryParent : MonoBehaviour 
{
	void OnDestroy()
	{
		Debug.Log("DestoryParent : OnDestory()");
	}
}
