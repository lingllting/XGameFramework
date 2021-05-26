namespace XGameFramework
{
    public class SequenceNodeChain : ExecuteNodeChain
    {
        protected override ExecuteNode ExecuteNode => _sequenceNode;

        private SequenceNode _sequenceNode;

        public SequenceNodeChain()
        {
            _sequenceNode = SafeObjectPool<SequenceNode>.Instance.Allocate();
        }

        public override IExecuteNodeChain Append(IExecuteNode node)
        {
            _sequenceNode.Append(node);
            return this;
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            _sequenceNode.Dispose();
            _sequenceNode = null;
        }
    }
}