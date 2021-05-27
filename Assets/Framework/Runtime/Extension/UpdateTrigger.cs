using System;
using UnityEngine;

namespace XGameFramework
{
    public class UpdateTrigger : MonoBehaviour
    {
        public Func<bool> condition = null;
        public Action trigger = null;
        
        void Update()
        {
            if (condition == null)
                return;

            if (condition.Invoke())
            {
                trigger?.Invoke();
                Destroy(this);
            }
        }
    }
}