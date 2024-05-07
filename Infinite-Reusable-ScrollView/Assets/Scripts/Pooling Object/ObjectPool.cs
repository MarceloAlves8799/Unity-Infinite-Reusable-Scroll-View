using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

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

    private void AddObjectToPool()
    {
        GameObject prefabInstance = Instantiate(_prefabToInstantiate, transform);

        string newName = prefabInstance.name.Replace("(Clone)", "");
        prefabInstance.name = newName;
        prefabInstance.SetActive(false);

        _objectPool.Enqueue(prefabInstance);
    }

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

    public void ReturnObjectToPool(GameObject obj)
    {
        if (obj == null)
        {
            return;
        }

        obj.SetActive(false);

        if (obj.TryGetComponent<IPoolable>(out IPoolable poolObj))
        {
            poolObj.OnSpawn();
        }

        _objectPool.Enqueue(obj);
    }

}
