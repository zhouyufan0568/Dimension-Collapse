using System;
using System.Collections.Generic;
using UnityEngine;

namespace DimensionCollapse
{
    /// <summary>
    /// Provide service related to object pool.
    /// Best practise: when writing scripts for weapons that need a bullet pool, please follow these steps:
    /// First, register the bullet in Prefab List and Capacity List in inspector. 
    /// Then, get the single instance by ObjectPoolManager.INSTANCE.
    /// Afterwards, get an object pool by calling GetObjectPool(). If instantLoad equals true, then the pool can be used instantly, or you Must call FillPool() before usage.
    /// Finally, use RecyclePool() to recycle your object pool.
    /// </summary>
    public class ObjectPoolManager : MonoBehaviour
    {
        public static ObjectPoolManager INSTANCE
        {
            get
            {
                return instance;
            }
        }
        private static ObjectPoolManager instance;

        public List<GameObject> prefabList;
        public List<int> capacityList;
        private Dictionary<string, GameObject> prefabNameToPrefab;
        private Dictionary<string, Stack<GameObject>> prefabNameToPrefabStore;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(this);
                return;
            }
            instance = this;

            prefabNameToPrefab = new Dictionary<string, GameObject>(prefabList.Capacity);
            foreach (GameObject prefab in prefabList)
            {
                prefabNameToPrefab.Add(prefab.name, prefab);
            }

