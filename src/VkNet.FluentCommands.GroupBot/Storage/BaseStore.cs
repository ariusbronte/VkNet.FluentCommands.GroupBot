using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace VkNet.FluentCommands.GroupBot.Storage
{
    internal abstract class BaseStore<TKey, T1, T2, T3, TResult> : BaseEventStore<T1, T2, T3, TResult> where TResult : Task
    {
        private readonly ConcurrentDictionary<TKey, Func<T1, T2, T3, TResult>> 
            _store = new ConcurrentDictionary<TKey, Func<T1, T2, T3, TResult>>();
        
        protected void StoreValue(TKey key, Func<T1, T2, T3, TResult> value)
        {
            _store.TryAdd(key, value);
        }
        
        protected ConcurrentDictionary<TKey, Func<T1, T2, T3, TResult>> RetrieveValues()
        {
            return _store;
        }
    }

    internal abstract class BaseEventStore<T1, T2, T3, T4, TResult> where TResult : Task
    {
        private Func<T1, T2, T3, T4, TResult> _eventHandler;

        protected void SetEventHandler(Func<T1, T2, T3, T4, TResult> func)
        {
            _eventHandler = func;
        }

        protected async Task TriggerEventHandler(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            if(_eventHandler != null) await _eventHandler.Invoke(arg1, arg2, arg3, arg4).ConfigureAwait(false);;
        }
    }
    
    internal abstract class BaseEventStore<T1, T2, T3, TResult> where TResult : Task
    {
        private Func<T1, T2, T3, TResult> _eventHandler;

        protected void SetEventHandler(Func<T1, T2, T3, TResult> func)
        {
            _eventHandler = func;
        }

        protected async Task TriggerEventHandler(T1 arg1, T2 arg2, T3 arg3)
        {
            if(_eventHandler != null) await _eventHandler.Invoke(arg1, arg2, arg3).ConfigureAwait(false);
        }
    }
    
    internal abstract class BaseEventStore<T1, T2, TResult> where TResult : Task
    {
        private Func<T1, T2, TResult> _eventHandler;

        protected void SetEventHandler(Func<T1, T2, TResult> func)
        {
            _eventHandler = func;
        }

        protected async Task TriggerEventHandler(T1 arg1, T2 arg2)
        {
            if(_eventHandler != null) await _eventHandler.Invoke(arg1, arg2).ConfigureAwait(false);
        }
    }
}