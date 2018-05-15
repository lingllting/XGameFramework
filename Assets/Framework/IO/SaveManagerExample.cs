using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AKBFramework;

public class SaveManagerExample : MonoBehaviour
{

	// Use this for initialization
	void Start () 
	{
		SaveManager.Save<int> ("Value1", 25);
		int value = SaveManager.Load<int> ("Value1");
		Debug.Log (value);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
