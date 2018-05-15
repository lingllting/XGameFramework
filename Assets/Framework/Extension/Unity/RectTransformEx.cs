namespace AKBFramework
{
	using UnityEngine;

	public static class RectTransformExtension
    {
		public static Vector2 GetPosInRootTrans(this RectTransform selfRectTransform, Transform rootTrans)
		{
			return RectTransformUtility.CalculateRelativeRectTransformBounds(rootTrans, selfRectTransform).center;
		}

		public static RectTransform AnchorPosX(this RectTransform selfRectTrans, float anchorPosX)
		{
			var anchorPos = selfRectTrans.anchoredPosition;
			anchorPos.x = anchorPosX;
			selfRectTrans.anchoredPosition = anchorPos;
			return selfRectTrans;
		}
		
		public static RectTransform SetSizeWidth(this RectTransform selfRectTrans, float sizeWidth)
		{
			var sizeDelta = selfRectTrans.sizeDelta;
			sizeDelta.x = sizeWidth;
			selfRectTrans.sizeDelta = sizeDelta;
			return selfRectTrans;
		}

		public static RectTransform SetSizeHeight(this RectTransform selfRectTrans, float sizeHeight)
		{
			var sizeDelta = selfRectTrans.sizeDelta;
			sizeDelta.y = sizeHeight;
			selfRectTrans.sizeDelta = sizeDelta;
			return selfRectTrans;
		}
	}
}