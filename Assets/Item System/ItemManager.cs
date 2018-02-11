using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Linq;

using DimensionCollapse;

namespace DimensionCollapse
{
    public class ItemManager
    {
        public static readonly ItemManager INSTANCE;
        static ItemManager()
        {
            INSTANCE = new ItemManager();
        }

        private Dictionary<int, Item> idToItemPrefab;
        private System.Random random;
        private GameObject itemCollection;

        private ItemManager()
        {
            Item[] items = Resources.LoadAll<Item>("Items/Resources");
            StringBuilder builder = new StringBuilder("Item list:\n");
            foreach (var item in items)
            {
                builder.AppendFormat("ID: {0:0000}\tName: {1}\n", item.ID, item.Name);
            }
            Debug.Log(builder.ToString());

            idToItemPrefab = new Dictionary<int, Item>(items.Length);
            foreach (var item in items)
            {
                idToItemPrefab[item.ID] = item;
            }

            random = new System.Random();
        }

        public Item GetItemPrefabById(int id)
        {
            return idToItemPrefab.ContainsKey(id) ? idToItemPrefab[id] : null;
        }

        public string GetItemNameById(int id)
        {
            return GetItemPrefabById(id).name;
        }

        public int GetItemCount()
        {
            return idToItemPrefab.Keys.Count;
        }

        public Item GetRandomItemPrefab()
        {
            return idToItemPrefab.Values.ElementAt(random.Next(0, GetItemCount()));
        }

        public string GetRandomItemName()
        {
            return GetRandomItemPrefab().name;
        }

        public Item GetRandomItemPrefabNullable(float pForNull)
        {
            return Random.value < pForNull ? null : GetRandomItemPrefab();
        }

        public void ResetItemCollection()
        {
            if (itemCollection != null)
            {
                Object.Destroy(itemCollection);
            }

            itemCollection = new GameObject("Items");
        }

        public void AddIntoItemCollection(GameObject item)
        {
            item.transform.parent = itemCollection.transform;
        }
    }
}
