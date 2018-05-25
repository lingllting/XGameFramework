using UnityEngine;
using System.Collections;
using System.Reflection;
using System;
using System.Collections.Generic;

namespace AKBFramework.UI
{
	public class BasePanel : BaseItem, IObserver
	{
		private List<IMediator> mMediatorList = new List<IMediator>();
		public List<IMediator> MediatorList
		{
			get{ return mMediatorList;}
		}

		private List<ICommand> mCommandList = new List<ICommand>();
		public List<ICommand> CommandList
		{
			get{ return mCommandList;}
		}
		/// <summary>
		/// 通知更新函数
		/// </summary>
		public virtual void UpdateObserver()
		{
			
		}

		public virtual void OnEventHandler(string type, System.Object param)
		{

		}

		//发送事件
		protected void SendEvent()
		{
			for (int i = 0; i < mMediatorList.Count; i++)
			{
				mMediatorList[i].SendEvent(this, "", null);
			}
		}
		protected void SendEvent(string type, System.Object param)
		{
			for (int i = 0; i < mMediatorList.Count; i++)
			{
				mMediatorList[i].SendEvent(this, type, param);
			}
		}

		public void Hide()
		{
			this.gameObject.SetActive(false);
		}
	}
}