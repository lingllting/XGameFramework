namespace AKBFramework
{
    public class RepeatNodeChain : ExecuteNodeChain
    {
        protected override ExecuteNode mNode
        {
            get { return mRepeatNode; }
        }

        private RepeatNode mRepeatNode;

        private SequenceNode mSequenceNode;
        
        public RepeatNodeChain(int repeatCount)
        {
            mSequenceNode = new SequenceNode();
            mRepeatNode = new RepeatNode(mSequenceNode,repeatCount);
        }

        public override IExecuteNodeChain Append(IExecuteNode node)
        {
            mSequenceNode.Append(node);
            return this;
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            if (null != mRepeatNode)
            {
                mRepeatNode.Dispose();
            }

            mRepeatNode = null;
            mSequenceNode = null;
        }
    }
}