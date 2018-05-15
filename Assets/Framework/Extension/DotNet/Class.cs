namespace AKBFramework
{
	public static class ClassExtention
	{
		public static void Example()
		{
			var simpleClass = new object();

			if (simpleClass.IsNull()) // simpleClass == null
			{
				// do sth
			}
			else if (simpleClass.IsNotNull()) // simpleClasss != null
			{
				// do sth
			}
		}

		public static bool IsNull<T>(this T selfObj) where T : class
		{
			return null == selfObj;
		}
		
		public static bool IsNotNull<T>(this T selfObj) where T : class
		{
			return null != selfObj;
		}
	}
}