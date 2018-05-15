namespace AKBFramework
{
    using System;
    using UnityEngine;
    
    public static class IExecuteNodeChainExtention
    {
        public static IExecuteNodeChain Repeat<T>(this T selfbehaviour, int count = -1) where T : MonoBehaviour
        {
            var retNodeChain = new RepeatNodeChain(count) {Executer = selfbehaviour};
            // dispose when distroyed
            retNodeChain.DisposeWhenGameObjDestroyed();
            return retNodeChain;
        }

        public static IExecuteNodeChain Sequence<T>(this T selfbehaviour) where T : MonoBehaviour
        {
            var retNodeChain = new SequenceNodeChain {Executer = selfbehaviour};
            retNodeChain.DisposeWhenGameObjDestroyed();
            return retNodeChain;
        }

        public static IExecuteNodeChain Delay(this IExecuteNodeChain senfChain, float seconds)
        {
            return senfChain.Append(DelayNode.Allocate(seconds));
        }
        
        /// <summary>
        /// Same as Delayw
        /// </summary>
        /// <param name="senfChain"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static IExecuteNodeChain Wait(this IExecuteNodeChain senfChain, float seconds)
        {
            return senfChain.Append(DelayNode.Allocate(seconds));
        }

        public static IExecuteNodeChain Event(this IExecuteNodeChain selfChain,params Action[] onEvents)
        {
            return selfChain.Append(EventNode.Allocate(onEvents));
        }
        

        public static IExecuteNodeChain Until(this IExecuteNodeChain selfChain, Func<bool> condition)
        {
            return selfChain.Append(UntilNode.Allocate(condition));
        }
    }
}