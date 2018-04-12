using UnityEngine;
using DimensionCollapse;

[RequireComponent(typeof(PlayerManager))]
public class PickupManager : MonoBehaviour
{
    [Tooltip("tools about camera")]
    [SerializeField] private Transform mCenter;
    [Tooltip("position the weapon will be")]
    [SerializeField] private Transform mWeaponPanel;
    [SerializeField] private Transform itemStorage;
    public PlayerManager playerManager;
    private GameObject items=null;
    private Inventory inventory;

    //to get the cursor position(in the center of the window)
    private Transform mCamera;
    //the item will be picked
    private GameObject currentItem;
    private RaycastHit hit;

    private void Start()
    {
        //playerManager = GetComponent<PlayerManager>();
        inventory = playerManager.GetComponent<Inventory>();
        mCamera = mCenter.Find("Camera");
    }

    public int PickItem()
    {
        Vector3 forward = mCamera.TransformDirection(Vector3.forward);
        if (Physics.Raycast(mCamera.position, forward, out hit, 5f))
        {
            Debug.DrawLine(mCamera.position, hit.point, Color.red);//划出射线，只有在scene视图中才能看到
            Vector3 forward2 = mCenter.TransformDirection(Vector3.forward);
            if (Physics.Raycast(mCenter.position, forward2, out hit, 7f))
            {
                Debug.DrawLine(mCenter.position, hit.point);
                currentItem = hit.collider.gameObject;

                Item item = currentItem.GetComponent<Item>();
                if (item != null && !item.Picked)
                {
                    PhotonView photonView = item.GetComponent<PhotonView>();
                    if (photonView != null)
                    {
                        return photonView.viewID;
                    }
                }
            }
        }
        return -1;
    }

    public void PickItemCore(int photonViewId)
    {
        PhotonView photonView = PhotonView.Find(photonViewId);
        if (photonView != null)
        {
            Item item = photonView.GetComponent<Item>();
            if (item == null)
            {
                return;
            }

			if (playerManager.inventory.PickItem(item.gameObject)){
                item.OnPickedUp(playerManager);
                item.transform.parent = itemStorage;
                item.transform.localPosition = Vector3.zero;
                item.transform.localEulerAngles = Vector3.zero;
                item.gameObject.SetActive(false);
			}

//            if ((item is Weapon || item is Missile) && mWeaponPanel.childCount == 0)
//            {
//                EquipeWeapon(item.gameObject);
//            }
//            else
//            {
//                if (playerManager.inventory.AddItem(item.gameObject))
//                {
//                    item.gameObject.SetActive(false);
//                }
//            }
        }
    }

    //public void DropHandItem()
    //{
    //    Vector3 forward = mCamera.TransformDirection(Vector3.forward);
    //    if (mWeaponPanel.childCount > 0)
    //    {
    //        //Debug.Log("throw");
    //        GameObject weapon = mWeaponPanel.GetChild(0).gameObject;
    //        mWeaponPanel.DetachChildren();
    //        playerManager.itemInHand = null;
    //        weapon.GetComponent<Item>().OnThrown();
    //        weapon.transform.Rotate(weapon.transform.up, -90, Space.World);
    //        weapon.GetComponent<Rigidbody>().AddForce(forward.normalized * 100f);

    //        GameObject newWeapon = playerManager.inventory.GetNextWeapon();
    //        if (newWeapon != null && mWeaponPanel.childCount == 0)
    //        {
    //            Debug.Log("newWeapon");
    //            newWeapon.SetActive(true);
    //            EquipeWeapon(newWeapon);
    //        }
    //    }
    //}

    public void DropHandItem()
    {
        Vector3 forward = mCamera.TransformDirection(Vector3.forward);
        if (playerManager.itemInHand!=null)
        {
            //Debug.Log("throw");
            GameObject weapon = playerManager.itemInHand.gameObject;
            if (items == null) {
                items = GameObject.Find("Items");
            }
            weapon.transform.parent=items.transform;
            playerManager.itemInHand = null;
            weapon.GetComponent<Item>().OnThrown();
            weapon.transform.Rotate(weapon.transform.up, -90, Space.World);
            weapon.GetComponent<Rigidbody>().AddForce(forward.normalized * 100f);
            playerManager.GetComponent<Inventory>().RemoveReference(weapon);
        }
    }

    public void EquipeWeapon(GameObject weaponGO)
    {
        weaponGO.transform.parent = mWeaponPanel;
        weaponGO.transform.localPosition = Vector3.zero;
        weaponGO.transform.localEulerAngles = Vector3.zero;

		playerManager.itemInHand = weaponGO.GetComponent<Item> ();
        playerManager.SetupIK();
    }

