using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectPoolItem
{
    public GameObject objectToPool;
    public int amountToPool;
    public string poolName;
    public bool shouldExpand = true;
}

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler sharedInstance;

    public const string DefaultRootObjectPoolName = "Pooled Objects";
    public List<ObjectPoolItem> itemsToPool;
    public List<GameObject> pooledObjects;

    private string rootPoolName = DefaultRootObjectPoolName;


    private void Awake()
    {
        sharedInstance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (string.IsNullOrEmpty(rootPoolName))
            rootPoolName = DefaultRootObjectPoolName;

        GetParentPoolObject(rootPoolName);

        pooledObjects = new List<GameObject>();
        foreach(var item in itemsToPool)
        {
            for (int i = 0; i < item.amountToPool; i++)
            {
                CreatePooledObject(item);
            }
        }        
    }

    private GameObject GetParentPoolObject(string objectPoolName)
    {
        // Use the root object pool name if no name was specified
        if (string.IsNullOrEmpty(objectPoolName))
            objectPoolName = rootPoolName;

        GameObject parentObject = GameObject.Find(objectPoolName);

        // Create the parent object if necessary
        if (parentObject == null)
        {
            parentObject = new GameObject();
            parentObject.name = objectPoolName;

            // Add sub pools to the root object pool if necessary
            if (objectPoolName != rootPoolName)
                parentObject.transform.parent = GameObject.Find(rootPoolName).transform;
        }

        return parentObject;
    }

    public GameObject GetPooledObject(string tag)
    {
        foreach (GameObject obj in pooledObjects)
        {
            if (!obj.activeInHierarchy && obj.CompareTag(tag))
            {
                return obj;
            }
        }

        foreach(var item in itemsToPool)
        {
            if (item.objectToPool.CompareTag(tag))
            {
                if(item.shouldExpand)
                {
                    return CreatePooledObject(item);
                }
            }
        }

        return null;
    }

    private GameObject CreatePooledObject(ObjectPoolItem item)
    {
        GameObject obj = Instantiate<GameObject>(item.objectToPool);

        // Get the parent for this pooled object and assign the new object to it
        var parentPoolObject = GetParentPoolObject(item.poolName);
        obj.transform.parent = parentPoolObject.transform;

        obj.SetActive(false);
        pooledObjects.Add(obj);
        return obj;
    }

    public bool ReturnToPool(GameObject obj)
    {
        foreach (var item in itemsToPool)
        {
            if (item.objectToPool.CompareTag(obj.tag))
            {                
                return true;
            }
        }
        return false;
    }

}
