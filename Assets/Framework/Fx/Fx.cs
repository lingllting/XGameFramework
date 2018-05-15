using UnityEngine;
using System.Collections;
using System;

namespace AKBFramework
{
	public class Fx
	{
		public static readonly int	sMaxScaleInfoCount = 200;
		//父结点
		public Transform ParentTransform{get;set;}
		//绑定到的对象(默认是Fx目录下，且坐标缩放旋转都为初始值)
		//如果这个值不为Null,则指定了绑定对象，此时的位置旋转和缩放需要函数传入
		public GameObject BindTarget{get;set;}
		//有绑定对象时所应用的位置旋转缩放参数
		private Vector3 bindLocalPosition = Vector3.zero;
		public Vector3 BindLocalPosition
		{
			get{ return bindLocalPosition;}
			set{ bindLocalPosition = value;}
		}
		private Vector3 bindLocalScale = Vector3.one;
		public Vector3 BindLocalScale
		{
			get{ return bindLocalScale;}
			set{ bindLocalScale = value;}
		}
		private Quaternion bindLocalRotation = Quaternion.identity;
		public Quaternion BindLocalRotation
		{
			get{ return bindLocalRotation;}
			set{ bindLocalRotation = value;}
		}

		private Quaternion originRotation = Quaternion.identity;
		public Quaternion OriginRotation
		{
			get { return originRotation; }
		}

		//设置特效播放时间
		private bool openTimeLimit = false;
		public float DurationAge = 0;
		private float duration;
		public float Duration
		{
			get
			{
				return duration;
			}
		}
		//特效实例
		public GameObject Instance{get;set;}
		//特效根节点下的粒子对象
		public ParticleSystem	RootParticle{get;set;}
		//准备关闭特效
		public bool IsClosing{get;set;}							
		//目标是否被摄像机看见了
		public bool IsView{get;set;}
		//目标是否被使用了
		public bool IsUsed{get;set;}	
		//特效的所有粒子
		private ParticleSystem[] particleArray = new ParticleSystem[sMaxScaleInfoCount];
		public ParticleSystem[] ParticleArray
		{
			get{ return particleArray;}
		}
		//特效的所有拖尾
		private TrailRenderer[] trailArray = new TrailRenderer[sMaxScaleInfoCount];
		public TrailRenderer[] TrailArray
		{
			get{ return trailArray;}
		}
		//特效的所有旧版粒子发射器
		private ParticleEmitter[] particleEmitterArray = new ParticleEmitter[sMaxScaleInfoCount];
		public ParticleEmitter[] ParticleEmitterArray
		{
			get { return particleEmitterArray; }
		}
		//特效的所有旧版粒子动画
		private ParticleAnimator[] particleAnimatorArray = new ParticleAnimator[sMaxScaleInfoCount];
		public ParticleAnimator[] ParticleAnimatorArray
		{
			get { return particleAnimatorArray; }
		}


		private GameObject[] unknowArray = new GameObject[sMaxScaleInfoCount];
		public GameObject[] UnknowArray
		{
			get{ return unknowArray;}
		}

		#region ParticleSystem 参数
		private float[] particleSizeArray = new float[sMaxScaleInfoCount];
		public float[] ParticleSizeArray
		{
			get { return particleSizeArray; }
		}
		private float[] particleSpeedArray = new float[sMaxScaleInfoCount];
		public float[] ParticleSpeedArray
		{
			get { return particleSpeedArray; }
		}
		private float[] particleGravityArray = new float[sMaxScaleInfoCount];
		public float[] ParticleGravityArray
		{
			get { return particleGravityArray; }
		}
		#endregion

		#region ParticleEmitter 参数
		public float[] MinSizeArray = new float[sMaxScaleInfoCount];
		public float[] MaxSizeArray = new float[sMaxScaleInfoCount];
		public Vector3[] WorldVelocityArray = new Vector3[sMaxScaleInfoCount];
		public Vector3[] LocalVelocityArray = new Vector3[sMaxScaleInfoCount];
		public Vector3[] RandomVelocityArray = new Vector3[sMaxScaleInfoCount];
		#endregion

