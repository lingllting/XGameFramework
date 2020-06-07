using UnityEngine;
using System.Collections;

namespace AKBFramework
{
    public enum CustomCoroutineState
    {
        Ready,
        Running,
        Pause,
        Finished,
    }

    public class CustomCoroutine
    {
        public delegate void OnFinish(CustomCoroutine customCoroutine);
        // callback of finish event
        public event OnFinish onFinish;

        private IEnumerator _routine;
        public IEnumerator Routine
        {
            get 
            {
                return _routine;
            }
        }

        private CustomCoroutineState _state;
        public CustomCoroutineState State 
        {
            get
            {
                return _state;
            }
        }

        public CustomCoroutine(IEnumerator routine)
        {
            _routine = routine;
            _state = CustomCoroutineState.Ready;
        }

        public IEnumerator Start()
        {
            if (_state != CustomCoroutineState.Ready)
            {
                throw new System.InvalidOperationException("Unable to start coroutine in state: " + _state);
            }

            CustomCoroutineManager.Instance.AddCoroutine(this);

            _state = CustomCoroutineState.Running;
            do
            {
                try
                {
                    if (!_routine.MoveNext())
                    {
                        _state = CustomCoroutineState.Finished;
                        break;
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("Exception in coroutine: " + ex.Message);
                    _state = CustomCoroutineState.Finished;
                    break;
                }

                yield return _routine.Current;
                while (_state == CustomCoroutineState.Pause)
                {
                    yield return null;
                }
            }
            while (_state == CustomCoroutineState.Running);

            _state = CustomCoroutineState.Finished;
            
            if (onFinish != null)
            {
                onFinish(this);
            }

            CustomCoroutineManager.Instance.RemoveCoroutine(this);
        }

        public void Stop()
        {
            _state = CustomCoroutineState.Finished;
        }

        public void Pause()
        {
            if (_state == CustomCoroutineState.Running)
            {
                _state = CustomCoroutineState.Pause;
            }
        }

        public void Resume()
        {
            if (_state == CustomCoroutineState.Pause)
            {
                _state = CustomCoroutineState.Running;
            }
        }
    }

    public static class CoroutineExtension
    {
        public static CustomCoroutine StartCustomCoroutine(this MonoBehaviour mb, IEnumerator routine)
        {
            if (mb != null && routine != null)
            {
                CustomCoroutine customCoroutine = new CustomCoroutine(routine);
                mb.StartCoroutine(customCoroutine.Start());
                return customCoroutine;
            }
            return null;
        }
    }

}
