namespace AKBFramework
{
    using UnityEngine.Events;
    using UnityEngine.UI;
    
    public static class ToggleExtension
    {
        public static void RegOnValueChangedEvent(this Toggle selfToggle, UnityAction<bool> onValueChangedEvent)
        {
            selfToggle.onValueChanged.AddListener(onValueChangedEvent);
        }
    }
}