using UnityEngine;
using System.Collections;
using XGameFramework.UI;

public class DemoMediator : BaseMediator
{
	//中介者的面板
	public DemoPanelA panelA;
	public DemoPanelB panelB;
	
	public DemoMediator()
	{
		
	}
	
	public void SendEvent(BasePanel sendPanel, string type, System.Object param)
	{
		switch (type)
		{
		case "B-A":
			Debug.Log("B 告诉 A： " + param as string);
			panelA.gameObject.SetActive(false);
			break;
		case "A-B":
			Debug.Log("A 告诉 B： " + param as string);
			break;
		case "HidePanelB":
			panelB.gameObject.SetActive(false);
			break;
		}
	}
}
