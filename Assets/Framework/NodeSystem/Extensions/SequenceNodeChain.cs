namespace AKBFramework
{
    /// <summary>
    /// 支持链式方法
    /// </summary>
    public class SequenceNodeChain : ExecuteNodeChain
    {
        protected override ExecuteNode mNode
        {
            get { return mSequenceNode; }
        }

        private SequenceNode mSequenceNode;

        public SequenceNodeChain()
        {
            mSequenceNode = SafeObjectPool<SequenceNode>.Instance.Allocate();
        }

        public override IExecuteNodeChain Append(IExecuteNode node)
        {
            mSequenceNode.Append(node);
            return this;
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            mSequenceNode.Dispose();
            mSequenceNode = null;
        }
    }
}