using System.Collections.Generic;
using UnityEngine;

public class PickupAround : MonoBehaviour {

    private List<GameObject> items;//GameObject需要进一步进行封装
    
    public bool pickup;

	// Use this for initialization
	void Start () {
        items = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        items.Add(other.gameObject);
        Debug.Log(items.Count);
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("stayTrigger");
    }

    private void OnTriggerExit(Collider other)
    {
        items.Remove(other.gameObject);
        Debug.Log(items.Count);
    }

    private void OnCollisionEnter(Collision collision)
    {
        items.Add(collision.gameObject);
        Debug.Log(items.Count);
    }

    private void OnCollisionExit(Collision collision)
    {
        items.Remove(collision.gameObject);
        Debug.Log(items.Count);
    }

}
