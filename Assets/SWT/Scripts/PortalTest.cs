using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DimensionCollapse;

public class PortalTest : MonoBehaviour {

    private Portal portal;
	// Use this for initialization
	void Start () {
        portal = GetComponent<Portal>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.H))
        {
            portal.Cast();
        }
	}
}
