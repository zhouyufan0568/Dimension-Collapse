using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGunBulletSpawner : MonoBehaviour {
    public GameObject bullet;
    public float bulletLifetime = 3f;
    public float interval = 0.25f;

    private float nextShot;
	// Use this for initialization
	void Start () {
        nextShot = float.MinValue;
	}
	
	// Update is called once per frame
	void Update () {
        if (Time.time >= nextShot && Input.GetButton("Fire1"))
        {
            nextShot = Time.time + interval;
            GameObject bulletInstance = Instantiate(bullet, transform.position, transform.rotation);
            Destroy(bulletInstance, bulletLifetime);
        }
	}
}
