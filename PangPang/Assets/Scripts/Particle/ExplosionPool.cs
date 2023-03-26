using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PangPang.Board;

namespace PangPang.Particle
{
    [System.Serializable]
    public class ObjectInfo
    {
        public Queue<GameObject> objectQueue = new Queue<GameObject>();
        public GameObject objectPrefab;
        public Transform parent;
        public string name;
        public int poolCount;
    }

    public class ExplosionPool : MonoBehaviour
    {
        public static ExplosionPool instance;

        private void Awake()
        {
            instance = this;
            Initialize();
        }

        [SerializeField]
        private List<ObjectInfo> objectsInfo = new List<ObjectInfo>();

        private void Initialize()
        {
            foreach (var objectInfo in objectsInfo)
                InsertQueue(objectInfo);
        }

        private void InsertQueue(ObjectInfo objectInfo)
        {
            for (int i = 0; i < objectInfo.poolCount; i++)
            {
                var newObj = Instantiate(objectInfo.objectPrefab);

                newObj.gameObject.SetActive(false);

                if (objectInfo.parent != null)
                    newObj.transform.SetParent(objectInfo.parent);
                else
                    newObj.transform.SetParent(transform);

                objectInfo.objectQueue.Enqueue(newObj);
            }
        }

        public GameObject GetObject(string _name)
        {
            foreach (var objectInfo in objectsInfo)
            {
                if (objectInfo.name == _name)
                {
                    if (objectInfo.objectQueue.Count > 0)
                    {
                        var obj = objectInfo.objectQueue.Dequeue();
                        obj.gameObject.SetActive(true);
                        return obj;
                    }
                    else
                    {
                        var newObj = Instantiate(objectInfo.objectPrefab);
                        newObj.gameObject.SetActive(true);
                        if (objectInfo.parent != null)
                            newObj.transform.SetParent(objectInfo.parent);
                        else
                            newObj.transform.SetParent(transform);
                        return newObj;
                    }
                }
            }

            Debug.Log("오브젝트를 찾을 수 없습니다.");
            return null;
        }

        public void ReturnObject(GameObject obj, string name)
        {
            foreach (var objectInfo in objectsInfo)
            {
                if (objectInfo.name == name)
                {
                    obj.gameObject.SetActive(false);
                    objectInfo.objectQueue.Enqueue(obj);
                }
            }
        }
    }
}