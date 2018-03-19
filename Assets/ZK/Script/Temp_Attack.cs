using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DimensionCollapse;
public class Temp_Attack : MonoBehaviour
{
    public RangedWeapon gun;
    public RangedWeapon gun2;
    public RangedWeaponChanger gun3;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (gun != null)
            {
                gun.Attack();
            }
            if (gun2 != null)
            {
                gun2.Attack();
            }

        }
        if (Input.GetMouseButtonDown(0))
        {
            if (gun3 != null)
            {
                gun3.Attack(true);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (gun3 != null)
            {
                gun3.Attack(false);
            }
        }

        //临时代码
        if (Input.GetKeyDown(KeyCode.R))
        {

            if (gun != null)
            {
                gun.newReload();
            }
            if (gun2 != null)
            {
                gun2.newReload();
            }
        }
    }
    private void OnGUI()
    {
        if (gun != null)
        {
            GUILayout.TextArea(gun.CurrentChanger + "/" + gun.AlternativeCharger, 200);
        }
        if (gun2 != null)
        {
            GUILayout.TextArea(gun2.CurrentChanger + "/" + gun2.AlternativeCharger, 200);
        }
        if (gun3 != null)
        {
            GUILayout.TextArea(gun3.CurrentChanger + "/" + gun3.AlternativeCharger, 200);
        }

    }
}