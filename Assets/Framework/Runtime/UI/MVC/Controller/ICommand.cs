using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 命令接口
/// </summary>
public interface ICommand 
{
	//命令的执行方法
	void Execute();
	void Execute(Object obj);
	//允许提交命令的类
	string[] AllowClass();
	
	//处理网络消息
	void HandleNetMessage();
	//处理事件
	void HandleEvent();
}