            prefabNameToPrefabStore = new Dictionary<string, Stack<GameObject>>(prefabList.Capacity);
            for (int i = 0; i < prefabList.Capacity; i++)
            {
                int capacity = i < capacityList.Capacity ? capacityList[i] : 0;
                GameObject prefab = prefabList[i];
                GameObject parent = new GameObject("Object Pool(" + prefab.name + " * " + capacity + ")");
                parent.transform.SetParent(this.transform);
                var stack = new Stack<GameObject>(capacity);
                while (capacity-- > 0)
                {
                    GameObject obj = Instantiate(prefab);
                    obj.SetActive(false);
                    obj.transform.SetParent(parent.transform);
                    stack.Push(obj);
                }
                prefabNameToPrefabStore.Add(prefabList[i].name, stack);
            }
        }

        public ObjectPool GetObjectPool(GameObject prefab, int capacity, bool instantLoad = true)
        {
            if (prefab == null || capacity <= 0)
            {
                Debug.Log("Error: invalid argument when calling GetObjectPool(). Will return null");
                return null;
            }

            if (!prefabNameToPrefab.ContainsKey(prefab.name))
            {
                return new ObjectPool(prefab, capacity, true);
            }

            ObjectPool pool = new ObjectPool(prefab, capacity, false);
            if (instantLoad)
            {
                FillPool(pool);
            }
            return pool;
        }

        public void FillPool(ObjectPool pool)
        {
            if (pool == null)
            {
                Debug.Log("Error: try to fill a null pool");
                return;
            }
            if (!pool.IsEmpty)
            {
                Debug.Log("Error: try to fill a pool which is already full.");
                return;
            }
            string name = pool.PrefabName;
            if (!prefabNameToPrefabStore.ContainsKey(name))
            {
                Debug.Log("Error: try to Fill a pool which requires the prefab that haven't registered.");
                return;
            }
            GameObject prefab = prefabNameToPrefab[name];

            Stack<GameObject> stack = prefabNameToPrefabStore[name];
            int bringFromStore = 0;
            int needToInstantiate = 0;
            if (stack.Count >= pool.Capacity)
            {
                bringFromStore = pool.Capacity;
            }
            else
            {
                Debug.Log("The number of objects in store can't meet the requirement of the pool, will instantiate " + needToInstantiate + " new object.");
                bringFromStore = stack.Count;
                needToInstantiate = pool.Capacity - bringFromStore;
            }
            GameObject[] objs = new GameObject[pool.Capacity];
            int index = 0;
            while (bringFromStore-- > 0)
            {
                GameObject obj = stack.Pop();
                obj.SetActive(false);
                objs[index++] = obj;
            }
            while (needToInstantiate-- > 0)
            {
                GameObject obj = Instantiate(prefab);
                obj.SetActive(false);
                objs[index++] = obj;
            }
            pool.LoadFrom(objs);
        }

        public void RecyclePool(ObjectPool pool)
        {
            if (pool == null)
            {
                Debug.Log("Error: try to recycle a null pool");
                return;
            }
            if (pool.IsEmpty)
            {
                Debug.Log("Error: try to recycle an empty pool.");
                return;
            }

            string name = pool.PrefabName;
            if (!prefabNameToPrefabStore.ContainsKey(name))
            {
                Debug.Log("Error: try to recycle a pool which contains prefab that haven't registered.");
                return;
            }

            Stack<GameObject> stack = prefabNameToPrefabStore[name];
            foreach (GameObject obj in pool.Empty())
            {
                stack.Push(obj);
            }
        }

        public class ObjectPool
        {
            private readonly TwinArray<GameObject> pool;
            private readonly GameObject prefab;
            public ObjectPool(GameObject prefab, int capacity, bool instantFill = false)
            {
                if (prefab == null || capacity <= 0)
                {
                    throw new ArgumentException("Invalid argument when creating ObjectPool.");
                }

                this.prefab = prefab;
                if (!instantFill)
                {
                    pool = new TwinArray<GameObject>(capacity);
                }
                else
                {
                    GameObject[] objs = new GameObject[capacity];
                    GameObject parent = new GameObject("Object Pool(" + prefab.name + " * " + capacity + ")");
                    for (int i = 0; i < capacity; i++)
                    {
                        GameObject obj = Instantiate(prefab);
                        obj.transform.SetParent(parent.transform);
                        obj.SetActive(false);
                        objs[i] = obj;
                    }
                    pool = new TwinArray<GameObject>(objs);
                }
            }

            public string PrefabName
            {
                get
                {
                    return prefab.name;
                }
            }

            public int Capacity
            {
                get
                {
                    return pool.Length;
                }
            }

            public bool IsEmpty
            {
                get
                {
                    return pool.IsEmpty;
                }
            }

            public GameObject Next(bool setActive)
            {
                if (pool.IsEmpty)
                {
                    Debug.Log("Error: try to obtain elements from an empty pool, will return null.");
                    return null;
                }
                GameObject obj = pool.NextElement();
                if (setActive)
                {
                    obj.SetActive(true);
                }
                return obj;
            }

            public GameObject[] Empty()
            {
                if (!pool.IsEmpty)
                {
                    return pool.Empty();
                }
                else
                {
                    Debug.Log("Error: try to empty an empty pool");
                    return new GameObject[0];
                }
            }

            public void LoadFrom(GameObject[] content)
            {
                if (pool.IsEmpty)
                {
                    if (pool.Length == content.Length)
                    {
                        pool.LoadFrom(content);
                    }
                    else
                    {
                        Debug.Log("Error: size of the new content don't match that of the pool.");
                    }
                }
                else
                {
                    Debug.Log("Error: try to fulfill the pool when it is not empty.");
                }
            }
        }

        private class TwinArray<T>
        {
            private readonly T[,] content;
            private int flag;
            private int index;
            private bool isEmpty;
            public TwinArray(T[] content)
            {
                flag = 0;
                index = 0;
                this.content = new T[2, content.Length];
                for (int i = 0; i < content.Length; i++)
                {
                    this.content[flag, i] = content[i];
                }
                isEmpty = false;
            }

            public TwinArray(int length)
            {
                content = new T[2, length];
                isEmpty = true;
            }

            public T NextElement()
            {
                T element = content[flag, index];
                content[1 - flag, index] = element;
                if (++index == Length)
                {
                    index = 0;
                    flag = 1 - flag;
                }
                return element;
            }

            public T[] Empty()
            {
                T[] objs = new T[Length];
                for (int i = index; i < Length; i++)
                {
                    objs[i] = content[flag, i];
                    content[flag, i] = default(T);
                }

                for (int i = 0; i < index; i++)
                {
                    objs[i] = content[1 - flag, i];
                    content[1 - flag, i] = default(T);
                }
                isEmpty = true;
                return objs;
            }

            public void LoadFrom(T[] content)
            {
                flag = 0;
                index = 0;
                for (int i = 0; i < content.Length; i++)
                {
                    this.content[flag, i] = content[i];
                }
                isEmpty = false;
            }

            public int Length
            {
                get
                {
                    return content.GetLength(1);
                }
            }

            public bool IsEmpty
            {
                get
                {
                    return isEmpty;
                }
            }
        }
    }
}
