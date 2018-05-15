using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AKBFramework.BehaviorTree
{
	/// <summary>
	/// 行为树节点基类
	/// </summary>
	public interface IBehaviorTreeNode
	{
		bool Proc();
	}

	/// <summary>
	/// 行为树根节点
	/// </summary>
	public class BehaviorTreeRootNode : IBehaviorTreeNode
	{
		public IBehaviorTreeNode root = null;

		public bool Proc()
		{
			return true;
		}
		
		public IBehaviorTreeNode AddChild(IBehaviorTreeNode root)
		{
			this.root = root;
			return this;
		}
	}

	/// <summary>
	/// 行为节点,叶子节点
	/// </summary>
	public class ActionNode : IBehaviorTreeNode
    {
        protected List<ConditionNode> conditions = new List<ConditionNode>();

        public virtual bool Proc()
		{
			return false;
		}

        public IBehaviorTreeNode AddChild(ConditionNode _node)
        {
            conditions.Add(_node);
            return this;
        }

        public void DelChild(ConditionNode _node)
        {
            conditions.Remove(_node);
        }
    }

	/// <summary>
	/// 条件判断节点,叶子节点
	/// </summary>
	public class ConditionNode : IBehaviorTreeNode
	{
		public virtual bool Proc()
		{
			return false;
		}
	}

	/// <summary>
	/// 复合节点，不能为叶子节点
	/// </summary>
	public class CompositeNode : IBehaviorTreeNode
	{
		protected List<IBehaviorTreeNode> children = new List<IBehaviorTreeNode>();
		
		/// <summary>
		/// 由子数实现
		/// </summary>
		/// <returns></returns>
		public virtual bool Proc()
		{
			return true;
		}
		
		public IBehaviorTreeNode AddChild(IBehaviorTreeNode _node)
		{
			children.Add(_node);
			return this;
		}
		
		public void DelChild(IBehaviorTreeNode _node)
		{
			children.Remove(_node);
		}
		
		public void ClearChildren()
		{
			children.Clear();
		}
	}

	/// <summary>
	/// 装饰节点
	/// </summary>
	public class DecoratorNode : IBehaviorTreeNode
	{
		protected IBehaviorTreeNode child = null;
		
		public virtual bool Proc()
		{
			return child.Proc() ;
		}
		
		public IBehaviorTreeNode AddChild(IBehaviorTreeNode _child)
		{
			child = _child;
			return this;
		}
	}

	/// <summary>
	/// 遇到一个child执行后返回true,停止迭代
	/// 本node向自己的的父节点也返回true
	/// 如果所有child返回false,本node向自己父节点返回false
	/// </summary>
	public class SelectorNode : CompositeNode
	{
		public override bool Proc()
		{
			for  (int i = 0; i < children.Count; i++)
			{
				if (children[i].Proc())
				{
					return true;
				}
			}
			return false;
		}
	}
	
	/// <summary>
	/// 遇到一个child执行后返回false,停止迭代
	/// 本node向自己父节点返回flase
	/// 如果所有child返回true,本node向自己父节点返回true
	/// </summary>
	public class SequenceNode : CompositeNode
	{
		public override bool Proc()
		{
			for  (int i = 0; i < children.Count; i++)
			{
				if (!children[i].Proc())
				{
					return false;
				}
			}
			return true;
		}
	}
}
