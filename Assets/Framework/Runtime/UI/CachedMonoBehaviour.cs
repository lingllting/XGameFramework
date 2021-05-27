using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XGameFramework
{
	public class CachedMonoBehaviour : MonoBehaviour
	{
		private bool mIsTransformCached = false;
		private Transform mTransform;
		public Transform CachedTransform
		{
			get
			{
				if (!mIsTransformCached)
				{
					mIsTransformCached = true;
					mTransform = this.transform;
				}
				return mTransform;
			}
		}

		private bool mIsGameObjectCached = false;
		private GameObject mGameObject;
		public GameObject CachedGameObject
		{
			get
			{
				if (!mIsGameObjectCached)
				{
					mIsGameObjectCached = true;
					mGameObject = this.gameObject;
				}
				return mGameObject;
			}
		}
	}
}
