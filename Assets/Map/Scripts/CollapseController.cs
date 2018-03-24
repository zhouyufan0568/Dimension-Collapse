using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CollapseController : MonoBehaviour {

	//地板数组
	private FloorUnit[] floor;

	//距离安全区出现时间
	[SerializeField]
	private float timeToReset = 60f;

	//距离坍塌时间
	[SerializeField]
	private float timeToCollapse=60f;

	//距离坍塌完毕时间
	[SerializeField]
	private float timeToCFinish=60f;

	//收缩比例
	[SerializeField]
	private float shrinkRatio=0.8f;

	//开始计时
	private float whenToStart;
	public static float elapsed;
	//计数
	private int cnt=0;

	//下落物体链表
	private LinkedList<FloorUnit> dropingCube;
	private Dictionary<GameObject, float> originY;

	// Use this for initialization
	void Start () {
		dropingCube = new LinkedList<FloorUnit> ();
		originY = new Dictionary<GameObject, float> ();
		floor = InitializeMap ();
		InitializeFallTime (floor);
		Array.Sort (floor);
		whenToStart = Time.time;
	}

	// Update is called once per frame
	void Update () {
		elapsed = Time.time - whenToStart;

		while (cnt<floor.Length && elapsed >= floor [cnt].timeToFall) {
			dropingCube.AddLast (floor[cnt]);
			LinkedListNode<GameObject> cur = floor[cnt].list.First;
			while (cur != null) {
				originY.Add (cur.Value, cur.Value.transform.position.y);
				Destroy (cur.Value.GetComponent<BoxCollider> ());
				cur = cur.Next;
			}
			cnt++;
		}

		LinkedListNode<FloorUnit> t = dropingCube.First;
		while (t != null) {
			LinkedListNode<GameObject> cur = t.Value.list.First;
			float time = elapsed - t.Value.timeToFall;
			bool hasSth = false;
			while (cur != null) {
				if (cur.Value != null) {
					hasSth = true;
					Vector3 position = cur.Value.transform.position;
					cur.Value.transform.position = new Vector3 (position.x, originY [cur.Value] - 0.5f * 9.81f * Mathf.Pow (time, 2), position.z);
					if (cur.Value.transform.position.y <= -30f) {
						originY.Remove (cur.Value);
						Destroy (cur.Value);
					}
				}
				cur = cur.Next;
			}
			if (hasSth) {
				t = t.Next;
			} else {
				LinkedListNode<FloorUnit> pre = t;
				t = t.Next;
				dropingCube.Remove (pre);
			}
		}

	}

	private void InitializeFallTime (FloorUnit[] fl){
        float firstx = 120 / 2;
        float firstz = 120 / 2;
		float firstr=Mathf.Sqrt ((Mathf.Pow (120 / 2, 2) + Mathf.Pow (120 / 2, 2)));
		Circle now = new Circle (firstx,firstz,firstr);
		Circle bef = null;
		float distance;
		float maxDistance;
		//直线与大圆的交点
		float fx,fz,x1,x2;
		float a, b, c, k;

        bool[] flags = new bool[fl.Length];

		while (now.r > 1) {

			bef = now;
            Vector2 cPos = new Vector2(60, 60);
			now = new Circle(cPos.x, cPos.y, bef.r * shrinkRatio);
			for (int i = 0; i < fl.Length; i++) {
				if(!flags[i]){
					float flx = fl [i].obj.transform.position.x;
					float flz = fl [i].obj.transform.position.z;
					distance = Mathf.Sqrt (Mathf.Pow ((flx - now.x), 2) + Mathf.Pow ((flz - now.z), 2))-now.r;
					if (distance > 0) {
                        flags[i] = true;
						k = (flz - now.z) / (flx - now.x);
						a = 1 + k * k;
						b = -2 * bef.x - 2 * k * k * now.x + 2 * k * (now.z - bef.z);
						c = -1 * bef.r * bef.r + bef.x * bef.x + Mathf.Pow ((-1 * now.x * k + now.z - bef.z), 2);
						x1= (-1 * b - Mathf.Sqrt (b * b - 4 * a * c)) / (2 * a);
						x2=(-1 * b + Mathf.Sqrt (b * b - 4 * a * c)) / (2 * a);
						if (flx < now.x) {
							fx = Mathf.Min (x1,x2);
						} else {
							fx = Mathf.Max (x1,x2);
						}
						fz = k * (fx - now.x) + now.z;
						if (float.IsNaN(fx) || float.IsNaN(fz)) {
							fx = flx;
							if (flz < now.z)
								fz = bef.z - Mathf.Sqrt (bef.r * bef.r - Mathf.Pow ((fx - bef.x), 2));
							else
								fz = bef.z + Mathf.Sqrt (bef.r * bef.r - Mathf.Pow ((fx - bef.x), 2));
						} 
						maxDistance = Mathf.Sqrt (Mathf.Pow ((fx - now.x), 2) + Mathf.Pow ((fz - now.z), 2)) - now.r;
						if (distance >= maxDistance || maxDistance <= 0) {
							fl [i].timeToFall += timeToReset + timeToCollapse;
						}
						else {
							fl [i].timeToFall += timeToReset + timeToCollapse + timeToCFinish * (maxDistance - distance) / maxDistance;
						}
					} 
					else {
						fl [i].timeToFall += timeToReset + timeToCollapse + timeToCFinish;
					}
                    //Debug.Log(fl[i].timeToFall);
				}
			}
			//重置时间
			timeToReset *= shrinkRatio;
			timeToCollapse *= shrinkRatio;
			timeToCFinish *= shrinkRatio;
		}
	}

	private FloorUnit[] InitializeMap(){
		LinkedList<Transform> flo=new LinkedList<Transform>();
		LinkedList<Transform> other=new LinkedList<Transform>();
		//FloorUnit[,] temp=new FloorUnit[FloorMaker.wnum,FloorMaker.dnum];
		GetAllChildren (this.gameObject.transform,flo, other);
        //Debug.Log(flo.Count);

        FloorUnit[,] cubeSpace = new FloorUnit[120, 120];
        int count = 0;
        foreach (var cube in flo)
        {
            int x = Mathf.FloorToInt(cube.position.x);
            int z = Mathf.FloorToInt(cube.position.z);
            if (cubeSpace[x, z] == null)
            {
                count++;
                FloorUnit unit = new FloorUnit();
                unit.obj = cube.gameObject;
                unit.list = new LinkedList<GameObject>();
                unit.list.AddFirst(new LinkedListNode<GameObject>(cube.gameObject));
                cubeSpace[x, z] = unit;
            }
        }
        foreach (var cube in other)
        {
            int x = Mathf.FloorToInt(cube.position.x);
            int z = Mathf.FloorToInt(cube.position.z);
            if (cubeSpace[x, z] != null)
            {
                cubeSpace[x, z].list.AddLast(new LinkedListNode<GameObject>(cube.gameObject));
            }
        }
        //Debug.Log(count);
		FloorUnit[] res = new FloorUnit[count];
        int index = 0;
        for (int i = 0; i < 120; i++)
        {
            for (int j = 0; j < 120; j++)
            {
                if (cubeSpace[i, j] != null)
                {
                    res[index++] = cubeSpace[i, j];
                }
            }
        }
        Debug.Log(index);

		//int x, z;

		//foreach (Transform t in flo) {
		//	x = Mathf.FloorToInt ((t.position.x-FloorMaker.offset)/FloorMaker.floorUnitSize);
		//	z = Mathf.FloorToInt ((t.position.z-FloorMaker.offset)/FloorMaker.floorUnitSize);
		//	if (temp [x, z] == null) {
		//		temp[x,z]=new FloorUnit{
		//			obj=t.gameObject,
		//			list=new LinkedList<GameObject>()
		//		};
		//		temp [x, z].list.AddLast (t.gameObject);
		//	}

		//}

		//foreach (Transform t in other) {
		//	x = Mathf.FloorToInt (t.position.x/FloorMaker.floorUnitSize);
		//	z = Mathf.FloorToInt (t.position.z/FloorMaker.floorUnitSize);
		//	temp [x, z].list.AddLast(t.gameObject);
		//}

		//int num = 0;
		//foreach (Transform t in flo) {
		//	x = Mathf.FloorToInt ((t.position.x-FloorMaker.offset)/FloorMaker.floorUnitSize);
		//	z = Mathf.FloorToInt ((t.position.z-FloorMaker.offset)/FloorMaker.floorUnitSize);
		//	res[num] = temp [x, z];
		//	num++;
		//}

		return res;
	}

	private void GetAllChildren(Transform trans,LinkedList<Transform> floor,LinkedList<Transform> other){
        for (int i = 0; i < trans.childCount; i++)
        {
            if (trans.GetChild(i).tag == "MapFloor")
            {
                Transform ftrans = trans.GetChild(i).transform;
                for (int j = 0; j < ftrans.childCount; j++)
                {
                    floor.AddLast(ftrans.GetChild(j));
                }
            }
            else
            {
                if (trans.GetChild(i).childCount != 0)
                {
                    GetAllChildren(trans.GetChild(i).transform, floor, other);
                }
                else
                {
                    other.AddLast(trans.GetChild(i));
                }
            }
        }
    }

	private class Circle{
		public float x;
		public float z;
		public float r;

		public Circle(float x,float z,float r){
			this.x=x;
			this.z=z;
			this.r=r;
		}
	}

	private class FloorUnit: IComparable<FloorUnit>{
		public GameObject obj;
		public float timeToFall=0;
		public LinkedList<GameObject> list;
		public int CompareTo(FloorUnit other)
		{
			return timeToFall.CompareTo(other.timeToFall);
		}
	}
}