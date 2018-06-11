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

        public Transform Retrieve(GameObject gameObject)
        {
            return Retrieve(gameObject.transform);
        }

		public Transform Retrieve(Transform transform)
		{
			return mSpawnPool.Spawn (transform);
		}

		public void Recycle(Transform transform)
		{
			mSpawnPool.Despawn (transform);
		}

        public PrefabPool AddPrefabPool(Transform poolItem)
        {
            //TODO SpawnPool Settings...
            PrefabPool prefabPool = new PrefabPool(poolItem);
            //TODO PrefabPool Settings...
            mSpawnPool.CreatePrefabPool(prefabPool);
            return prefabPool;
        }
	}

	public class PoolManager : MonoSingleton<PoolManager>
	{
		public Pool CreatePool(Transform prefab)
		{
			SpawnPool spawnPool = InnerPoolManager.Pools.Create (prefab.name);
			Pool pool = new Pool (spawnPool);

            PrefabPool prefabPool = pool.AddPrefabPool(prefab);
			return pool;
		}

        public Pool CreatePool(string poolName)
        {
            SpawnPool spawnPool = InnerPoolManager.Pools.Create(poolName);
            Pool pool = new Pool(spawnPool);
            return pool;
        }
	}
}
