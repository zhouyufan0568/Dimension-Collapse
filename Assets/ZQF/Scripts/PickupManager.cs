using UnityEngine;
using DimensionCollapse;

[RequireComponent(typeof(PlayerManager))]
public class PickupManager : MonoBehaviour
{
    [Tooltip("tools about camera")]
    [SerializeField] private Transform mCenter;
    [Tooltip("position the weapon will be")]
    [SerializeField] private Transform mWeaponPanel;
    public PlayerManager playerManager;

    //to get the cursor position(in the center of the window)
    private Transform mCamera;
    //the item will be picked
    private GameObject currentItem;
    private RaycastHit hit;

    private void Start()
    {
        //playerManager = GetComponent<PlayerManager>();
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

            item.OnPickedUp(playerManager);

            if ((item is Weapon || item is Missile) && mWeaponPanel.childCount == 0)
            {
                EquipeWeapon(item.gameObject);
            }
            else
            {
                if (playerManager.inventory.AddItem(item.gameObject))
                {
                    item.gameObject.SetActive(false);
                }
            }
        }
    }

    public void DropHandItem()
    {
        Vector3 forward = mCamera.TransformDirection(Vector3.forward);
        if (mWeaponPanel.childCount > 0)
        {
            //Debug.Log("throw");
            GameObject weapon = mWeaponPanel.GetChild(0).gameObject;
            mWeaponPanel.DetachChildren();
            playerManager.itemInHand = null;
            weapon.GetComponent<Item>().OnThrown();
            weapon.transform.Rotate(weapon.transform.up, -90, Space.World);
            weapon.GetComponent<Rigidbody>().AddForce(forward.normalized * 100f);

            GameObject newWeapon = playerManager.inventory.GetNextWeapon();
            if (newWeapon != null && mWeaponPanel.childCount == 0)
            {
                Debug.Log("newWeapon");
                newWeapon.SetActive(true);
                EquipeWeapon(newWeapon);
            }
        }
    }

    public void EquipeWeapon(GameObject weaponGO)
    {
        weaponGO.transform.parent = mWeaponPanel;
        weaponGO.transform.localPosition = Vector3.zero;
        weaponGO.transform.localEulerAngles = Vector3.zero;

		playerManager.itemInHand = weaponGO.GetComponent<Item> ();
    }
}
