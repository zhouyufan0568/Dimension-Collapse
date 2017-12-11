using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropBombController : MonoBehaviour {
    public GameObject explosion;
    public float speed = 20f;
	// Use this for initialization
	void Start () {
        GetComponent<Rigidbody>().velocity = transform.forward * speed;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnDestroy()
    {
        GameObject effect = Instantiate(explosion, transform.position, transform.rotation);
        Destroy(effect, 1.0f);
    }
}
