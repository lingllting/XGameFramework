namespace XGameFramework
{
    public class RepeatNodeChain : ExecuteNodeChain
    {
        protected override ExecuteNode ExecuteNode => _repeatNode;

        private RepeatNode _repeatNode;
        private SequenceNode _sequenceNode;
        
        public RepeatNodeChain(int repeatCount)
        {
            _sequenceNode = new SequenceNode();
            _repeatNode = new RepeatNode(_sequenceNode, repeatCount);
        }

        public override IExecuteNodeChain Append(IExecuteNode node)
        {
            _sequenceNode.Append(node);
            return this;
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            _repeatNode?.Dispose();
            _repeatNode = null;
            _sequenceNode = null;
        }
    }
}