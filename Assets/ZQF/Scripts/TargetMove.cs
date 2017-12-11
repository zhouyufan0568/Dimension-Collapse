using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMove : MonoBehaviour {

    private float mTime = 0;
    public float TurnTime;
    private Vector3 CurrentForward;

	// Use this for initialization
	void Start () {
        CurrentForward = transform.right.normalized;
	}
	
	// Update is called once per frame
	void Update () {
        if (mTime < TurnTime)
        {
            transform.Translate(CurrentForward*3f*Time.deltaTime);
            mTime += Time.deltaTime;
        }
        else
        {
            mTime = 0;
            CurrentForward = -CurrentForward;
        }
	}
}