    public void EquipeWeaponByButton(int index)
    {
        bool equipe = false;
        if (playerManager.itemInHand != null)
        {
            playerManager.itemInHand.gameObject.SetActive(false);
            playerManager.itemInHand.transform.parent = itemStorage;
        }
        switch (index)
        {
            case 0:
            case 1:
                {
                    if (inventory.Weapon[index] != null)
                    {
                        equipe = true;
                        inventory.Weapon[index].SetActive(true);
                        EquipeWeapon(inventory.Weapon[index]);
                    }
                    break;
                }
            case 2:
                {
                    if (inventory.Missile[index - 2] != null)
                    {
                        equipe = true;
                        inventory.Missile[index - 2].SetActive(true);
                        EquipeWeapon(inventory.Missile[index - 2]);
                    }
                    break;
                }
            case 3:
            case 4:
                {
                    if (inventory.Remedy[index - 3] != null)
                    {
                        equipe = true;
                        inventory.Remedy[index - 3].SetActive(true);
                        EquipeWeapon(inventory.Remedy[index - 3]);
                    }
                    break;
                }
            default: { break; }
        }
        if (equipe == false)
        {
            if (playerManager.itemInHand != null)
            {
                playerManager.itemInHand.gameObject.SetActive(true);
                playerManager.itemInHand.transform.parent = mWeaponPanel;
            }
        }
    }

    public void DropItemByUI(Inventory.ItemType it, int index) {
        switch (it)
        {
            case Inventory.ItemType.Backpack:
                {
                    DropItem(inventory.ItemList[index]);
                    inventory.ItemList.RemoveAt(index);
                    break;
                }
            case Inventory.ItemType.Equiped:
                {
                    switch (index)
                    {
                        case 0:
                        case 1:
                            {
                                DropItem(inventory.Weapon[index]);
                                playerManager.GetComponent<Inventory>().RemoveReference(inventory.Weapon[index]);
                                break;
                            }
                        case 2:
                            {
                                DropItem(inventory.Missile[index-2]);
                                playerManager.GetComponent<Inventory>().RemoveReference(inventory.Missile[index - 2]);
                                break;
                            }
                        case 3:
                        case 4:
                            {
                                DropItem(inventory.Remedy[index-3]);
                                playerManager.GetComponent<Inventory>().RemoveReference(inventory.Remedy[index - 3]);
                                break;
                            }
                        default: { break; }
                    }
                    break;
                }
            case Inventory.ItemType.Skill: {
                    DropItem(inventory.Skills[index]);
                    playerManager.GetComponent<Inventory>().RemoveReference(inventory.Skills[index]);
                    break; }
            default: break;
        }
    }

    public void DropItem(GameObject item) {
        Vector3 forward = mCamera.TransformDirection(Vector3.forward);
        if (item != null)
        {
            if (items == null)
            {
                items = GameObject.Find("Items");
            }
            item.transform.parent = items.transform;
            item.GetComponent<Item>().OnThrown();
            item.SetActive(true);
            item.transform.Rotate(item.transform.up, -90, Space.World);
            item.GetComponent<Rigidbody>().AddForce(forward.normalized * 100f);
        }
    }

    public void SwapItemByUI(Inventory.ItemType itemType, int itemindex, Inventory.ItemType toitemType, int toitemindex) {
        GameObject from=null,to=null;
        inventory.FindItemByTypeAndIndex(itemType, itemindex,ref from);
        inventory.FindItemByTypeAndIndex(toitemType, toitemindex,ref to);
                    
        if (to != null) {
            if (IsSameType(from, to)||(itemType==Inventory.ItemType.Backpack && toitemType == Inventory.ItemType.Backpack)) {
                inventory.SwapItem(itemType, itemindex, toitemType, toitemindex);
            }
        } else {
            if (toitemType == Inventory.ItemType.Backpack)                                  
            {
                inventory.AddToInventory(from);
            }
            else {
                inventory.EquipeItem(toitemType, toitemindex, from);
            }
        }
    }

    private bool IsSameType(GameObject a, GameObject b) {
        Item ai = a.GetComponent<Item>();
        Item bi = b.GetComponent<Item>();
        if ((ai is Weapon && bi is Weapon) || (ai is Skill && bi is Skill) || (ai is Missile && bi is Missile)) {
            return true;
        }
        return false;
    }
}
