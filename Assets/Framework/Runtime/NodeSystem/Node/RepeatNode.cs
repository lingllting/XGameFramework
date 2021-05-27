namespace XGameFramework
{
    public class RepeatNode : ExecuteNode
    {
        public RepeatNode(IExecuteNode node, int repeatCount)
        {
            _repeatCount = repeatCount;
            _executeNode = node;
        }
        
        private IExecuteNode _executeNode;

        private int _repeatCount = 1;
        private bool _completed = false;

        private int _curRepeatCount = 0;

        protected override void OnReset()
        {
            if (null != _executeNode)
            {
                _executeNode.Reset();
            }
            _curRepeatCount = 0;
            _completed = false;
        }
        
        protected override void OnExecute(float dt)
        {
            if (_repeatCount == -1)
            {
                if (_executeNode.Execute(dt))
                {
                    _executeNode.Reset();
                }
                return;
            }

            if (_executeNode.Execute(dt))
            {
                _executeNode.Reset();
                _curRepeatCount++;
            }

            if (_curRepeatCount == _repeatCount)
            {
                Finished = true;
                _completed = true;
            }
        }

        protected override void OnDispose()
        {
            if (null != _executeNode)
            {
                _executeNode.Dispose();
                _executeNode = null;
            }
        }
    }
}