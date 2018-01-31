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

    // Update is called once per frame
    //void Update()
    //{
        
    //    if (Input.GetKeyDown(KeyCode.E))
    //    {
    //        PickItem();
    //    }
    //    if (Input.GetKeyUp(KeyCode.Q))
    //    {
    //        DropHandItem();
    //    }
    //}

    public void PickItem()
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
                    if (item is Weapon && mWeaponPanel.childCount == 0)
                    {
                        EquipeWeapon(currentItem.gameObject);
                    }
                    else
                    {
                        if (playerManager.inventory.AddItem(currentItem.gameObject))
                        {
                            ///item.Picked = true;
                            currentItem.SetActive(false);
                        }
                    }
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
            ///weapon.GetComponent<Collider>().enabled = true;
            weapon.transform.Rotate(weapon.transform.up, -90, Space.World);
            ///Rigidbody weaponRigid = weapon.AddComponent<Rigidbody>();
            ///weaponRigid.useGravity = true;
            ///weaponRigid.isKinematic = false;
            weapon.GetComponent<Rigidbody>().AddForce(forward.normalized * 100f);
            ///weapon.GetComponent<Weapon>().Picked = false;
            //Commented by SWT.
            //Shoot脚本已被删除。功能移到RPCManager中了。
            //mWeaponPanel.GetComponent<Shoot>().weapon = null;

            GameObject newWeapon = playerManager.inventory.GetNextWeapon();
            if (newWeapon != null && mWeaponPanel.childCount == 0)
            {
                Debug.Log("newWeapon");
                newWeapon.SetActive(true);
                EquipeWeapon(newWeapon);
            }
        }
    }

    private void EquipeWeapon(GameObject weaponGO)
    {
        ///Destroy(weaponGO.GetComponent<Rigidbody>());
        ///weaponGO.GetComponent<Collider>().enabled = false;
        //Debug.Log("Remove rigid");
        weaponGO.GetComponent<Item>().OnPickedUp(playerManager);

        weaponGO.transform.parent = mWeaponPanel;
        weaponGO.transform.localPosition = Vector3.zero;
        weaponGO.transform.localEulerAngles = Vector3.zero;
        ///weaponGO.GetComponent<Item>().Picked = true;

        playerManager.itemInHand = weaponGO.GetComponent<Weapon>();

        //Commented by SWT.
        //Shoot脚本已被删除。功能移到RPCManager中了。
        //mWeaponPanel.GetComponent<Shoot>().weapon = weaponGO.GetComponent<Weapon>();
    }

}
