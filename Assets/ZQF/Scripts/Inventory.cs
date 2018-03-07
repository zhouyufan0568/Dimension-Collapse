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
        public GameObject[] Weapon = new GameObject[2];
		public GameObject[] Missile=new GameObject[1];
		public GameObject[] Remedy = new GameObject[2];
        public GameObject[] Skills = new GameObject[2];

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
                case ItemType.Backpack: {
                        ItemList.RemoveAt(index);
                        break;
                    }
                case ItemType.Equiped: {

                        break;
                    }
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
			if (item.GetComponent<Item> () is Weapon) {
				return PickEquiped(Weapon,item);
			} else if (item.GetComponent<Item> () is Skill) {
				return PickEquiped(Skills,item);
			} else if (item.GetComponent<Item> () is Missile) {
				return PickEquiped(Missile,item);
			} else {
				return AddItem (item);
			}
        }

        private bool PickEquiped(GameObject[] it,GameObject item){
			for (int i = 0; i < it.Length; i++) {
				if (it[i] == null) {
					it[i] = item;
					return true;
				}
			}
			return AddItem (item);
		}

        public void RemoveReference(GameObject item) {
            if (item.GetComponent<Item>() is Weapon)
            {
                for (int i = 0; i < Weapon.Length; i++) {
                    if (Weapon[i] == item) {
                        AutoEquipe<Weapon>(ref Weapon[i]);
                        return;
                    }
                }
            }
            else if (item.GetComponent<Item>() is Skill)
            {
                for (int i = 0; i < Skills.Length; i++)
                {
                    if (Skills[i] == item)
                    {
                        AutoEquipe<Skill>(ref Skills[i]);
                        return;
                    }
                }
            }
            else if (item.GetComponent<Item>() is Missile)
            {
                for (int i = 0; i < Missile.Length; i++)
                {
                    if (Missile[i] == item)
                    {
                        AutoEquipe<Missile>(ref Missile[i]);
                        return;
                    }
                }
            }
            else
            {
                Debug.Log("丢弃的东西是其他");
            }
            for (int i = 0; i < ItemList.Count; i++) {
                if (ItemList[i] == item)
                {
                    ItemList.RemoveAt(i);
                    return;
                }
            }
        }

        public void AutoEquipe<T>(ref GameObject item) {
            item = FindFirstInInventory<T>();
            if (item != null)
            {
                ItemList.Remove(item);
            }
        }

        private GameObject FindFirstInInventory<T>() {
            for (int i = 0; i < ItemList.Count; i++) {
                if (ItemList[i].GetComponent<Item>() is T) {
                    return ItemList[i];
                }
            }
            return null;
        }

        public void FindItemByTypeAndIndex(Inventory.ItemType type, int index,ref GameObject obj) {
            switch (type) {
                case ItemType.Equiped: {
                        switch (index)
                        {
                            case 0:
                            case 1:
                                {
                                    obj = Weapon[index];
                                    break;
                                }
                            case 2:
                                {
                                    obj = Missile[index - 2];
                                    break;
                                }
                            case 3:
                            case 4:
                                {
                                    obj = Remedy[index - 3];
                                    break;
                                }
                            default: { break; }
                        }
                        break;
                    }
                case ItemType.Skill: {
                        obj = Skills[index];
                        break;
                    }
                case ItemType.Backpack: {
                        if (index < ItemList.Count)
                        {
                            obj = ItemList[index];
                            break;
                        }
                        else {
                            obj = null;
                            break;
                        }
                    }
                default: { break; }
            }
        }

        public void SwapItem(Inventory.ItemType itemType, int itemindex, Inventory.ItemType toitemType, int toitemindex) {

            GameObject[] a = null;
            int aIndex=-1;
            GameObject[] b = null;
            int bIndex=-1;

            GetItemInfo(itemType, itemindex, ref a, ref aIndex);
            GetItemInfo(toitemType, toitemindex, ref b, ref bIndex);

            GameObject temp;

            if (a != null && b != null)
            { 
                temp = a[aIndex];
                a[aIndex] = b[bIndex];
                b[bIndex] = temp;
            }
            else if (a == null && b != null) {
                temp = ItemList[aIndex];
                ItemList[aIndex] = b[bIndex];
                b[bIndex] = temp;
            } else if (a != null && b == null) {
                temp = a[aIndex];
                a[aIndex] = ItemList[bIndex];
                ItemList[bIndex] = temp;
            } else {
                temp = ItemList[aIndex];
                ItemList[aIndex] = ItemList[bIndex];
                ItemList[bIndex] = temp;
            }

        }

        public void GetItemInfo(Inventory.ItemType itemType, int itemindex, ref GameObject[] a,ref int aIndex) {
            switch (itemType)
            {
                case ItemType.Equiped:
                    {
                        switch (itemindex)
                        {
                            case 0:
                            case 1:
                                {
                                    a = Weapon;
                                    aIndex = itemindex;
                                    break;
                                }
                            case 2:
                                {
                                    a = Missile;
                                    aIndex = itemindex - 2;
                                    break;
                                }
                            case 3:
                            case 4:
                                {
                                    a = Remedy;
                                    aIndex = itemindex - 3;
                                    break;
                                }
                            default: { break; }
                        }
                        break;
                    }
                case ItemType.Skill:
                    {
                        a = Skills;
                        aIndex = itemindex;
                        break;
                    }
                case ItemType.Backpack:
                    {
                        a = null;
                        aIndex = itemindex;
                        break;
                    }
            }
        }

        public void AddToInventory(GameObject item) {
            RemoveReference(item);
            if (AddItem(item))
            {

            }
            else
            {
                Debug.Log("空间不足");
            }
        }

        public void EquipeItem(ItemType toitemType, int toitemindex,GameObject item) {
            GameObject[] array = null;
            int index = -1;
            GetItemInfo(toitemType, toitemindex, ref array, ref index);
            if ((array == Weapon && item.GetComponent<Item>() is Weapon) || (array == Missile && item.GetComponent<Item>() is Missile) || (array == Remedy && item.GetComponent<Item>() is Remedy) || (array == Skills && item.GetComponent<Item>() is Skill)) {
                RemoveReference(item);
                array[index] = item;
            }
        }
    }
}
