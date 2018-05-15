namespace AKBFramework
{
    using UnityEngine;
    using UnityEngine.UI;

    public static class GraphicExtension
    {
        public static void Example()
        {
            var gameObject = new GameObject();
            var image = gameObject.AddComponent<Image>();
            var rawImage = gameObject.AddComponent<RawImage>();

            // image.color = new Color(image.color.r,image.color.g,image.color.b,1.0f);
            image.ColorAlpha(1.0f);
            rawImage.ColorAlpha(1.0f);
        }
        
        public static T ColorAlpha<T>(this T selfGraphic, float alpha) where T : Graphic
        {
            var color = selfGraphic.color;
            color.a = alpha;
            selfGraphic.color = color;
            return selfGraphic;
        }
    }
}