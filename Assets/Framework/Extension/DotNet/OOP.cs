namespace AKBFramework
{
    using System;
    using System.Linq;

    public static class OOPExtension
    {
        interface ExampleInterface
        {

        }

        public static void Example()
        {
            if (typeof(OOPExtension).ImplementsInterface<ExampleInterface>())
            {
            }

            if (new object().ImplementsInterface<ExampleInterface>())
            {
            }
        }


        /// <summary>
        /// Determines whether the type implements the specified interface
        /// and is not an interface itself.
        /// </summary>
        /// <returns><c>true</c>, if interface was implementsed, <c>false</c> otherwise.</returns>
        /// <param name="type">Type.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static bool ImplementsInterface<T>(this Type type)
        {
            return !type.IsInterface && type.GetInterfaces().Contains(typeof(T));
        }
        
        /// <summary>
        /// Determines whether the type implements the specified interface
        /// and is not an interface itself.
        /// </summary>
        /// <returns><c>true</c>, if interface was implementsed, <c>false</c> otherwise.</returns>
        /// <param name="type">Type.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static bool ImplementsInterface<T>(this object obj)
        {
            var type = obj.GetType();
            return !type.IsInterface && type.GetInterfaces().Contains(typeof(T));
        }
    }
}
