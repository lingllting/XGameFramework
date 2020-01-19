using System;
using System.Collections.Generic;
using UnityEngine;

namespace AKBFramework
{
#region Event interface

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class EventPriority : Attribute
    {
        public int Priority = 0;
        public EventPriority(int priority) { Priority = priority; }
    }

    public delegate void OnEvent(params object[] param);

#endregion

    public class EventDispatcher : Singleton<EventDispatcher>
    {
        private readonly Dictionary<int, ListenerWrap> mAllListenerMap = new Dictionary<int, ListenerWrap>(50);

        public bool IsRecycled { get; set; }
        public EventDispatcher() { }

#region internal structure of event listener

        private class ListenerWrap
        {
            private List<OnEvent> mEventList;

            public bool Fire(int key, params object[] param)
            {
                if (mEventList == null)
                {
                    return false;
                }

                for (int i = 0; i < mEventList.Count; i++)
                {
                    if (mEventList[i] == null || mEventList[i].Target.Equals(null))
                    {
                        mEventList.RemoveAt(i);
                        i--;
                        continue;
                    }

                    mEventList[i](param);
                }
                return true;
            }

            public bool Add(OnEvent listener)
            {
                if (mEventList == null)
                {
                    mEventList = new List<OnEvent>();
                }

                if (mEventList.Contains(listener))
                {
                    return false;
                }

                mEventList.Add(listener);
                mEventList.Sort((a, b) =>
                {
                    var aPrioritys = a.Method.GetCustomAttributes(typeof(EventPriority), true);
                    var bPrioritys = b.Method.GetCustomAttributes(typeof(EventPriority), true);
                    if ((aPrioritys.Equals(null) || (aPrioritys.Length == 0)) && (bPrioritys.Equals(null) || (bPrioritys.Length == 0)))
                    {
                        return 0;
                    }
                    else if ((aPrioritys.Equals(null) || (aPrioritys.Length == 0)) && (!bPrioritys.Equals(null) && (bPrioritys.Length > 0)))
                    {
                        return -1;
                    }
                    else if ((!aPrioritys.Equals(null) && (aPrioritys.Length > 0)) && (bPrioritys.Equals(null) || (bPrioritys.Length == 0)))
                    {
                        return 1;
                    }
                    else if ((!aPrioritys.Equals(null) && (aPrioritys.Length > 0)) && (!bPrioritys.Equals(null) && (bPrioritys.Length > 0)))
                    {
                        var aPriority = aPrioritys[0] as EventPriority;
                        var bPriority = bPrioritys[0] as EventPriority;
                        if (aPriority.Priority <= bPriority.Priority)
                        {
                            return -1;
                        }
                        else
                        {
                            return 1;
                        }
                    }
                    else
                    {
                        return 0;
                    }
                });
                return true;
            }

            public void Remove(OnEvent listener)
            {
                if (mEventList == null)
                {
                    return;
                }

                mEventList.Remove(listener);
            }

            public void RemoveAll()
            {
                if (mEventList == null)
                {
                    return;
                }

                mEventList.Clear();
            }
        }

#endregion

#region Instance functions

        public bool Register<T>(T key, OnEvent fun) where T : IConvertible
        {
            var kv = key.ToInt32(null);
            ListenerWrap wrap;
            if (!mAllListenerMap.TryGetValue(kv, out wrap))
            {
                wrap = new ListenerWrap();
                mAllListenerMap.Add(kv, wrap);
            }

            if (wrap.Add(fun))
            {
                return true;
            }

            Debug.LogError("Already Register Same Event:" + key);
            return false;
        }

        public void UnRegister<T>(T key, OnEvent fun) where T : IConvertible
        {
            ListenerWrap wrap;
            if (mAllListenerMap.TryGetValue(key.ToInt32(null), out wrap))
            {
                wrap.Remove(fun);
            }
        }

        public void UnRegister<T>(T key) where T : IConvertible
        {
            ListenerWrap wrap;
            if (mAllListenerMap.TryGetValue(key.ToInt32(null), out wrap))
            {
                wrap.RemoveAll();
                wrap = null;

                mAllListenerMap.Remove(key.ToInt32(null));
            }
        }

        public bool Send<T>(T key, params object[] param) where T : IConvertible
        {
            int kv = key.ToInt32(null);
            ListenerWrap wrap;
            if (mAllListenerMap.TryGetValue(kv, out wrap))
            {
                return wrap.Fire(kv, param);
            }
            return false;
        }

        public void OnRecycled()
        {
            mAllListenerMap.Clear();
        }

#endregion


#region High frequency API
        public static bool SendEvent<T>(T key, params object[] param) where T : IConvertible
        {
            return Instance.Send(key, param);
        }

        public static bool RegisterEvent<T>(T key, OnEvent fun) where T : IConvertible
        {
            return Instance.Register(key, fun);
        }

        public static void UnRegisterEvent<T>(T key, OnEvent fun) where T : IConvertible
        {
            Instance.UnRegister(key, fun);
        }
#endregion
    }
}