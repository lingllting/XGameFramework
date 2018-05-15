#if !(UNITY_IPHONE || UNITY_ANDROID || UNITY_METRO)

using UnityEngine;
using UniRx.Triggers; // for enable gameObject.EventAsObservbale()

namespace UniRx.Examples
{
    public class Sample03_GameObjectAsObservable : MonoBehaviour
    {
        void Start()
        {
            // All events can subscribe by ***AsObservable if enables UniRx.Triggers
            this.OnMouseDownAsObservable()
				.SelectMany(_ => {Debug.Log("SelectMany"); return Observable.ReturnUnit();})
                //.TakeUntil(this.gameObject.OnMouseUpAsObservable())
                //.Select(_ => Input.mousePosition)
				.Select(_ => {Debug.Log("Select"); return Input.mousePosition;})
				//.Do(Debug.Log("Do"))
                //.RepeatUntilDestroy(this)
                .Subscribe(x => Debug.Log(x), ()=> Debug.Log("!!!" + "complete"));
        }
    }
}

#endif