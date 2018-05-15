using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AKBFramework.UI
{
	/// <summary>
	/// 被观察者基类
	/// </summary>
	public class BaseObserved : IObserved
	{
		//观察者列表
		protected List<IObserver> mObserverList = new List<IObserver>();
		/// <summary>
		/// 添加观察者
		/// </summary>
		/// <param name='observer'>
		/// 观察者
		/// </param>
		public void AddObserver(IObserver observer)
		{
			mObserverList.Add(observer);
		}
		//通知更新函数
		public virtual void Notify()
		{
			for (int i = 0; i < mObserverList.Count; i++)
			{
				mObserverList[i].UpdateObserver();
			}
		}
		public virtual void Notify(BasePanel panel){}
		public virtual void Notify(Hashtable hashTable){}
	}
}
