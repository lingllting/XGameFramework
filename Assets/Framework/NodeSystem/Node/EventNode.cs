namespace XGameFramework
{
    using System;
    
    public class EventNode : ExecuteNode, IPoolable
    {
        private Action _onExecuteEvent;
        
        public static EventNode Allocate(params Action[] onExecuteEvents)
        {
            var retNode = SafeObjectPool<EventNode>.Instance.Allocate();
            Array.ForEach(onExecuteEvents, onExecuteEvent => retNode._onExecuteEvent += onExecuteEvent);
            return retNode;
        }
        
        protected override void OnExecute(float dt)
        {
            _onExecuteEvent.Invoke();
            Finished = true;
        }
        
        protected override void OnDispose()
        {
            SafeObjectPool<EventNode>.Instance.Recycle(this);
        }

        public void OnRecycled()
        {
            Reset();
            _onExecuteEvent = null;
        }

        public bool IsRecycled { get; set; }
    }
}