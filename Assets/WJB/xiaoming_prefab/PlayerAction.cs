using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour {
    //玩家水平 垂直方向偏移量
    public float horizon;
    public float vertical;
    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    void Move(float horizon, float vetical) {
        transform.Translate(new Vector3(0, 1, 1));
    }
}
