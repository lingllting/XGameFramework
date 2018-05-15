namespace AKBFramework
{
    using System;

    public class CustomObjectFactory<T> : IObjectFactory<T>
    {
        public CustomObjectFactory(Func<T> factoryMethod)
        {
            mFactoryMethod = factoryMethod;
        }
        
        protected Func<T> mFactoryMethod;

        public T Create()
        {
            return mFactoryMethod();
        }
    }
}