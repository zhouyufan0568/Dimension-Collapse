using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour {

	public GameObject[] gobject;

	// Use this for initialization
	void Start () {
		Debug.Log(Noise (50,60));
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	double Noise(int x,int y)    // 根据(x,y)获取一个初步噪声值  
	{  
		int n = x + y * 57;    
		n = (n<<13) ^ n;  
		return ( 1.0 - ( (n * (n * n * 15731 + 789221) + 1376312589) & 0x7fffffff) / 1073741824.0);  
	}  
}
