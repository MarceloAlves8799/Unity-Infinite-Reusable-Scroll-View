using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _prefabToInstantiate;
    [SerializeField] private int _initialPoolSize;

    private Queue<GameObject> _objectPool = new Queue<GameObject>();


    private void Start()
    {
        // Initialize Pool
        for (int i = 0; i < _initialPoolSize; i++)
        {
            AddObjectToPool();
        }
    }

    // Method responsible for Instantiate elements to Pool
    private void AddObjectToPool()
    {
        GameObject prefabInstance = Instantiate(_prefabToInstantiate, transform);

        string newName = prefabInstance.name.Replace("(Clone)", "");
        prefabInstance.name = newName;
        prefabInstance.SetActive(false);

        _objectPool.Enqueue(prefabInstance);
    }

    // Method to be called when need some Prefab instance
    public GameObject GetObjectFromPool()
    {
        if (_objectPool.Count == 0)
        {
            AddObjectToPool();
        }

        GameObject obj = _objectPool.Dequeue();
        obj.SetActive(true);

        if(obj.TryGetComponent<IPoolable>(out IPoolable poolObj))
        {
            poolObj.OnSpawn();
        }

        return obj;
    }

    // Method to be called when need to disable Prefab
    public void ReturnObjectToPool(GameObject obj)
    {
        if (obj == null)
        {
            return;
        }

        obj.transform.SetParent(transform);

        if (obj.TryGetComponent<IPoolable>(out IPoolable poolObj))
        {
            poolObj.OnSpawn();
        }

        _objectPool.Enqueue(obj);
    }

}
