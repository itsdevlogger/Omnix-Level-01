using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtensions
{
    private static readonly Dictionary<GameObject, Dictionary<System.Type, Component>> _componentCache = new();

    public static T GetCachedComponent<T>(this GameObject go) where T : Component
    {
        if (!_componentCache.TryGetValue(go, out var typeDict))
        {
            typeDict = new Dictionary<System.Type, Component>();
            _componentCache[go] = typeDict;
        }

        var type = typeof(T);
        if (!typeDict.TryGetValue(type, out var component))
        {
            component = go.GetComponent<T>();
            if (component != null)
                typeDict[type] = component;
        }

        return component as T;
    }

    public static Component GetCachedComponent(this GameObject go, System.Type type)
    {
        if (!_componentCache.TryGetValue(go, out var typeDict))
        {
            typeDict = new Dictionary<System.Type, Component>();
            _componentCache[go] = typeDict;
        }

        if (!typeDict.TryGetValue(type, out var component))
        {
            component = go.GetComponent(type);
            if (component != null)
                typeDict[type] = component;
        }

        return component;
    }

    public static bool TryGetCachedComponent<T>(this GameObject go, out T component) where T : Component
    {
        component = null;

        if (!_componentCache.TryGetValue(go, out var typeDict))
        {
            typeDict = new Dictionary<System.Type, Component>();
            _componentCache[go] = typeDict;
        }

        var type = typeof(T);
        if (!typeDict.TryGetValue(type, out var cachedComponent))
        {
            cachedComponent = go.GetComponent<T>();
            if (cachedComponent == null)
                return false;
         
            typeDict[type] = cachedComponent;
            component = cachedComponent as T;
            return component != null;
        }

        component = cachedComponent as T;
        return true;
    }
}
