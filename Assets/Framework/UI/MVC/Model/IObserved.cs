using UnityEngine;
using System.Collections;

namespace AKBFramework.UI
{
	/// <summary>
	/// 被观察者接口
	/// </summary>
	public class IObserved 
	{
		/// <summary>
		/// 群发通知更新函数
		/// </summary>
		void Notify(){}

		/// <summary>
		/// 指定面板更新
		/// </summary>
		/// <param name='panel'>
		/// 需要更新的面板
		/// </param>
		void Notify(BasePanel panel){}

		/// <summary>
		/// 群发更新指定内容
		/// </summary>
		/// <param name='hashTable'>
		/// Hash table.
		/// </param>
		void Notify(Hashtable hashTable){}
	}
}
