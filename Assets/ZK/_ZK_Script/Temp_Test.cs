using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temp_Test : MonoBehaviour
{

    public GameObject target;
    private Vector3 orginal;
    // Use this for initialization
    void Start()
    {
        orginal = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            this.transform.position = target.transform.position;
            this.GetComponent<Rigidbody>().isKinematic = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            this.transform.position = orginal;
            this.GetComponent<Rigidbody>().isKinematic = true;
        }

    }
    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("It is from cube's OnCollisionEnter function");

    }
    void OnCollisionStay(Collision other)
    {
        Debug.Log("It is from cube's OnCollisionStay function");
    }
    private void OnCollisionExit(Collision other)
    {
        Debug.Log("It is from cube's OnCollisionExit function");
    }
}
