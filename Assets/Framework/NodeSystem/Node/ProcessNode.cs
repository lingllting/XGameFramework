namespace XGameFramework
{
    public class ProcessNode : ExecuteNode
    {
        protected string mTips = "Default";

        public virtual float Progress { get; set; }

        public virtual string Tips
        {
            get { return mTips; }
            set { mTips = value; }
        }
    }
}