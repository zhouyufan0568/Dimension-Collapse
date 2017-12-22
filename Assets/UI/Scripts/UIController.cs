using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour {

	public GameObject backpack;
	private Animator backpackAnimator;
	private CanvasGroup backpackCanvasGroup;

	// Use this for initialization
	void Start () {
		backpackAnimator = backpack.GetComponent<Animator> ();
		backpackCanvasGroup = backpack.GetComponent<CanvasGroup> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("Backpack")) {
			backpackAnimator.SetTrigger ("Open");
			backpackCanvasGroup.interactable=true;
			backpackCanvasGroup.blocksRaycasts = true;
		}
		if(Input.GetButtonUp("Backpack")){
			backpackAnimator.SetTrigger ("Close");
			backpackCanvasGroup.interactable=false;
			backpackCanvasGroup.blocksRaycasts = false;
		}
	}
}
