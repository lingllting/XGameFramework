namespace AKBFramework
{
	using UnityEngine;

	public static class LightmapExtension 
	{
		public static void SetAmbientLightHTMLStringColor(string htmlStringColor)
		{
			RenderSettings.ambientLight = htmlStringColor.HtmlStringToColor();
		}
	}
}