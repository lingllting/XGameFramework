namespace AKBFramework
{
    using UnityEngine;

    public static class GenericExtention
    {
        public static void Example()
        {
            var typeName = GenericExtention.GetTypeName<string>();
            Debug.Log(typeName);   
        }
        
        public static string GetTypeName<T>()
        {
            return typeof(T).ToString();
        }
    }
}