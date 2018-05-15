namespace AKBFramework
{
	using UnityEngine;
	
	public static class CameraExtension 
	{
		public static void Example()
		{
			var screenshotTexture2D = Camera.main.CaptureCamera(new Rect(0, 0, Screen.width, Screen.height));
			Debug.Log(screenshotTexture2D.width);
		}

		public static Texture2D CaptureCamera(this Camera camera,Rect rect)
		{
			var renderTexture = new RenderTexture(Screen.width,Screen.height,0);
			camera.targetTexture = renderTexture;
			camera.Render();

			RenderTexture.active = renderTexture;

			var screenShot = new Texture2D((int) rect.width, (int) rect.height, TextureFormat.RGB24, false);
			screenShot.ReadPixels(rect,0,0);
			screenShot.Apply();

			camera.targetTexture = null;
			RenderTexture.active = null;
			Object.Destroy(renderTexture);

			return screenShot;
		}
	}
}