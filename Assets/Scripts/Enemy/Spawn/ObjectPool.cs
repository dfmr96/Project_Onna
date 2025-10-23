using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ObjectPool<T> where T : Component
{
    private T prefab;
    private Transform parent;
    [SerializeField] List<T> pool;

    public ObjectPool(T prefab, int initialSize, Transform parent = null)
    {
        this.prefab = prefab;
        this.parent = parent;
        pool = new List<T>();

        for (int i = 0; i < initialSize; i++)
        {
            CreateNewInstance();
        }
    }

    private T CreateNewInstance()
    {
        T obj = GameObject.Instantiate(prefab, parent);
        obj.gameObject.SetActive(false);
        pool.Add(obj);
        return obj;
    }

    public T Get()
    {
        foreach (T obj in pool)
        {
            if (!obj.gameObject.activeInHierarchy)
            {
                obj.gameObject.SetActive(true);
                return obj;
            }
        }

        return CreateNewInstance();
    }

    public void Release(T obj)
    {
        obj.gameObject.SetActive(false);
    }
}

