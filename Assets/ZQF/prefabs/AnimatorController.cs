using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DimensionCollapse{
	public class AnimatorController : MonoBehaviour {
		private Animator animator;
		// Use this for initialization
		void Start () {
			animator = GetComponent<Animator> ();
		}

		// Update is called once per frame
		void Update () {
			if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D)) {
				animator.SetBool ("iswalk", true);
			}
			if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D)) {
				animator.SetBool ("iswalk", false);
			}
		}
	}
}
