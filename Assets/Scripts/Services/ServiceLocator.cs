using System;
using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> services = new Dictionary<Type, object>();

        /// <summary>
        /// Register a service implementation
        /// </summary>
        public static void Register<T>(T service) where T : class
        {
            var type = typeof(T);
            if (services.ContainsKey(type))
            {
                Debug.LogWarning($"ServiceLocator: Service of type {type.Name} is already registered. Replacing existing service.");
            }

            services[type] = service;
            Debug.Log($"ServiceLocator: Registered service {type.Name}");
        }

        /// <summary>
        /// Get a registered service
        /// </summary>
        public static T Get<T>() where T : class
        {
            var type = typeof(T);
            if (services.TryGetValue(type, out var service))
            {
                return service as T;
            }

            Debug.LogError($"ServiceLocator: Service of type {type.Name} not found. Make sure it's registered before use.");
            return null;
        }

        /// <summary>
        /// Check if a service is registered
        /// </summary>
        public static bool IsRegistered<T>() where T : class
        {
            return services.ContainsKey(typeof(T));
        }

        /// <summary>
        /// Unregister a service (useful for cleanup)
        /// </summary>
        public static void Unregister<T>() where T : class
        {
            var type = typeof(T);
            if (services.Remove(type))
            {
                Debug.Log($"ServiceLocator: Unregistered service {type.Name}");
            }
            else
            {
                Debug.LogWarning($"ServiceLocator: Attempted to unregister service {type.Name} that was not registered");
            }
        }

        /// <summary>
        /// Clear all registered services (useful for scene transitions)
        /// </summary>
        public static void Clear()
        {
            services.Clear();
            Debug.Log("ServiceLocator: All services cleared");
        }

        /// <summary>
        /// Get all registered service types (for debugging)
        /// </summary>
        public static Type[] GetRegisteredTypes()
        {
            var types = new Type[services.Count];
            services.Keys.CopyTo(types, 0);
            return types;
        }
    }
}