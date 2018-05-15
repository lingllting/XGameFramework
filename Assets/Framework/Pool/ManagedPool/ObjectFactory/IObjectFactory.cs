namespace AKBFramework
{
    public interface IObjectFactory<T>
    {
        T Create();
    }
}