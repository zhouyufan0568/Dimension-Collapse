using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropGunBulletSpawner : MonoBehaviour {
    public GameObject bullet;
    public float lifetime = 5f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Fire1"))
        {
            GameObject oneBullet = Instantiate(bullet, transform.position, transform.rotation);
            Destroy(oneBullet, lifetime);
        }
	}
}
