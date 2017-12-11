using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGunBulletController : MonoBehaviour {
    public float speed = 50f;

	// Use this for initialization
	void Start () {
        GetComponent<Rigidbody>().velocity = transform.forward * speed;	
	}
	
}
