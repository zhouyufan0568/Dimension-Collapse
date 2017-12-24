using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temp_Animatior : MonoBehaviour {

private Animator animator;

	// Use this for initialization
	void Start () {
		animator = this.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetAxis("Vertical") != 0)
		{
			animator.SetBool("isRunning",true);

		}else
		{
			animator.SetBool("isRunning",false);
		}
	}
}