		#region ParticleAnimator 参数
		public Vector3[] ForceArray = new Vector3[sMaxScaleInfoCount];
		public Vector3[] RandomForceArray = new Vector3[sMaxScaleInfoCount];
		#endregion

		#region TrailRenderer 参数
		public float[] StartWidthArray = new float[sMaxScaleInfoCount];
		public float[] EndWidthArray = new float[sMaxScaleInfoCount];
		#endregion

		//特效所有的动画所属的结点
		private GameObject[] animationObjectArray = new GameObject[sMaxScaleInfoCount];
		public GameObject[] AnimationObjectArray
		{
			get{ return animationObjectArray;}
		}
		//新粒子数量
		public int ParticleCount{get;set;}
		//旧粒子数量
		public int LegacyParticleEmitterCount { get; set; }
		public int LegacyParticleAnimatorCount { get; set; }
		//拖尾数量
		public int TrailCount{get;set;}
		//其他
		public int UnknowCount{get;set;}
		//动画数量
		public int AnimationCount{get;set;}


		public void Init(float speed = 1)
		{
			//搜索这个特效以及他的子特效
			if (ParticleCount < 1)
			{
				ParticleCount = 0;
				AnimationCount = 0;
				openTimeLimit = false;

				//设定子特效和动画特效
				WrapMode wrapMode = RootParticle.loop ? WrapMode.Loop : WrapMode.Once;
				Transform[] children = RootParticle.transform.GetComponentsInChildren<Transform>();
				for (int i = 0, count = children.Length; i < count; i++)
				{
					Transform tr = children[i];
					ParticleSystem particle = tr.GetComponent<ParticleSystem>();
					if (particle != null)
					{
						ParticleArray[ParticleCount] = particle;
						ParticleSizeArray[ParticleCount] = particle.startSize;
						ParticleSpeedArray[ParticleCount] = particle.startSpeed;
						ParticleGravityArray[ParticleCount] = particle.gravityModifier;
						ParticleCount++;

						particle.playbackSpeed = speed;
					}

					ParticleEmitter emitter = tr.GetComponent<ParticleEmitter>();
					ParticleAnimator particleAnimator = tr.GetComponent<ParticleAnimator>();
					if (emitter != null)
					{
						ParticleEmitterArray[LegacyParticleEmitterCount] = emitter;
						MinSizeArray[LegacyParticleEmitterCount] = emitter.minSize;
						MaxSizeArray[LegacyParticleEmitterCount] = emitter.maxSize;
						WorldVelocityArray[LegacyParticleEmitterCount] = emitter.worldVelocity;
						LocalVelocityArray[LegacyParticleEmitterCount] = emitter.localVelocity;
						RandomVelocityArray[LegacyParticleEmitterCount] = emitter.rndVelocity;
						LegacyParticleEmitterCount++;
					}
					if (particleAnimator != null)
					{
						ParticleAnimatorArray[LegacyParticleAnimatorCount] = particleAnimator;
						ForceArray[LegacyParticleAnimatorCount] = particleAnimator.force;
						RandomForceArray[LegacyParticleAnimatorCount] = particleAnimator.rndForce;
						LegacyParticleAnimatorCount++;
					}

					if (tr.gameObject.GetComponent<Animation>() != null)
					{
						Animation anim = tr.gameObject.GetComponent<Animation>();
						AnimationObjectArray[AnimationCount] = tr.gameObject;
						anim.wrapMode = wrapMode;
						if (anim.clip != null)
						{
							anim[anim.clip.name].speed = speed;
						}
						AnimationCount++;
					}

					TrailRenderer tlr = tr.GetComponent<TrailRenderer>();
					if(tlr != null)
					{
						TrailArray[TrailCount] = tlr;
						StartWidthArray[TrailCount] = tlr.startWidth;
						EndWidthArray[TrailCount] = tlr.endWidth;
						TrailCount++;
					}
				}
			}
			else
			{
				for (int i = 0; i < ParticleCount; i++)
				{
					ParticleArray[i].playbackSpeed = speed;
				}
				for (int i = 0; i < AnimationCount; i++)
				{
					Animation anim = AnimationObjectArray[i].GetComponent<Animation>();
					if (anim.clip != null)
					{
						anim[anim.clip.name].speed = speed;
					}
				}
			}
		}

