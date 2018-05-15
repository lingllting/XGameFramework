using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AKBFramework
{
	public class SaveManager
	{
		public static void Save<T>(string key, object value)
		{
			ES3.Save<T>(key, value);
		}

		public static T Load<T>(string key)
		{
			return ES3.Load<T>(key);
		}
	}
}
