using UnityEngine;
using System.Collections;

namespace AKBFramework.UI
{
	/// <summary>
	/// 观察者模式目标接口
	/// </summary>
	public interface IObserver 
	{
		//更新函数
		void UpdateObserver();
	}

}
