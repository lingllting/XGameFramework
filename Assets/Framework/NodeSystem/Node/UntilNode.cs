namespace XGameFramework
{
    using System;

    /// <summary>
    /// like filter, add condition
    /// </summary>
    public class UntilNode : ExecuteNode, IPoolable
    {
        private Func<bool> mCondition;

        public static UntilNode Allocate(Func<bool> condition, bool autoDispose = false)
        {
            var retNode = SafeObjectPool<UntilNode>.Instance.Allocate();
            retNode.mCondition = condition;
            return retNode;
        }

        protected override void OnExecute(float dt)
        {
            Finished = mCondition.Invoke();
        }

        protected override void OnDispose()
        {
            SafeObjectPool<UntilNode>.Instance.Recycle(this);
        }

        public void OnRecycled()
        {
            mCondition = null;
        }

        public bool IsRecycled { get; set; }
    }
}