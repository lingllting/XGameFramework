using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathologicalGames;
using InnerPoolManager = PathologicalGames.PoolManager;

namespace AKBFramework
{
	public class Pool
	{
		private readonly SpawnPool mSpawnPool;
		public Pool(SpawnPool innerPool)
		{
			mSpawnPool = innerPool;
		}

		public Transform Retrieve(Transform transform)
		{
			return mSpawnPool.Spawn (transform);
		}

		public void Recycle(Transform transform)
		{
			mSpawnPool.Despawn (transform);
		}
	}

	public class PoolManager : MonoSingleton<PoolManager>
	{
		public Pool CreatePool(Transform prefab)
		{
			SpawnPool spawnPool = InnerPoolManager.Pools.Create (prefab.name);
			//TODO SpawnPool Settings...
			PrefabPool prefabPool = new PrefabPool(prefab);
			//TODO PrefabPool Settings...
			spawnPool.CreatePrefabPool(prefabPool);
			Pool pool = new Pool (spawnPool);
			return pool;
		}
	}
}
