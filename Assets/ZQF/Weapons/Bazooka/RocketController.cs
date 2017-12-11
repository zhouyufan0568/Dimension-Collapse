using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketController : MonoBehaviour {
    public float speed = 150f;

    private bool exploded = false;

    public GameObject explosion;
	// Use this for initialization
	void Start () {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddForce(gameObject.transform.up * speed, ForceMode.Impulse);
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (!exploded)
        {
            exploded = true;
            GameObject effect = Instantiate(explosion, collision.transform.position, Quaternion.identity);
            Destroy(effect, 3.9f);
            Destroy(gameObject, 4f);
        }
    }
}
