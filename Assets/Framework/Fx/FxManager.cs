using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AKBFramework
{
	public class FxManager : MonoSingleton<FxManager>
	{
		//特效列表
		private List<Fx> fxList = new List<Fx>(0);
		public List<Fx> FxList
		{
			get{ return fxList;}
		}

		public bool Inited
		{
			get;
			set;
		}

		public bool InitComponent()
		{
			Inited = true;    
			return true;
		}

		public void StartBattle()
		{

		}

		public void EndBattle()
		{

		}

		public void Destroy()
		{
			DestroyAllFx();
		}

		public void DestroyAllFx()
		{
			Fx fx = null;
			for (int i = 0; i < FxList.Count; i++)
			{
				fx = FxList[i];
				for (int j = 0; j < Fx.sMaxScaleInfoCount; j++)
				{
					fx.ParticleArray[j] = null;
					fx.AnimationObjectArray[j] = null;
					fx.ParticleCount = 0;
				}

				fx.RootParticle = null;
				fx.BindTarget = null;
				fx.ParentTransform = null;

				GameObject.Destroy(fx.Instance);
				fx.Instance = null;
				fx = null;
			}

			FxList.Clear();
		}

		/// <summary>
		/// 创建一个特效对象.
		/// </summary>
		/// <returns>特效对象.</returns>
		/// <param name="fxName">特效的名字.</param>
		public Fx CreateFx(string fxName)
		{
			if (fxName == "")
			{
				return null;
			}

			//先从池中找未使用的特效-
			Fx fx = null;
			for (int i = 0; i < FxList.Count; i++)
			{
				fx = FxList[i];
				if (fx.Instance && fx.Instance.name == fxName && !fx.IsUsed && !fx.IsClosing)
				{
					fx.IsUsed = true;

					if (fx.RootParticle)
					{
						fx.RootParticle.time = 0.0f;
					}

					fx.Play(false);
					fx.RootParticle.Clear();

					return fx;
				}
			}

			Fx newFx = new Fx();
			newFx.Instance = new GameObject("Fx_" + fxName);
			ParticleSystem ps =  newFx.Instance.AddComponent<ParticleSystem>();
			ps.GetComponent<Renderer>().enabled = false;
			//newFx.Instance = ysResManager.Instance().GetEffectPrefab(fxName);
			newFx.Instance.SetActive(true);
			//ysFixShaderTool.ResetShader(newFx.Instance);
			newFx.Instance.name = fxName;
			newFx.RootParticle = newFx.Instance.GetComponent<ParticleSystem>();
			newFx.IsUsed = true;

			FxList.Add(newFx);

			newFx.Instance.transform.parent = this.transform;
			newFx.Instance.transform.localPosition = Vector3.zero;
			newFx.Instance.transform.localRotation = Quaternion.identity;
			newFx.Instance.transform.localScale = Vector3.one;
			newFx.ParentTransform = this.transform;
			newFx.Play(false);

			return newFx;
		}

		void Update() 
		{
			//先关闭特效管理器中所有的特效视野标志开关-
			Fx fx = null;
			for (int i = 0; i < FxList.Count; i++)
			{
				fx = FxList[i];
				fx.IsView = false;

				//对于准备回收的特效做回收条件判断
				if (fx.IsClosing)
				{
					if (fx.IsCanClose())
					{
						fx.Close();
					}
				}
				else
				{
					if (fx.IsUsed)
					{
						fx.Update();
						if (!fx.IsPlaying() && !fx.IsLoopPlay())
						{
							fx.BeginClose();
						}
					}
				}
			}
		}
	}
}
