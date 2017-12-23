using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BringToFirst : MonoBehaviour {

	void Start () {
        transform.SetAsLastSibling();	
	}
}
