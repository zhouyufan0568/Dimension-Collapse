using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorMaker : MonoBehaviour {

	public GameObject gameobject;
	 
	public int length = 100;
	public int wide = 100;
	public int floorElementSize=4;

	public static int width = 0;  
	public static int depth = 0;  
	public static int floorUnitSize = 0;  

	//长和宽上cube数量
	public static int wnum=0;
	public static int dnum=0;
	//偏移量
	public static float offset=0;

	[SerializeField]  
	private bool _needToCollider = false;  

	void Awake(){
		width = length;
		depth = wide;
		floorUnitSize = floorElementSize;
		wnum = width / floorUnitSize;
		dnum = depth / floorUnitSize;
		offset = floorUnitSize / 2 - 0.5f;
		for (int x = 0; x < wnum; x++) {
			for (int z = 0; z < dnum; z++) {
				GameObject cube = Instantiate (gameobject);
				cube.transform.SetParent (transform); 
				cube.transform.localScale = new Vector3 (floorUnitSize, 1, floorUnitSize);
				cube.transform.localPosition = new Vector3 (floorUnitSize * x + offset, 0, floorUnitSize * z + offset);  
				if (!_needToCollider) {  
					Destroy (cube.GetComponent<BoxCollider> ());  
				}
			}
		}
	}
}
