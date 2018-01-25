using System.Collections.Generic;
using UnityEngine;
using DimensionCollapse;

public class Inventory:MonoBehaviour{

    private int weaponIndex = 0;
    public int maxCount;
    private List<GameObject> ItemList = new List<GameObject>();
    private Weapon[] EquipedWeapon;

    public bool AddItem(GameObject item)
    {
        if (ItemList.Count>=maxCount)
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
    public int GetInventoryCapacity()
    {
        return ItemList.Count;
    }
    public GameObject GetNextWeapon()
    {
        for(int i=0;i<ItemList.Count;i++)
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
	
}
