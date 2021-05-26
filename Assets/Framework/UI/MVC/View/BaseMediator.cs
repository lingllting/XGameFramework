using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XGameFramework.UI
{
	public class BaseMediator : IMediator
	{
		protected List<BasePanel> mPanelList = new List<BasePanel>();
		public void AddPanel(BasePanel panel)
		{
			
		}

		public void RemovePanel(BasePanel panel)
		{
			
		}

		public void SendEvent(BasePanel panel, string type, System.Object param)
		{
			panel.OnEventHandler(type, param);
		}
	}
}
