using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace PathologicalGames
{
    
    /// <description>
    /// Online Docs: 
    ///     http://docs.poolmanager2.path-o-logical.com/code-reference/spawnpool
    ///     
    ///	A special List class that manages object pools and keeps the scene 
    ///	organized.
    ///	
    ///  * Only active/spawned instances are iterable. Inactive/despawned
    ///    instances in the pool are kept internally only.
    /// 
    ///	 * Instanciated objects can optionally be made a child of this GameObject
    ///	   (reffered to as a 'group') to keep the scene hierachy organized.
    ///		 
    ///	 * Instances will get a number appended to the end of their name. E.g. 
    ///	   an "Enemy" prefab will be called "Enemy(Clone)001", "Enemy(Clone)002", 
    ///	   "Enemy(Clone)003", etc. Unity names all clones the same which can be
    ///	   confusing to work with.
    ///		   
    ///	 * Objects aren't destroyed by the Despawn() method. Instead, they are
    ///	   deactivated and stored to an internal queue to be used again. This
    ///	   avoids the time it takes unity to destroy gameobjects and helps  
    ///	   performance by reusing GameObjects. 
    ///		   
    ///  * Two events are implimented to enable objects to handle their own reset needs. 
    ///    Both are optional.
    ///      1) When objects are Despawned BroadcastMessage("OnDespawned()") is sent.
    ///		 2) When reactivated, a BroadcastMessage("OnRespawned()") is sent. 
    ///		    This 
    /// </description>
    [AddComponentMenu("Path-o-logical/PoolManager/SpawnPool")]
    public sealed class SpawnPool : MonoBehaviour, IList<Transform>
    {
        #region Inspector Parameters
        /// <summary>
        /// Returns the name of this pool used by PoolManager. This will always be the
        /// same as the name in Unity, unless the name contains the work "Pool", which
        /// PoolManager will strip out. This is done so you can name a prefab or
        /// GameObject in a way that is development friendly. For example, "EnemiesPool" 
        /// is easier to understand than just "Enemies" when looking through a project.
        /// </summary>
        public string poolName = "";

        /// <summary>
        /// Matches new instances to the SpawnPool GameObject's scale.
        /// </summary>
        public bool matchPoolScale = false;

        /// <summary>
        /// Matches new instances to the SpawnPool GameObject's layer.
        /// </summary>
        public bool matchPoolLayer = false;

        /// <summary>
        /// If True, do not reparent instances under the SpawnPool's Transform.
        /// </summary>
        public bool dontReparent = false;
		
		/// <summary>
        /// If true, the Pool's group, GameObject, will be set to Unity's 
        /// Object.DontDestroyOnLoad()
        /// </summary>
        public bool dontDestroyOnLoad
		{
			get
			{
				return this._dontDestroyOnLoad;
			}
			
			set
			{
				this._dontDestroyOnLoad = value;
				
				if (this.group != null)
					Object.DontDestroyOnLoad(this.group.gameObject);
			}
		}
        public bool _dontDestroyOnLoad = false;  // Property backer and used by GUI.
		
        /// <summary>
        /// Print information to the Unity Console
        /// </summary>
        public bool logMessages = false;

        /// <summary>
        /// A list of PreloadDef options objects used by the inspector for user input
        /// </summary>
        public List<PrefabPool> _perPrefabPoolOptions = new List<PrefabPool>();

        /// <summary>
        /// Used by the inspector to store this instances foldout states.
        /// </summary>
        public Dictionary<object, bool> prefabsFoldOutStates = new Dictionary<object, bool>();
        #endregion Inspector Parameters



        #region Public Code-only Parameters
        /// <summary>
        /// The time in seconds to stop waiting for particles to die.
        /// A warning will be logged if this is triggered.
        /// </summary>
        public float maxParticleDespawnTime = 300;

        /// <summary>
        /// The group is an empty game object which will be the parent of all
        /// instances in the pool. This helps keep the scene easy to work with.
        /// </summary>
        public Transform group { get; private set; }

        /// <summary>
        /// Returns the prefab of the given name (dictionary key)
        /// </summary>
        public PrefabsDict prefabs = new PrefabsDict();

        // Keeps the state of each individual foldout item during the editor session
        public Dictionary<object, bool> _editorListItemStates = new Dictionary<object, bool>();

        /// <summary>
        /// Readonly access to prefab pools via a dictionary<string, PrefabPool>.
        /// </summary>
        public Dictionary<string, PrefabPool> prefabPools
        {
            get
            {
                var dict = new Dictionary<string, PrefabPool>();

                for (int i = 0; i < this._prefabPools.Count; i++)
                    dict[this._prefabPools[i].prefabGO.name] = this._prefabPools[i];

                return dict;
            }
        }
        #endregion Public Code-only Parameters



        #region Private Properties
        private List<PrefabPool> _prefabPools = new List<PrefabPool>();
        internal List<Transform> _spawned = new List<Transform>();
        #endregion Private Properties



        #region Constructor and Init
        private void Awake()
        {
            if (this._dontDestroyOnLoad) Object.DontDestroyOnLoad(this.gameObject);
            this.group = this.transform;

            if (this.poolName == "")
            {
                this.poolName = this.group.name.Replace("Pool", "");
                this.poolName = this.poolName.Replace("(Clone)", "");
            }

            if (this.logMessages)
                Debug.Log(string.Format("SpawnPool {0}: Initializing..", this.poolName));

            //这里是针对在Inspector界面设定的PrefabPool的处理
            for (int i = 0; i < this._perPrefabPoolOptions.Count; i++)
            {
                if (this._perPrefabPoolOptions[i].prefab == null)
                {
                    continue;
                }

                //手动调用一下构造函数，因为Inspector的PrefabPool不会进构造函数
                this._perPrefabPoolOptions[i].inspectorInstanceConstructor();
                this.CreatePrefabPool(this._perPrefabPoolOptions[i]);
            }
            PoolManager.Pools.Add(this);
        }

		public delegate GameObject InstantiateDelegate(GameObject prefab, Vector3 pos, Quaternion rot);
		public delegate void DestroyDelegate(GameObject instance);

		/// <summary>
		/// This can be used to intercept Instantiate and Destroy to implement your own handling. See 
		/// PoolManagerExampleFiles/Scripts/InstanceHandlerDelegateExample.cs.
		/// 
		/// Simply add your own delegate and it will be run to create a new instance. 
		/// 
		/// If at least one delegate is added to InstanceHandler.InstantiateDelegates it will be used instead of 
		/// Unity's Instantiate.
		/// 
		/// Setting a delegate on the SpawnPool here will override a global delegate, if used. See the 
		/// static InstanceHandler for details on global override delegates
		/// </summary>
		public InstantiateDelegate instantiateDelegates;

		/// <summary>
		/// This can be used to intercept Instantiate and Destroy to implement your own handling. See 
		/// PoolManagerExampleFiles/Scripts/InstanceHandlerDelegateExample.cs.
		/// 
		/// Simply add your own delegate and it will be run to destroy an instance. 
		/// 
		/// If at least one delegate is added to InstanceHandler.DestroyDelegates it will be used instead of 
		/// Unity's Instantiate.
		/// </summary>
		public DestroyDelegate destroyDelegates;
		
		/// <summary>
		/// See the InstantiateDelegates docs
		/// </summary>
		/// <param name="prefab">The prefab to spawn an instance from</param>
		/// <param name="pos">The position to spawn the instance</param>
		/// <param name="rot">The rotation of the new instance</param>
		/// <returns>Transform</returns>
		internal GameObject InstantiatePrefab(GameObject prefab, Vector3 pos, Quaternion rot)
		{
			if (this.instantiateDelegates != null)
			{
				return this.instantiateDelegates(prefab, pos, rot);
			}
			else
			{
				return InstanceHandler.InstantiatePrefab(prefab, pos, rot);
			}
		}
		
		
		/// <summary>
		/// See the DestroyDelegates docs
		/// </summary>
		/// <param name="prefab">The prefab to spawn an instance from</param>
		/// <returns>void</returns>
		internal void DestroyInstance(GameObject instance)
		{
			if (this.destroyDelegates != null)
			{
				this.destroyDelegates(instance);
			}
			else
			{
				InstanceHandler.DestroyInstance(instance);
			}
		}


        /// <summary>
        /// Runs when this group GameObject is destroyed and executes clean-up
        /// </summary>
        private void OnDestroy()
        {
            if (this.logMessages)
                Debug.Log(string.Format("SpawnPool {0}: Destroying...", this.poolName));

			if (PoolManager.Pools.ContainsValue(this))
				PoolManager.Pools.Remove(this);

            this.StopAllCoroutines();

            // We don't need the references to spawns which are about to be destroyed
            this._spawned.Clear();

			// Clean-up
            foreach (PrefabPool pool in this._prefabPools) 
			{
				pool.SelfDestruct();
			}

            // Probably overkill, and may not do anything at all, but...
            this._prefabPools.Clear();
            this.prefabs._Clear();
        }

		public void CreatePrefabPool(PrefabPool prefabPool)
		{
			bool isAlreadyPool = this.GetPrefabPool(prefabPool.prefab.gameObject) == null ? false : true;
			if (isAlreadyPool)
			{
				throw new System.Exception (string.Format
            	(
					"Prefab '{0}' is already in  SpawnPool '{1}'. Prefabs can be in more than 1 SpawnPool but " +
					"cannot be in the same SpawnPool twice.",
					prefabPool.prefab, 
					this.poolName
				));
			}
			
			prefabPool.spawnPool = this;
			this._prefabPools.Add(prefabPool);
			this.prefabs._Add(prefabPool.prefab.name, prefabPool.prefab);
			
			if (prefabPool.preloaded != true)
			{
				prefabPool.PreloadInstances();
			}
		}

        /// <summary>
        /// Add an existing instance to this pool. This is used during game start 
        /// to pool objects which are not instanciated at runtime
        /// </summary>
        /// <param name="instance">The instance to add</param>
        /// <param name="prefabName">
        /// The name of the prefab used to create this instance
        /// </param>
        /// <param name="despawn">True to depawn on start</param>
        /// <param name="parent">True to make a child of the pool's group</param>
        public void Add(Transform instance, string prefabName, bool despawn, bool parent)
        {
            for (int i = 0; i < this._prefabPools.Count; i++)
            {
                if (this._prefabPools[i].prefabGO == null)
                {
                    Debug.LogError("Unexpected Error: PrefabPool.prefabGO is null");
                    return;
                }

                if (this._prefabPools[i].prefabGO.name == prefabName)
                {
                    this._prefabPools[i].AddUnpooled(instance, despawn);
                    if (parent) 
					{
						var worldPositionStays = !(instance is RectTransform);
						instance.SetParent(this.group, worldPositionStays);
					}

                    // New instances are active and must be added to the internal list 
                    if (!despawn) this._spawned.Add(instance);
                    return;
                }
            }

            // Log an error if a PrefabPool with the given name was not found
            Debug.LogError(string.Format("SpawnPool {0}: PrefabPool {1} not found.",
                                         this.poolName,
                                         prefabName));

        }
        #endregion Constructor and Init

        #region IList接口实现
        public void Add(Transform item)
        {
        }
        public void Remove(Transform item)
        {
        }
		#endregion

        #region Pool Functionality
        /// <summary>
        /// 获取一个资源实例.
        /// </summary>
        public Transform Spawn(Transform prefab, Vector3 pos, Quaternion rot, Transform parent)
        {
            Transform inst;
			bool worldPositionStays;

            for (int i = 0; i < this._prefabPools.Count; i++)
            {
                if (this._prefabPools[i].prefabGO == prefab.gameObject)
                {
                    inst = this._prefabPools[i].SpawnInstance(pos, rot);
                    if (inst == null) return null;

					worldPositionStays = !(inst is RectTransform);

					if (parent != null)
					{
						inst.SetParent(parent, worldPositionStays);
					}
                    else if (!this.dontReparent && inst.parent != this.group)
					{
						inst.SetParent(this.group, worldPositionStays);
					}
                    this._spawned.Add(inst);
	                inst.gameObject.BroadcastMessage("OnSpawned", this, SendMessageOptions.DontRequireReceiver);
                    return inst;
                }
            }

            PrefabPool newPrefabPool = new PrefabPool(prefab);
            this.CreatePrefabPool(newPrefabPool);
            inst = newPrefabPool.SpawnInstance(pos, rot);
			worldPositionStays = !(inst is RectTransform);
			if (parent != null)
			{
				inst.SetParent(parent, worldPositionStays);
			}
			else if (!this.dontReparent && inst.parent != this.group)
			{
				inst.SetParent(this.group, worldPositionStays);
			}
            this._spawned.Add(inst);
            inst.gameObject.BroadcastMessage("OnSpawned",this,SendMessageOptions.DontRequireReceiver);
            return inst;
        }
			
        public Transform Spawn(Transform prefab, Vector3 pos, Quaternion rot)
        {
            Transform inst = this.Spawn(prefab, pos, rot, null);
            return inst;
        }

        /// <summary>
        /// See primary Spawn method for documentation.
        /// 
        /// Overload to take only a prefab and instance using an 'empty' 
        /// position and rotation.
        /// </summary>
        public Transform Spawn(Transform prefab)
        {
            return this.Spawn(prefab, Vector3.zero, Quaternion.identity);
        }


        /// <summary>
        /// See primary Spawn method for documentation.
        /// 
        /// Convienince overload to take only a prefab  and parent the new 
        /// instance under the given parent
        /// </summary>
        public Transform Spawn(Transform prefab, Transform parent)
        {
            return this.Spawn(prefab, Vector3.zero, Quaternion.identity, parent);
        }
		
		
		#region GameObject Overloads
		public Transform Spawn(GameObject prefab, Vector3 pos, Quaternion rot, Transform parent)
		{
			return Spawn(prefab.transform, pos, rot, parent);
		}
		
		public Transform Spawn(GameObject prefab, Vector3 pos, Quaternion rot)
		{
			return Spawn(prefab.transform, pos, rot);
		}
		
		public Transform Spawn(GameObject prefab)
		{
			return Spawn(prefab.transform);
		}
		
		public Transform Spawn(GameObject prefab, Transform parent)
		{
			return Spawn(prefab.transform, parent);
		}
		#endregion GameObject Overloads
		
		
        /// <summary>
        /// See primary Spawn method for documentation.
        /// 
        /// Overload to take only a prefab name. The cached reference is pulled  
        /// from the SpawnPool.prefabs dictionary.
        /// </summary>
        public Transform Spawn(string prefabName)
        {
            Transform prefab = this.prefabs[prefabName];
            return this.Spawn(prefab);
        }


        /// <summary>
        /// See primary Spawn method for documentation.
        /// 
        /// Convienince overload to take only a prefab name and parent the new 
        /// instance under the given parent
        /// </summary>
        public Transform Spawn(string prefabName, Transform parent)
        {
            Transform prefab = this.prefabs[prefabName];
            return this.Spawn(prefab, parent);
        }


        /// <summary>
        /// See primary Spawn method for documentation.
        /// 
        /// Overload to take only a prefab name. The cached reference is pulled from 
        /// the SpawnPool.prefabs dictionary. An instance will be set to the passed 
        /// position and rotation.
        /// </summary>
        public Transform Spawn(string prefabName, Vector3 pos, Quaternion rot)
        {
            Transform prefab = this.prefabs[prefabName];
            return this.Spawn(prefab, pos, rot);
        }


        /// <summary>
        /// See primary Spawn method for documentation.
        /// 
        /// Convienince overload to take only a prefab name and parent the new 
        /// instance under the given parent. An instance will be set to the passed 
        /// position and rotation.
        /// </summary>
        public Transform Spawn(string prefabName, Vector3 pos, Quaternion rot, 
                               Transform parent)
        {
            Transform prefab = this.prefabs[prefabName];
            return this.Spawn(prefab, pos, rot, parent);
        }


        public AudioSource Spawn(AudioSource prefab, Vector3 pos, Quaternion rot)
        {
            return this.Spawn(prefab, pos, rot, null);
        }

        public AudioSource Spawn(AudioSource prefab)
        {
            return this.Spawn(prefab, Vector3.zero, Quaternion.identity, null);
        }
		
		public AudioSource Spawn(AudioSource prefab, Transform parent)
        {
            return this.Spawn(prefab, Vector3.zero, Quaternion.identity, parent);
        }
		
        public AudioSource Spawn(AudioSource prefab, Vector3 pos, Quaternion rot, Transform parent)
        {
            Transform inst = Spawn(prefab.transform, pos, rot, parent);
            if (inst == null) return null;
            var src = inst.GetComponent<AudioSource>();
            src.Play();
            this.StartCoroutine(this.ListForAudioStop(src));
            return src;
        }

        public ParticleSystem Spawn(ParticleSystem prefab, Vector3 pos, Quaternion rot)
        {
            return Spawn(prefab, pos, rot, null);
        }

        public ParticleSystem Spawn(ParticleSystem prefab, Vector3 pos, Quaternion rot, Transform parent)
        {
            Transform inst = this.Spawn(prefab.transform, pos, rot, parent);
            if (inst == null) return null;
            var emitter = inst.GetComponent<ParticleSystem>();
            //emitter.Play(true);
            this.StartCoroutine(this.ListenForEmitDespawn(emitter));
            return emitter;
        }
        
        public void Despawn(Transform instance)
        {
            // Find the item and despawn it
            bool despawned = false;
            for (int i = 0; i < this._prefabPools.Count; i++)
            {
                if (this._prefabPools[i]._spawned.Contains(instance))
                {
                    despawned = this._prefabPools[i].DespawnInstance(instance);
                    break;
                }
                else if (this._prefabPools[i]._despawned.Contains(instance))
                {
                    Debug.LogError(
                        string.Format("SpawnPool {0}: {1} has already been despawned. " +
                                       "You cannot despawn something more than once!",
                                        this.poolName,
                                        instance.name));
                    return;
                }
            }

            // If still false, then the instance wasn't found anywhere in the pool
            if (!despawned)
            {
                Debug.LogError(string.Format("SpawnPool {0}: {1} not found in SpawnPool",
                               this.poolName,
                               instance.name));
                return;
            }

            // Remove from the internal list. Only active instances are kept. 
            // 	 This isn't needed for Pool functionality. It is just done 
            //	 as a user-friendly feature which has been needed before.
            this._spawned.Remove(instance);
        }


        /// <summary>
        ///	See docs for Despawn(Transform instance) for basic functionalty information.
        ///		
        /// Convienince overload to provide the option to re-parent for the instance 
        /// just before despawn.
        /// </summary>
        public void Despawn(Transform instance, Transform parent)
        {
			// Spawn the new instance (Note: prefab already set in PrefabPool)
			bool worldPositionStays = !(instance is RectTransform);
			instance.SetParent(parent, worldPositionStays);
            this.Despawn(instance);
        }


        /// <description>
        /// See docs for Despawn(Transform instance). This expands that functionality.
        ///   If the passed object is managed by this SpawnPool, it will be 
        ///   deactivated and made available to be spawned again.
        /// </description>
        /// <param name="item">The transform of the instance to process</param>
        /// <param name="seconds">The time in seconds to wait before despawning</param>
        public void Despawn(Transform instance, float seconds)
        {
            this.StartCoroutine(this.DoDespawnAfterSeconds(instance, seconds, false, null));
        }


        /// <summary>
        ///	See docs for Despawn(Transform instance) for basic functionalty information.
        ///		
        /// Convienince overload to provide the option to re-parent for the instance 
        /// just before despawn.
        /// </summary>
        public void Despawn(Transform instance, float seconds, Transform parent)
        {
            this.StartCoroutine(this.DoDespawnAfterSeconds(instance, seconds, true, parent));
        }


        /// <summary>
        /// Waits X seconds before despawning. See the docs for DespawnAfterSeconds()
        /// the argument useParent is used because a null parent is valid in Unity. It will 
        /// make the scene root the parent
        /// </summary>
        private IEnumerator DoDespawnAfterSeconds(Transform instance, float seconds, bool useParent, Transform parent)
        {
            GameObject go = instance.gameObject;
            while (seconds > 0)
            {
                yield return null;

                // If the instance was deactivated while waiting here, just quit
                if (!go.activeInHierarchy)
                    yield break;
                
                seconds -= Time.deltaTime;
            }

            if (useParent)
                this.Despawn(instance, parent);
            else
                this.Despawn(instance);
        }


        /// <description>
        /// Despawns all active instances in this SpawnPool
        /// </description>
        public void DespawnAll()
        {
            var spawned = new List<Transform>(this._spawned);
            for (int i = 0; i < spawned.Count; i++)
                this.Despawn(spawned[i]);
        }


        /// <description>
        ///	Returns true if the passed transform is currently spawned.
        /// </description>
        /// <param name="item">The transform of the gameobject to test</param>
        public bool IsSpawned(Transform instance)
        {
            return this._spawned.Contains(instance);
        }

        #endregion Pool Functionality



        #region Utility Functions
        /// <summary>
        /// 根据GameObject查找PrefabTool.
        /// </summary>
        public PrefabPool GetPrefabPool(GameObject prefab)
        {
            for (int i = 0; i < this._prefabPools.Count; i++)
            {
				if (this._prefabPools [i].prefabGO == null) 
				{
					Debug.LogError(string.Format("SpawnPool {0}: PrefabPool.prefabGO is null", this.poolName));
				}

				if (this._prefabPools [i].prefabGO == prefab)
				{
					return this._prefabPools[i];
				}
            }
            return null;
        }

		/// <summary>
		/// 根据对象实例查找资源.
		/// </summary>
        public GameObject GetPrefab(GameObject instance)
        {
			for (int i = 0; i < this._prefabPools.Count; i++)
			{
				if (this._prefabPools [i].Contains (instance.transform))
				{
					return this._prefabPools [i].prefabGO;
				}
			}
            return null;
        }

        private IEnumerator ListForAudioStop(AudioSource src)
        {
            // Safer to wait a frame before testing if playing.
            yield return null;

			GameObject srcGameObject = src.gameObject;
            while (src.isPlaying)
			{
                yield return null;
			}

			// Handle despawed while still playing
			if (!srcGameObject.activeInHierarchy)
			{
				src.Stop();
				yield break;
			}

            this.Despawn(src.transform);
        }


        // ParticleSystem (Shuriken) Version...
        private IEnumerator ListenForEmitDespawn(ParticleSystem emitter)
        {
            // Wait for the delay time to complete
            // Waiting the extra frame seems to be more stable and means at least one 
            //  frame will always pass
			yield return new WaitForSeconds(emitter.main.startDelay.constantMax + 0.25f);

            // Do nothing until all particles die or the safecount hits a max value
            float safetimer = 0;   // Just in case! See Spawn() for more info
			GameObject emitterGO = emitter.gameObject;
			while (emitter.IsAlive(true) && emitterGO.activeInHierarchy)
            {
                safetimer += Time.deltaTime;
                if (safetimer > this.maxParticleDespawnTime)
                    Debug.LogWarning
                    (
                        string.Format
                        (
                            "SpawnPool {0}: " +
                                "Timed out while listening for all particles to die. " +
                                "Waited for {1}sec.",
                            this.poolName,
                            this.maxParticleDespawnTime
                        )
                    );

                yield return null;
            }

            // Turn off emit before despawning
			if (emitterGO.activeInHierarchy)
			{
				this.Despawn(emitter.transform);
				emitter.Clear(true);
			}
        }

        #endregion Utility Functions

        /// <summary>
        /// Returns a formatted string showing all the spawned member names
        /// </summary>
        public override string ToString()
        {
            // Get a string[] array of the keys for formatting with join()
            var name_list = new List<string>();
            foreach (Transform item in this._spawned)
                name_list.Add(item.name);

            // Return a comma-sperated list inside square brackets (Pythonesque)
            return System.String.Join(", ", name_list.ToArray());
        }


        /// <summary>
        /// Read-only index access. You can still modify the instance at the given index.
        /// Read-only reffers to setting an index to a new instance reference, which would
        /// change the list. Setting via index is never needed to work with index access.
        /// </summary>
        /// <param name="index">int address of the item to get</param>
        /// <returns></returns>
        public Transform this[int index]
        {
            get { return this._spawned[index]; }
            set { throw new System.NotImplementedException("Read-only."); }
        }

        /// <summary>
        /// The name "Contains" is misleading so IsSpawned was implimented instead.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(Transform item)
        {
            string message = "Use IsSpawned(Transform instance) instead.";
            throw new System.NotImplementedException(message);
        }


        /// <summary>
        /// Used by OTHERList.AddRange()
        /// This adds this list to the passed list
        /// </summary>
        /// <param name="array">The list AddRange is being called on</param>
        /// <param name="arrayIndex">
        /// The starting index for the copy operation. AddRange seems to pass the last index.
        /// </param>
        public void CopyTo(Transform[] array, int arrayIndex)
        {
            this._spawned.CopyTo(array, arrayIndex);
        }


        /// <summary>
        /// Returns the number of items in this (the collection). Readonly.
        /// </summary>
        public int Count
        {
            get { return this._spawned.Count; }
        }


        /// <summary>
        /// Impliments the ability to use this list in a foreach loop
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Transform> GetEnumerator()
        {
            for (int i = 0; i < this._spawned.Count; i++)
                yield return this._spawned[i];
        }

        /// <summary>
        /// Impliments the ability to use this list in a foreach loop
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int i = 0; i < this._spawned.Count; i++)
                yield return this._spawned[i];
        }

        // Not implemented
        public int IndexOf(Transform item) { throw new System.NotImplementedException(); }
        public void Insert(int index, Transform item) { throw new System.NotImplementedException(); }
        public void RemoveAt(int index) { throw new System.NotImplementedException(); }
        public void Clear() { throw new System.NotImplementedException(); }
        public bool IsReadOnly { get { throw new System.NotImplementedException(); } }
        bool ICollection<Transform>.Remove(Transform item) { throw new System.NotImplementedException(); }

    }
		
    public class PrefabsDict : IDictionary<string, Transform>
    {
        #region Public Custom Memebers
        /// <summary>
        /// Returns a formatted string showing all the prefab names
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            // Get a string[] array of the keys for formatting with join()
            var keysArray = new string[this._prefabs.Count];
            this._prefabs.Keys.CopyTo(keysArray, 0);

            // Return a comma-sperated list inside square brackets (Pythonesque)
            return string.Format("[{0}]", System.String.Join(", ", keysArray));
        }
        #endregion Public Custom Memebers


        #region Internal Dict Functionality
        // Internal Add and Remove...
        internal void _Add(string prefabName, Transform prefab)
        {
            this._prefabs.Add(prefabName, prefab);
        }

        internal bool _Remove(string prefabName)
        {
            return this._prefabs.Remove(prefabName);
        }

        internal void _Clear()
        {
            this._prefabs.Clear();
        }
        #endregion Internal Dict Functionality


        #region Dict Functionality
        // Internal (wrapped) dictionary
        private Dictionary<string, Transform> _prefabs = new Dictionary<string, Transform>();

        /// <summary>
        /// Get the number of SpawnPools in PoolManager
        /// </summary>
        public int Count { get { return this._prefabs.Count; } }

        /// <summary>
        /// Returns true if a prefab exists with the passed prefab name.
        /// </summary>
        /// <param name="prefabName">The name to look for</param>
        /// <returns>True if the prefab exists, otherwise, false.</returns>
        public bool ContainsKey(string prefabName)
        {
            return this._prefabs.ContainsKey(prefabName);
        }

        /// <summary>
        /// Used to get a prefab when the user is not sure if the prefabName is used.
        /// This is faster than checking Contains(prefabName) and then accessing the dict
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(string prefabName, out Transform prefab)
        {
            return this._prefabs.TryGetValue(prefabName, out prefab);
        }

        #region Not Implimented

        public void Add(string key, Transform value)
        {
            throw new System.NotImplementedException("Read-Only");
        }

        public bool Remove(string prefabName)
        {
            throw new System.NotImplementedException("Read-Only");
        }

        public bool Contains(KeyValuePair<string, Transform> item)
        {
            string msg = "Use Contains(string prefabName) instead.";
            throw new System.NotImplementedException(msg);
        }

        public Transform this[string key]
        {
            get
            {
                Transform prefab;
                try
                {
                    prefab = this._prefabs[key];
                }
                catch (KeyNotFoundException)
                {
                    string msg = string.Format("A Prefab with the name '{0}' not found. " +
                                                "\nPrefabs={1}",
                                                key, this.ToString());
                    throw new KeyNotFoundException(msg);
                }

                return prefab;
            }
            set
            {
                throw new System.NotImplementedException("Read-only.");
            }
        }

        public ICollection<string> Keys
        {
            get
            {
                return this._prefabs.Keys;
            }
        }


        public ICollection<Transform> Values
        {
            get
            {
                return this._prefabs.Values;
            }
        }


        #region ICollection<KeyValuePair<string, Transform>> Members
        private bool IsReadOnly { get { return true; } }
        bool ICollection<KeyValuePair<string, Transform>>.IsReadOnly { get { return true; } }

        public void Add(KeyValuePair<string, Transform> item)
        {
            throw new System.NotImplementedException("Read-only");
        }

        public void Clear() { throw new System.NotImplementedException(); }

        private void CopyTo(KeyValuePair<string, Transform>[] array, int arrayIndex)
        {
            string msg = "Cannot be copied";
            throw new System.NotImplementedException(msg);
        }

        void ICollection<KeyValuePair<string, Transform>>.CopyTo(KeyValuePair<string, Transform>[] array, int arrayIndex)
        {
            string msg = "Cannot be copied";
            throw new System.NotImplementedException(msg);
        }

        public bool Remove(KeyValuePair<string, Transform> item)
        {
            throw new System.NotImplementedException("Read-only");
        }
        #endregion ICollection<KeyValuePair<string, Transform>> Members
        #endregion Not Implimented




        #region IEnumerable<KeyValuePair<string, Transform>> Members
        public IEnumerator<KeyValuePair<string, Transform>> GetEnumerator()
        {
            return this._prefabs.GetEnumerator();
        }
        #endregion



        #region IEnumerable Members
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this._prefabs.GetEnumerator();
        }
        #endregion

        #endregion Dict Functionality

    }

}


