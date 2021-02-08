using System;
using System.Collections.Generic;
using UnityEngine;

namespace AKBFramework
{
    public static class GameObjectExtension
    {
        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            T component = go.GetComponent<T>();
            if(component == null)
            {
                component = go.AddComponent<T>();
                Debug.Assert(component != null, "Can not add component " + typeof(T).ToString(), go);
            }
            return component;
        }

        public static void AddChild(this GameObject parent, GameObject child)
        {
            child.transform.parent = parent.transform;
            child.transform.localPosition = Vector3.zero;
            child.transform.localScale = Vector3.one;
            child.transform.localRotation = Quaternion.identity;
        }

        public static MonoBehaviour GetActiveMonoBehaviour(this GameObject go)
        {
            List<MonoBehaviour> monoBehaviours = new List<MonoBehaviour>(go.GetComponentsInParent<MonoBehaviour>());
            return monoBehaviours.Find(m => m.isActiveAndEnabled);
        }

        public static void AddUpdateTrigger(this GameObject go, Func<bool> condition, Action trigger)
        {
            var updateTrigger = go.GetOrAddComponent<UpdateTrigger>();
            updateTrigger.condition = condition;
            updateTrigger.trigger = trigger;
        }

        public static void AddEndOfFrameTrigger(this GameObject go, Action trigger)
        {
            var updateTrigger = go.GetOrAddComponent<EndOfFrameTrigger>();
            updateTrigger.trigger = trigger;
        }
    }
}