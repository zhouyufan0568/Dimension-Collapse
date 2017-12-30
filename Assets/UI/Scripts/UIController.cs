using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour {

	public GameObject backpack;
	//private Animator backpackAnimator;
	//private CanvasGroup backpackCanvasGroup;

	// Use this for initialization
	void Start () {
		//backpackAnimator = backpack.GetComponent<Animator> ();
		//backpackCanvasGroup = backpack.GetComponent<CanvasGroup> ();
		backpack.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("Backpack")) {
			backpack.SetActive(true);
		}
		if(Input.GetButtonUp("Backpack")){
			backpack.SetActive(false);
		}
	}
}
