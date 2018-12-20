using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectPoolItem
{
    public GameObject objectToPool;
    public int amountToPool;
    public bool shouldExpand = true;
}

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler sharedInstance;

    public List<ObjectPoolItem> itemsToPool;
    public List<GameObject> pooledObjects;
   

    private void Awake()
    {
        sharedInstance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        pooledObjects = new List<GameObject>();

        foreach(var item in itemsToPool)
        {
            for (int i = 0; i < item.amountToPool; i++)
            {
                GameObject obj = Instantiate(item.objectToPool);
                obj.SetActive(false);

                pooledObjects.Add(obj);
            }
        }        
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
                    GameObject obj = Instantiate(item.objectToPool);
                    obj.SetActive(false);
                    pooledObjects.Add(obj);

                    return obj;
                }
            }
        }

        return null;
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
