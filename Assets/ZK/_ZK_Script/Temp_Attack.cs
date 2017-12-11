using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DimensionCollapse;
public class Temp_Attack : MonoBehaviour
{
    public RangedWeapon gun;
    public RangedWeapon gun2;

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

    }
}