using UnityEngine;
using System.Collections;
using System.Diagnostics;
using AKBFramework.UI;

public class DemoLogicData : BaseObserved
{
	//单例方法读写，但是我这里对写数据做出了限制
	private static DemoLogicData _instance = null;
	public static DemoLogicData Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new DemoLogicData();
			}
			return _instance;
		}
	}
	
	//核心数据更改的权限类
	public DemoLogicDataCommand m_Command;
	public DemoLogicData()
	{
		//创建更改权限类
		m_Command = new DemoLogicDataCommand(this);
	}
	
	/// <summary>
	/// 写数据的权限检查
	/// </summary>
	/// <returns>
	/// 是否修改成功
	/// </returns>
	private bool CheckAccess()
	{
		#if UNITY_EDITOR
		StackTrace trace = new StackTrace();
		StackFrame[] stackFrames = trace.GetFrames();
		if (stackFrames[2].GetMethod().DeclaringType.GetInterface(typeof(ICommand).FullName) == null)
		{
			UnityEngine.Debug.LogError(stackFrames[2].GetMethod().Name + "没有权限修改核心数据: " + stackFrames[1].GetMethod().Name);
			return false;
		}
		else
		{
			return true;
		}
		#else
		return true;
		#endif
	}
	
	//核心数据
	private int _nDataA;
	public int DataA
	{
		get{return _nDataA;}
		set
		{
			if(CheckAccess())
			{
				_nDataA = value;
			}
		}
	}
	private int _nDataB;
	public int DataB
	{
		get{return _nDataB;}
		set
		{
			if(CheckAccess())
			{
				_nDataB = value;
			}
		}
	}
}
