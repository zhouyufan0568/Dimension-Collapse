using UnityEngine;
using DimensionCollapse;

public class Pickup : MonoBehaviour {

    [SerializeField] private Transform mCenter;
    [SerializeField] private Transform mCamera;
    [SerializeField] private Transform mRightHand;
    private GameObject currentItem;
    private RaycastHit hit;
    private Inventory inventory;

    private void Start()
    {
        inventory = transform.GetComponent<Inventory>();
    }

    // Update is called once per frame
    void Update () {

		if (transform.position.y < -10f) {
			transform.position = new Vector3 (50,3,50);
		}

        Vector3 forward = mCamera.TransformDirection(Vector3.forward);
		if (Input.GetKeyDown(KeyCode.F))
        {
            if (Physics.Raycast(mCamera.position,forward,out hit, 5f))
            {
                
                Debug.DrawLine(mCamera.position, hit.point,Color.red);//划出射线，只有在scene视图中才能看到
                Vector3 forward2 = mCenter.TransformDirection(Vector3.forward);
                if (Physics.Raycast(mCenter.position,forward2, out hit, 7f)){
                    Debug.DrawLine(mCenter.position, hit.point);
                    currentItem = hit.collider.gameObject;

                    Item item = currentItem.GetComponent<Item>();
                    if (item!=null&&!item.Picked)
                    {
                        if(item is Weapon && mRightHand.childCount == 0)
                        {
                            EquipeWeapon(currentItem.gameObject);
                        }
                        else
                        {
                            if (inventory.AddItem(currentItem.gameObject))
                            {
                                item.Picked = true;
                                currentItem.SetActive(false);
                            }
                        }
                        
                    }
                }
            }
        }
		if (Input.GetKeyUp(KeyCode.G))
        {
            if (mRightHand.childCount > 0)
            {
                Debug.Log("throw");
                GameObject weapon = mRightHand.GetChild(0).gameObject;
                mRightHand.DetachChildren();
                weapon.GetComponent<Collider>().enabled = true;
                weapon.transform.Rotate(weapon.transform.up, -90, Space.World);
                Rigidbody weaponRigid = weapon.AddComponent<Rigidbody>();
                weaponRigid.useGravity = true;
                weaponRigid.isKinematic = false;
                weaponRigid.AddForce(forward.normalized * 100f);
                weapon.GetComponent<Weapon>().Picked = false;
                mRightHand.GetComponent<Attack>().weapon = null;

                GameObject newWeapon = inventory.GetNextWeapon();
                if (newWeapon != null&&mRightHand.childCount==0)
                {
                    Debug.Log("newWeapon");
                    newWeapon.SetActive(true);
                    EquipeWeapon(newWeapon);
                }
            }
        }
	}

    private void EquipeWeapon(GameObject weaponGO)
    {
        Destroy(weaponGO.GetComponent<Rigidbody>());
        weaponGO.GetComponent<Collider>().enabled = false;
        //Debug.Log("Remove rigid");
        weaponGO.transform.parent = mRightHand;
        weaponGO.transform.localPosition = Vector3.zero;
        weaponGO.transform.localEulerAngles = Vector3.zero;
        weaponGO.GetComponent<Item>().Picked = true;
        mRightHand.GetComponent<Attack>().weapon = weaponGO.GetComponent<Weapon>();
    }

}
