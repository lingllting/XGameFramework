using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AKBFramework.UI
{
	public abstract class BaseCommand : ICommand
	{
		public virtual void Execute()
		{
		}
		public virtual void Execute(Object obj)
		{
		}

		//允许修改该核心数据的面板对象
		public virtual string[] AllowClass()
		{
			return null;
		}

		public virtual void HandleNetMessage()
		{
		}

		public virtual void HandleEvent()
		{

		}
	}
}
