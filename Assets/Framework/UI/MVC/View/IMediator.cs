using UnityEngine;
using System.Collections;
using AKBFramework;
using System;

namespace AKBFramework.UI
{
	/// <summary>
	/// 中介者接口
	/// </summary>
	public interface IMediator 
	{
		//	//添加面板
		void AddPanel(BasePanel panel);
		//	//移除面板
		void RemovePanel(BasePanel panel);

		/// <summary>
		/// 面板的事件的发送接口
		/// </summary>
		/// <param name='panel'>
		/// 发送事件的面板
		/// </param>
		/// <param name='type'>
		/// 事件类型
		/// </param>
		/// <param name='param'>
		/// 事件参数
		/// </param>
		void SendEvent(BasePanel panel, string type, System.Object param);
	}
}
