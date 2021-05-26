using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
#if MVC
using strange.extensions.command.api;
using strange.extensions.mediation.api;
#endif

namespace XGameFramework.UI
{
	public class BaseUITools 
	{
		/// <summary>
		/// 自动将面板脚本绑定到指定面板
		/// </summary>
		/// <param name='strNameSpace'>
		/// String name space.
		/// </param>
		static public List<BasePanel> BindPanel(GameObject UI, string strNameSpace)
		{
			List<BasePanel> components = new List<BasePanel>();
			Canvas[] panels = UI.GetComponentsInChildren<Canvas>();
			//先绑定BasePanel
			for (int i = 0; i < panels.Length; i++)
			{
				Type type = Type.GetType(strNameSpace + panels[i].name);
				if (type == null) continue;
				if (type.BaseType != typeof(BasePanel)) 
				{
					if (type.BaseType == typeof(BaseItem))
					{
						if (panels[i].gameObject.GetComponent(type) == null)
						{
							Debug.Log("aaaaaaaaaaaaaaaaaaaaaaaaa");
							panels[i].gameObject.AddComponent(type);
						}					
					}
					continue;
				}

				if (panels[i].gameObject.GetComponent(type) == null)
				{
					components.Add(panels[i].gameObject.AddComponent(type) as BasePanel);
				}
				else
				{
					components.Add(panels[i].gameObject.GetComponent(type) as BasePanel);
				}
			}

			//再绑定BaseItem，因为Item要先去寻找父节点
			for (int i = 0; i < panels.Length; i++)
			{
				Type type = Type.GetType(strNameSpace + panels[i].name);
				if (type == null) continue;
				if (type.BaseType != typeof(BasePanel) && type.BaseType == typeof(BaseItem))
				{
					if (panels[i].gameObject.GetComponent(type) == null)
					{
						Debug.Log("aaaaaaaaaaaaaaaaaaaaaaaaa");
						panels[i].gameObject.AddComponent(type);
					}
				}
			}

			return components;
		}

		/// <summary>
		/// 绑定中介器的面板
		/// </summary>
		/// <param name='mediator'>
		/// 要绑定的中介者
		/// </param>
		/// <param name='panels'>
		/// 面板们
		/// </param>
		static public void BindMediator(IMediator mediator, List<BasePanel> panels)
		{
			FieldInfo[] fieldInfos = mediator.GetType().GetFields();
			for (int i = 0; i < fieldInfos.Length; i++)
			{
				if (fieldInfos[i].GetValue(mediator) != null)
				{
					continue;
				}
				if (fieldInfos[i].FieldType.BaseType != typeof(BasePanel))
				{
					continue;
				}
				
				for (int j = 0; j < panels.Count; j++)
				{
					if (fieldInfos[i].FieldType == panels[j].GetType())
					{
						fieldInfos[i].SetValue(mediator, panels[j]);
						panels[i].MediatorList.Add(mediator);
					}
				}
			}
		}
		
		/// <summary>
		/// 绑定面板和被面板观察的数据
		/// </summary>
		/// <param name='observed'>
		/// 被观察者
		/// </param>
		/// <param name='observers'>
		/// 观察者
		/// </param>
		static public void BindObserver(BaseObserved observed, List<BasePanel> observers)
		{
			FieldInfo[] fieldInfos = observed.GetType().GetFields();
			for (int i = 0; i < fieldInfos.Length; i++)
			{
				if(fieldInfos[i].GetValue(observed) != null)
				{
					continue;
				}
				if (fieldInfos[i].FieldType.GetInterface("IObserver") == null)
				{
					continue;
				}
				
				for (int j = 0; j < observers.Count; j++)
				{
					if (fieldInfos[i].FieldType == observers[j].GetType())
					{
						fieldInfos[i].SetValue(observed, observers[j]);
						observed.AddObserver(observers[j]);
					}
				}
			}
		}
		
		/// <summary>
		/// 绑定面板和数据命令层
		/// </summary>
		/// <param name='command'>
		/// 数据命令层
		/// </param>
		/// <param name='panels'>
		/// 面板
		/// </param>
		static public void BindCommand(ICommand command, List<BasePanel> panels)
		{
			string[] allowClass = command.AllowClass();
			for (int i = 0; i < allowClass.Length; i++)
			{
				for (int j = 0; j < panels.Count; j++)
				{
					if (allowClass[i] == panels[j].GetType().ToString())
					{
						panels[j].CommandList.Add(command);
					}
				}
			}
		}
	}
}
