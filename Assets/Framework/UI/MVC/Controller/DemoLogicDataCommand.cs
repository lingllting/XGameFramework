using UnityEngine;
using System.Collections;
using XGameFramework.UI;

/// <summary>
/// 核心数据命令层
/// </summary>
public class DemoLogicDataCommand : BaseCommand 
{
	//核心数据
	private DemoLogicData _data;
	
	public DemoLogicDataCommand(DemoLogicData data)
	{
		_data = data;
	}
	
	public override void Execute()
	{
		_data.DataA = 11;
		_data.Notify();
	}
	public override void Execute(Object obj)
	{
		
	}
}
