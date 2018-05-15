namespace AKBFramework
{
	using UnityEngine.Events;

	public static class UnityActionExtension
	{
		public static void Example()
		{
			UnityAction action = () => { };
			UnityAction<int> actionWithInt = num => { };
			UnityAction<int, string> actionWithIntString = (num, str) => { };

			action.InvokeGracefully();
			actionWithInt.InvokeGracefully(1);
			actionWithIntString.InvokeGracefully(1, "str");		
		}
		
		/// <summary>
		/// Call action
		/// </summary>
		/// <param name="selfAction"></param>
		/// <returns> call succeed</returns>
		public static bool InvokeGracefully(this UnityAction selfAction)
		{
			if (null != selfAction)
			{
				selfAction();
				return true;
			}
			return false;
		}

		/// <summary>
		/// Call action
		/// </summary>
		/// <param name="selfAction"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static bool InvokeGracefully<T>(this UnityAction<T> selfAction, T t)
		{
			if (null != selfAction)
			{
				selfAction(t);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Call action
		/// </summary>
		/// <param name="selfAction"></param>
		/// <returns> call succeed</returns>
		public static bool InvokeGracefully<T, K>(this UnityAction<T, K> selfAction, T t, K k)
		{
			if (null != selfAction)
			{
				selfAction(t, k);
				return true;
			}
			return false;
		}
	}
}