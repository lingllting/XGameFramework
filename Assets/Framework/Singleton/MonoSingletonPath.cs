
namespace AKBFramework
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public class MonoSingletonPath : Attribute
    {
		private string mPathInHierarchy;

        public MonoSingletonPath(string pathInHierarchy)
        {
            mPathInHierarchy = pathInHierarchy;
        }

        public string PathInHierarchy
        {
            get { return mPathInHierarchy; }
        }
    }
    
    [Obsolete("QMonoSingletonAttribute is deprecated.prease use QMonoSingletonPath instead")]
    [AttributeUsage(AttributeTargets.Class)]
    public class MonoSingletonAttribute : MonoSingletonPath
    {
        public MonoSingletonAttribute(string pathInHierarchy) : base(pathInHierarchy)
        {
        }
    }
}