public class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
{
	private readonly IDictionary<TKey, TValue> _dictionary;
	
	public ReadOnlyDictionary(IDictionary<TKey, TValue> dictionary)
	{
		_dictionary = dictionary;
	}
	
	#region IDictionary<TKey,TValue> Members
	
	void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
	{
		throw ReadOnlyException();
	}
	
	public bool ContainsKey(TKey key)
	{
		return _dictionary.ContainsKey(key);
	}
	
	public ICollection<TKey> Keys
	{
		get { return _dictionary.Keys; }
	}
	
	bool IDictionary<TKey, TValue>.Remove(TKey key)
	{
		throw ReadOnlyException();
	}
	
	public bool TryGetValue(TKey key, out TValue value)
	{
		return _dictionary.TryGetValue(key, out value);
	}
	
	public ICollection<TValue> Values
	{
		get { return _dictionary.Values; }
	}
	
	public TValue this[TKey key]
	{
		get
		{
			return _dictionary[key];
		}
	}
	
	TValue IDictionary<TKey, TValue>.this[TKey key]
	{
		get
		{
			return this[key];
		}
		set
		{
			throw ReadOnlyException();
		}
	}
	
	#endregion
	
	#region ICollection<KeyValuePair<TKey,TValue>> Members
	
	void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
	{
		throw ReadOnlyException();
	}
	
	void ICollection<KeyValuePair<TKey, TValue>>.Clear()
	{
		throw ReadOnlyException();
	}
	
	public bool Contains(KeyValuePair<TKey, TValue> item)
	{
		return _dictionary.Contains(item);
	}
	
	public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
	{
		_dictionary.CopyTo(array, arrayIndex);
	}
	
	public int Count
	{
		get { return _dictionary.Count; }
	}
	
	public bool IsReadOnly
	{
		get { return true; }
	}
	
	bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
	{
		throw ReadOnlyException();
	}
	
	#endregion
	
	#region IEnumerable<KeyValuePair<TKey,TValue>> Members
	
	public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
	{
		return _dictionary.GetEnumerator();
	}
	
	#endregion
	
	#region IEnumerable Members
	
	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
	
	#endregion
	
	private static System.Exception ReadOnlyException()
	{
		return new System.NotSupportedException("This dictionary is read-only");
	}
}