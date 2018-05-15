namespace AKBFramework
{
    using UnityEngine;
    using UnityEngine.UI;
    
    public static class ImageExtension
    {
        public static void Example()
        {
            var gameObject = new GameObject();
            var image1 = gameObject.AddComponent<Image>();

            image1.FillAmount(0.0f); // image1.fillAmount = 0.0f;
        }
        
        public static Image FillAmount(this Image selfImage, float fillamount)
        {
            selfImage.fillAmount = fillamount;
            return selfImage;
        }
    }
}