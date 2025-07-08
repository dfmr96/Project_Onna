using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace Core
{
    public static class EventBus
    {
        private static readonly Dictionary<Type, List<Delegate>> Subscribers = new();
        private static readonly Dictionary<Type, object> LastSignals = new();

        public static void Subscribe<T>(Action<T> callback)
        {
            var type = typeof(T);
            if (!Subscribers.ContainsKey(type))
                Subscribers[type] = new List<Delegate>();

            Subscribers[type].Add(callback);

            if (LastSignals.TryGetValue(type, out var lastSignal))
            {
                callback((T)lastSignal);
            }
        }

        public static void Unsubscribe<T>(Action<T> callback)
        {
            var type = typeof(T);
            if (Subscribers.TryGetValue(type, out var list))
            {
                list.Remove(callback);
            }
        }

        public static void Publish<T>(T signal)
        {
            var type = typeof(T);
            LastSignals[type] = signal;

            Debug.Log($"EventBus: Publicando {typeof(T).Name}");

            if (Subscribers.TryGetValue(type, out var list))
            {
                foreach (var callback in list)
                    ((Action<T>)callback).Invoke(signal);
            }
        }
        
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Tools/EventBus/Dump Subscriptions")]
        private static void Dump() => DumpSubscriptions();
#endif
        public static void DumpSubscriptions()
        {
            Debug.Log("EventBus: Mapa de suscripciones actuales:");

            foreach (var kvp in Subscribers)
            {
                var type = kvp.Key;
                var callbacks = kvp.Value;

                Debug.Log($"📨 {type.Name}:");

                foreach (var callback in callbacks)
                {
                    var method = callback.Method;
                    var target = callback.Target;
                    string className = method.DeclaringType?.Name ?? "Unknown";

                    Debug.Log($"   ↳ {className}.{method.Name} (target: {target})");
                }
            }
        }
    }
}