		public void BeginClose()
		{
			DurationAge = 0;
			IsClosing = true;
			openTimeLimit = false;
			Play(false);
		}

		public void Close()
		{
			//把父目录归还到默认目录中
			if (ParentTransform)
			{
				Instance.transform.parent = ParentTransform;
			}

			IsUsed = false;
			IsClosing = false;
			Play(false);
			BindTarget = null;
			DurationAge = 0;
			openTimeLimit = false;

			for (int i = 0; i < ParticleCount; i++)
			{
				ParticleArray[i].startSize = ParticleSizeArray[i];
				ParticleArray[i].startSpeed = ParticleSpeedArray[i];
				ParticleArray[i].gravityModifier = ParticleGravityArray[i];
				ParticleArray[i].playbackSpeed = 1;
			}
			for (int i = 0; i < LegacyParticleEmitterCount; i++)
			{
				ParticleEmitterArray[i].minSize = MinSizeArray[i];
				ParticleEmitterArray[i].maxSize = MaxSizeArray[i];
				ParticleEmitterArray[i].worldVelocity = WorldVelocityArray[i];
				ParticleEmitterArray[i].localVelocity = LocalVelocityArray[i];
				ParticleEmitterArray[i].rndVelocity = RandomVelocityArray[i];
			}
			for (int i = 0; i < LegacyParticleAnimatorCount; i++)
			{
				ParticleAnimatorArray[i].force = ForceArray[i];
				ParticleAnimatorArray[i].rndForce = RandomForceArray[i];
			}
			for (int i = 0; i < TrailCount; i++)
			{
				TrailArray[i].startWidth = StartWidthArray[i];
				TrailArray[i].endWidth = EndWidthArray[i];
			}
			for (int i = 0; i < AnimationCount; i++)
			{
				Animation anim = AnimationObjectArray[i].GetComponent<Animation>();
				if (anim.clip != null)
				{
					anim[anim.clip.name].speed = 1;
				}
			}

			Instance.transform.localScale = Vector3.one;
		}

		public void SetDuration(float time)
		{
			this.duration = time;
			this.openTimeLimit = true;
		}

		public bool IsCanClose()
		{
			int syParCount = 0;
			for (int h = 0; h < ParticleCount; h++)
			{		
				syParCount += ParticleArray[h].particleCount;
			}

			return (syParCount <=0);
		}

		/// <summary>
		/// 设置特效是否播放.
		/// </summary>
		/// <param name="isPlay">播放/停止播放.</param>
		public void Play(bool isPlay)
		{
			if (RootParticle == null || RootParticle.gameObject == null)
			{
				return;
			}

			if (isPlay)
			{
				RootParticle.Play();
				for (int i = 0; i < AnimationCount; i++)
				{
					if (AnimationObjectArray[i].GetComponent<Animation>())
					{
						if(RootParticle.loop)
						{
							AnimationObjectArray[i].GetComponent<Animation>().wrapMode = WrapMode.Loop;
						} 
						else
						{
							AnimationObjectArray[i].GetComponent<Animation>().wrapMode = WrapMode.Once;
						}

						AnimationObjectArray[i].SetActive(true);
						AnimationObjectArray[i].GetComponent<Animation>().Play();
					}	
				}
				for (int i = 0; i < LegacyParticleEmitterCount; i++)
				{
					ParticleEmitterArray[i].Emit();
				}

				for (int i = 0; i < TrailCount; i++)
				{
					if(TrailArray[i] !=null)
					{
						TrailArray[i].gameObject.SetActive(true);
					}
				}

				RootParticle.gameObject.SetActive(true);
			} 
			else
			{
				if (RootParticle)
				{
					RootParticle.Stop();

					if (RootParticle.startDelay > 0)
					{
						RootParticle.Clear();
					}
				}

				for (int i = 0;i < AnimationCount; i++)
				{
					if(AnimationObjectArray[i].GetComponent<Animation>())
					{
						AnimationObjectArray[i].GetComponent<Animation>().Stop();
						AnimationObjectArray[i].SetActive(false);
					}
				}

				for (int i = 0; i < TrailCount; i++)
				{
					if(TrailArray[i] !=null)
					{
						TrailArray[i].gameObject.SetActive(false);
					}
				}

				RootParticle.gameObject.SetActive(false);
			}
		}

