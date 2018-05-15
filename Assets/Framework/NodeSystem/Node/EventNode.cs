namespace AKBFramework
{
    using System;
    
    /// <inheritdoc />
    /// <summary>
    /// 延时执行节点
    /// </summary>
    public class EventNode : ExecuteNode, IPoolable
    {
        private Action mOnExecuteEvent;

        /// <summary>
        /// TODO:这里填可变参数会有问题
        /// </summary>
        /// <param name="onExecuteEvents"></param>
        /// <returns></returns>
        public static EventNode Allocate(params Action[] onExecuteEvents)
        {
            var retNode = SafeObjectPool<EventNode>.Instance.Allocate();
            Array.ForEach(onExecuteEvents, onExecuteEvent => retNode.mOnExecuteEvent += onExecuteEvent);
            return retNode;
        }

        /// <summary>
        /// finished
        /// </summary>
        protected override void OnExecute(float dt)
        {
            mOnExecuteEvent.Invoke();
            Finished = true;
        }
        
        protected override void OnDispose()
        {
            SafeObjectPool<EventNode>.Instance.Recycle(this);
        }

        public void OnRecycled()
        {
            Reset();
            mOnExecuteEvent = null;
        }

        public bool IsRecycled { get; set; }
    }
}