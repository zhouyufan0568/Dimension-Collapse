using System.Collections.Generic;
using UnityEngine;
using DimensionCollapse;

public class Inventory:MonoBehaviour{

    private int weaponIndex = 0;//current weapon
    public int maxCount = 5;
    private List<GameObject> ItemList = new List<GameObject>();
    private Weapon[] EquipedWeapon;
    private SkillItem[] m_SkillItems;

    public bool AddItem(GameObject item)
    {
        if (ItemList.Count>=maxCount)
        {
            return false;
        }
        ItemList.Add(item);
        return true;
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