		//查找效果是否播放
		public bool IsPlaying()
		{
			if (RootParticle == null)
			{
				return false;
			}
			if (openTimeLimit)
			{
				if (DurationAge < Duration)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				return RootParticle.isPlaying;
			}
		}

		public static Fx CreateFxAndPlay(string fxModuleName, GameObject bindObject, Vector3 position, Vector3 scale, Quaternion rot, bool isLoop, bool needRotate = true, float speed = 1)
		{
			if (bindObject == null)
			{
				needRotate = false;
			}
			Fx fx = CreateFx(fxModuleName, isLoop, needRotate, speed);

			if (fx == null)
			{
				Debug.LogError("fx name: " + fxModuleName);
			}
			//绑定到目标节点
			fx.BindToObject(bindObject, position, scale, rot);
			fx.Play(true);
			return fx;
		}

		public static Fx CreateFxAndPlay(string fxModuleName, Vector3 position, Vector3 scale, Quaternion rot, bool isLoop)
		{
			Fx fx = CreateFx(fxModuleName, isLoop, false);

			if (fx == null)
			{
				Debug.LogError("fx name: " + fxModuleName);
			}

			fx.BindTarget = null;
			fx.BindLocalPosition = position;
			fx.BindLocalRotation = rot;
			fx.BindLocalScale = scale;

			fx.Instance.transform.localRotation = fx.BindLocalRotation;
			fx.Instance.transform.localPosition = fx.BindLocalPosition;
			fx.Instance.transform.localScale = fx.BindLocalScale;

			fx.Play(true);
			return fx;
		}

		//设置循环播放选项<播放次数>
		public void setLoopPlay(int _times)
		{
			if(_times ==-1)
			{
				//循环播放
				RootParticle.loop =true;
			} 
			else
			{
				//非循环
				RootParticle.loop = false;
			}

			for(int i =0;i <ParticleCount;i++)
			{
				ParticleArray[i].loop = RootParticle.loop;
			}

			for(int i = 0; i < AnimationCount;i++)
			{
				if(AnimationObjectArray[i].activeInHierarchy)
				{
					AnimationObjectArray[i].SetActive(true);
					if(AnimationObjectArray[i].GetComponent<Animation>())
					{
						if(RootParticle.loop)
						{
							AnimationObjectArray[i].GetComponent<Animation>().wrapMode =WrapMode.Loop;
						} 
						else
						{
							AnimationObjectArray[i].GetComponent<Animation>().wrapMode =WrapMode.Once;
						}

						AnimationObjectArray[i].GetComponent<Animation>().Play();
					}
				}
			}
		}

		public void Update()
		{
			DurationAge += Time.deltaTime;
		}

		public bool IsLoopPlay()
		{
			if (RootParticle == null)
			{
				return false;
			}
			return RootParticle.loop;
		}

		//绑定对象并且调制设置
		private void BindToObject(GameObject bindObject, Vector3 position, Vector3 scale, Quaternion rotation)
		{
			BindTarget = bindObject;
			BindLocalPosition = position;
			BindLocalRotation = rotation;
			BindLocalScale = scale;

			//设置第一次的
			if (BindTarget)
			{
				Instance.transform.parent = BindTarget.transform;
				Instance.transform.localRotation = BindLocalRotation;
				Instance.transform.localPosition = BindLocalPosition;
				RootParticle.transform.localScale = BindLocalScale;
			} 
			else
			{
				Instance.transform.localRotation = BindLocalRotation;
				Instance.transform.Rotate(Vector3.right, 90f, Space.Self);
				Instance.transform.localPosition = BindLocalPosition;
				RootParticle.transform.localScale = BindLocalScale;
			}

			originRotation = Instance.transform.rotation;
		}

		private static Fx CreateFx(string fxName, bool isLoop, bool needRotate, float speed = 1)
		{
			Fx fx = FxManager.Instance.CreateFx(fxName);

			if (fx != null)
			{
				//设置循环播放
				if (isLoop)
				{
					fx.RootParticle.loop = true;
				}
				else
				{
					fx.RootParticle.loop = false;
				}
				//初始化
				fx.Init(speed);
			}

			return fx;
		}
	}
}
