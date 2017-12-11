using UnityEngine;
using DimensionCollapse;

public class Attack : MonoBehaviour {

    public Weapon weapon;
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButton(0))
        {
            if (weapon != null)
            {
                weapon.Attack();
            }
        }
	}
}
