namespace AKBFramework
{
    public class RepeatNode : ExecuteNode
    {
        public RepeatNode(IExecuteNode node, int repeatCount)
        {
            RepeatCount = repeatCount;
            mNode = node;
        }
        
        private IExecuteNode mNode;
        
        public int RepeatCount = 1;
        public bool Completed = false;

        private int mCurRepeatCount = 0;

        protected override void OnReset()
        {
            if (null != mNode)
            {
                mNode.Reset();
            }
            mCurRepeatCount = 0;
            Completed = false;
        }
        
        protected override void OnExecute(float dt)
        {
            if (RepeatCount == -1)
            {
                if (mNode.Execute(dt))
                {
                    mNode.Reset();
                }
                return;
            }

            if (mNode.Execute(dt))
            {
                mNode.Reset();
                mCurRepeatCount++;
            }

            if (mCurRepeatCount == RepeatCount)
            {
                Finished = true;
                Completed = true;
            }
        }

        protected override void OnDispose()
        {
            if (null != mNode)
            {
                mNode.Dispose();
                mNode = null;
            }
        }
    }
}