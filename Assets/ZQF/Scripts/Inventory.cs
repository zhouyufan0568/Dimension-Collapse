using System.Collections.Generic;
using UnityEngine;
using DimensionCollapse;

namespace DimensionCollapse
{
    public class Inventory : MonoBehaviour
    {

        private int weaponIndex = 0;
        public int maxCount;
        public List<GameObject> ItemList = new List<GameObject>();
        public GameObject[] EquipedWeapon = new GameObject[5];
        public Skill[] skills = new Skill[2];

        public enum ItemType
        {
            Backpack, Equiped, Skill
        }

        public bool AddItem(GameObject item)
        {
            Debug.Log("Inventory item(s):" + GetListCount());
            if (ItemList.Count >= maxCount)
            {
                return false;
            }
            ItemList.Add(item);
            return true;
        }

        public int RemoveItem(GameObject item)
        {
            if (ItemList.Contains(item))
            {
                return -1;
            }
            else
            {
                int pos = ItemList.IndexOf(item);
                ItemList.Remove(item);
                return pos;
            }
        }

        public int GetListCount()
        {
            return ItemList.Count;
        }

        public GameObject GetNextItem()
        {
            if (ItemList.Capacity > 0)
            {
                GameObject item = ItemList[0];
                ItemList.RemoveAt(0);
                return item;
            }

            return null;
        }

        public GameObject GetNextWeapon()
        {
            for (int i = 0; i < ItemList.Count; i++)
            {
                if (ItemList[i].GetComponent<Item>() is Weapon)
                {
                    GameObject result = ItemList[i];
                    ItemList.RemoveAt(i);
                    return result;
                }
            }
            return null;
        }
        /// <summary>
        /// 在背包UI界面中将物品移出背包时，调用函数将物品丢弃。
        /// </summary>
        /// <param name="it"></param>
        /// <param name="index"></param>
        public void RemoveItem(ItemType it, int index)
        {
            switch (it)
            {
                case ItemType.Backpack: { break; }
                case ItemType.Equiped: { break; }
                case ItemType.Skill: { break; }
                default: break;
            }
        }

        /// <summary>
        /// 将物品从一处移动到另一处。
        /// </summary>
        /// <param name="itfrom"></param>
        /// <param name="indexfrom"></param>
        /// <param name="itto"></param>
        /// <param name="indexto"></param>
        public void MoveItem(ItemType itfrom, int indexfrom, ItemType itto, int indexto)
        {
            //将物品从装备栏移动到背包内时，直接加到链表结尾。
            //将物品从背包内移动到装备栏时，如果目的装备栏有物品，则将其进行交换。
        }

        public bool PickItem(GameObject item) {
            if (item.GetComponent<Item>() is Weapon) {
                
            }
        }
    }
}
