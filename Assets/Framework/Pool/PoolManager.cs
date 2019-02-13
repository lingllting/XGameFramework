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
            Transform tr = mSpawnPool.Spawn(transform);
            tr.gameObject.SetActive(true);
            return tr;
		}

		public void Recycle(Transform transform)
		{
			mSpawnPool.Despawn (transform);
		}

        public PrefabPool AddPrefabPool(Transform poolItem)
        {
            PrefabPool prefabPool = mSpawnPool.GetPrefabPool(poolItem.gameObject);
            if (prefabPool != null)
            {
                return prefabPool;
            }
            //TODO SpawnPool Settings...
            prefabPool = new PrefabPool(poolItem);
            //TODO PrefabPool Settings...
            mSpawnPool.CreatePrefabPool(prefabPool);
            return prefabPool;
        }

        public void Release()
        {
            if (mSpawnPool != null)
            {
                GameObject.Destroy(mSpawnPool.gameObject);
                InnerPoolManager.Pools.Remove(mSpawnPool);
            }
        }
	}

	public class PoolManager : MonoSingleton<PoolManager>
	{
		public Pool CreatePool(Transform prefab)
		{
            string poolName = prefab.name + "Pool";
            Pool pool = CreatePool(poolName);
            PrefabPool prefabPool = pool.AddPrefabPool(prefab);
            return pool;
		}

        public Pool CreatePool(string poolName)
        {
            SpawnPool spawnPool = null;
            if (InnerPoolManager.Pools.ContainsKey(poolName))
            {
                spawnPool = InnerPoolManager.Pools[poolName];
            }
            else
            {
                spawnPool = InnerPoolManager.Pools.Create(poolName);
            }
            Pool pool = new Pool(spawnPool);
            return pool;
        }
	}
}
