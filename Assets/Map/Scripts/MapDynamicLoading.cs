using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace DimensionCollapse{
	public class MapDynamicLoading : Photon.PunBehaviour {

        public PlayerUI playerUI;

		[SerializeField]
		private int visibleScope=300;

		private int eachChunkSize;
		private int lengthOfMap;
		private int widthOfMap;
		private int numOfChunk;

		//----------------------------安全区-------------------------------------

		//收缩比例
		public float shrinkRatio=0.8f;

		//地板数组
		private FloorUnit[] floor;

		//距离安全区出现时间
		public float timeToReset = 60f;

		//距离坍塌时间
		public float timeToCollapse=60f;

		//距离坍塌完毕时间
		public float timeToCFinish=60f;

		//开始计时
		private float whenToStart;

		[HideInInspector]
		public float elapsed;

		//计数
		private int cnt=0;

		//下落物体链表
		//private LinkedList<FloorUnit> dropingCube;
		//private Dictionary<GameObject, float> originY;

		private static float[,] timeToFall;

        //初始距离安全区出现时间
        [HideInInspector]
        public float originTimeToReset;

        //初始距离坍塌时间
        [HideInInspector]
        public float originTimeToCollapse;

        //初始距离坍塌完毕时间
        [HideInInspector]
        public float originTimeToCFinish;

        //----------------------------动态加载-----------------------------------

        //private GameObject[] player;
        private GameObject[,] mapChunk;
		private int[,] flag;


		private int maxX;
		private int maxZ;
		private int visibleChunk;

		//上一帧人物的坐标
		private int lastX;
		private int lastZ;

		//当前人物坐标
		private int nowX;
		private int nowZ;

		private ObjectPool mapChunkPool;

		private CombineMesh combineMesh;
		private GameObject temp;

		public static GameObject mine;

        void Awake(){
            originTimeToReset = timeToReset;
            originTimeToCollapse = timeToCollapse;
            originTimeToCFinish = timeToCFinish;
		}

		// Use this for initialization
		void Start () {
			combineMesh = GetComponent<CombineMesh>();

            numOfChunk = combineMesh.ChunkCount;
            maxX = combineMesh.Chunk_X_Max;
            maxZ = combineMesh.Chunk_Z_Max;
            eachChunkSize = combineMesh.ChunkSize;
            visibleChunk = visibleScope / eachChunkSize;
            lengthOfMap = maxX * eachChunkSize;
            widthOfMap = maxZ * eachChunkSize;

            //----------------------------安全区-------------------------------------

            timeToFall =new float[maxX,maxZ];
			floor = InitializeMap ();
			InitializeFallTime (floor);
			Array.Sort (floor);
			if (PhotonNetwork.isMasterClient) {
				PhotonNetwork.room.SetCustomProperties (new ExitGames.Client.Photon.Hashtable (){ { "StartTime",PhotonNetwork.time } });
			}
			whenToStart = (float)(double)PhotonNetwork.room.CustomProperties["StartTime"];

			//----------------------------动态加载-----------------------------------

			//player = GameObject.FindGameObjectsWithTag ("Player");
//			foreach (GameObject p in player) {
//				if (p.GetComponent<PhotonView> ().isMine) {
//					mine = p;
//					break;
//				}
//			}
			mine=PlayerManager.LocalPlayerInstance;

			mapChunk=new GameObject[maxX,maxZ];
			flag = new int[maxX, maxZ];

			GetObjectPosition (mine,out nowX,out nowZ);
			elapsed = (float)PhotonNetwork.time - whenToStart;
			CreateChunk (nowX,nowZ);
			lastX = nowX;
			lastZ = nowZ;
		}

		// Update is called once per frame
		void Update () {

			//----------------------------安全区-------------------------------------

			elapsed = (float)PhotonNetwork.time - whenToStart;
			//Debug.Log (current+elapsed);

			while (cnt<floor.Length && elapsed >= timeToFall[floor[cnt].x,floor[cnt].z]) {
				if (mapChunk [floor [cnt].x, floor [cnt].z] != null) {
					Destroy (mapChunk [floor [cnt].x, floor [cnt].z].GetComponent<Collider> ());
					mapChunk [floor [cnt].x, floor [cnt].z].AddComponent<Rigidbody> ();
					Destroy (mapChunk [floor [cnt].x, floor [cnt].z],5);
				}
				cnt++;
			}

			//----------------------------动态加载-----------------------------------

			if (mine == null) {
				mine=PlayerManager.LocalPlayerInstance;
			}

			GetObjectPosition (mine,out nowX,out nowZ);

			if (nowX == lastX && nowZ == lastZ) {
				return;
			} 
			else {
				int diffx = nowX - lastX;
				int diffz = nowZ - lastZ;

				if (Mathf.Abs (diffx) <= 1 && Mathf.Abs (diffz) <= 1) {
					MoveChunk (nowX,nowZ,diffx,diffz);
				} 
				else {															//此时判定位置异常，重新生成player附近chunk
					Debug.Log("execute");
					DestroyChunk (lastX,lastZ);
					CreateChunk (nowX,nowZ);
				}

				lastX  = nowX;
				lastZ  = nowZ;
			}


			//		}
		}

		//params:nowx,nowz,differences between now and last position of player
		void MoveChunk(int x,int z,int difx,int difz){

			if (difx!=0) {
				int needDestroyX = x - difx * (visibleChunk + 1);
				int needCreateX = x + difx * visibleChunk;

				int lastz = z - difz;

				int start = (lastz - visibleChunk) > 0 ? (lastz - visibleChunk) : 0;
				int end = (lastz + visibleChunk + 1 < maxZ) ? (lastz + visibleChunk + 1) : maxZ;

				StartCoroutine (MoveChunkByFrame(start,end,needDestroyX,needCreateX,true));

			}

			if (difz!=0) {
				int needDestroyZ = z - difz * (visibleChunk + 1);
				int needCreateZ = z + difz * visibleChunk;

				int start = (x - visibleChunk) > 0 ? (x - visibleChunk) : 0;
				int end = (x + visibleChunk + 1 < maxX) ? (x + visibleChunk + 1) : maxX;

				StartCoroutine (MoveChunkByFrame(start,end,needDestroyZ,needCreateZ,false));

			}


		}

		IEnumerator MoveChunkByFrame(int start,int end,int needDestroy,int needCreate,bool isX){

			if (isX == true) {
				for (int i = start; i < end; i++) {

					if (needDestroy >= 0 && needDestroy < maxX) {
						flag [needDestroy, i]--;
						if (flag [needDestroy, i] == 0) {
							if (elapsed < timeToFall [needDestroy, i]) {
								combineMesh.RestoreChunkByXZ (needDestroy, i, mapChunk [needDestroy, i]);
								mapChunk [needDestroy, i] = null;
							}
						}
					}

					if (needCreate >= 0 && needCreate < maxX) {
						flag [needCreate, i]++;
						if (flag [needCreate, i] == 1) {
							mapChunk [needCreate, i] = GetMapChunk (needCreate, i, out temp);
						}
					}
					yield return null;
				}
			} else {
				for (int i = start; i < end; i++) {
					if (needDestroy >= 0 && needDestroy < maxZ) {
						flag [i, needDestroy]--;
						if (flag [i, needDestroy] == 0) {
							if (elapsed < timeToFall [i, needDestroy]) {
								combineMesh.RestoreChunkByXZ (i, needDestroy, mapChunk [i, needDestroy]);
								mapChunk [i, needDestroy] = null;
							}
						}
					}

					if (needCreate >= 0 && needCreate < maxZ) {
						flag [i, needCreate]++;
						if (flag [i, needCreate] == 1) {
							mapChunk [i, needCreate] = GetMapChunk (i, needCreate, out temp);
						}
					}
					yield return null;
				}
			}
		}

		private class ObjectPool
		{
			private Stack<GameObject> m_objectStack = new Stack<GameObject>();
			private GameObject map = GameObject.FindGameObjectWithTag ("Map");

			public GameObject New()
			{
				GameObject t;

				if (m_objectStack.Count == 0){
					t = new GameObject ();
					t.transform.SetParent (map.transform);
				}
				else{
					t = m_objectStack.Pop();
				}

				t.SetActive (true);

				return t;
			}

			public void Store(GameObject t)
			{
				t.SetActive (false);
				Destroy (t.GetComponent<Rigidbody> ());
				m_objectStack.Push(t);
			}
		}

		private void CreateChunk(int x,int z){

			int pminx = x - visibleChunk;
			int pmaxx = x + visibleChunk;
			int pminz = z - visibleChunk;
			int pmaxz = z + visibleChunk;

			int start = pminx > 0 ? pminx : 0;
			int end = (pmaxx + 1) < maxX ? (pmaxx + 1) : maxX;

			int cstart = pminz > 0 ? pminz : 0;
			int cend = (pmaxz + 1) < maxZ ? (pmaxz + 1) : maxZ;

			for (int i = start; i < end; i++) {
				for (int j = cstart; j < cend; j++) {

					flag [i, j]++;

					if (flag [i,j] == 1) {
						mapChunk [i, j] = GetMapChunk (i,j,out temp);
					}

				}
			}
		}

		private void DestroyChunk(int x,int z){

			int pminx = x - visibleChunk;
			int pmaxx = x + visibleChunk;
			int pminz = z - visibleChunk;
			int pmaxz = z + visibleChunk;

			int start = pminx > 0 ? pminx : 0;
			int end = (pmaxx + 1) < maxX ? (pmaxx + 1) : maxX;

			int cstart = pminz > 0 ? pminz : 0;
			int cend = (pmaxz + 1) < maxZ ? (pmaxz + 1) : maxZ;

			for (int i = start; i < end; i++) {
				for (int j = cstart; j < cend; j++) {

					flag [i, j]--;

					if (flag [i,j] == 0) {
						combineMesh.RestoreChunkByXZ (i,j,mapChunk [i, j]);
						//mapChunkPool.Store (mapChunk [i, j]);
						mapChunk [i, j] = null;
					}

				}
			}
		}

		private void GetObjectPosition(GameObject obj,out int posX,out int posZ){
			posX=Mathf.FloorToInt(obj.transform.position.x / eachChunkSize+0.5f);//四舍五入
			posZ=Mathf.FloorToInt(obj.transform.position.z / eachChunkSize+0.5f);
		}

		private FloorUnit[] InitializeMap(){

			FloorUnit[] res = new FloorUnit[numOfChunk];

			int cnt = 0;
			for (int i = 0; i < maxX; i++) {
				for (int j = 0; j < maxZ; j++) {
					if (combineMesh.IsChunkExists (i, j)) {
						res [cnt] = new FloorUnit ();
						res [cnt].x = i;
						res [cnt].z = j;
						cnt++;
					}
				}
			}

			return res;
		}

		private class FloorUnit: IComparable<FloorUnit>{

			public int x,z;
			public bool isSureDropOrNot=false;

			public int CompareTo(FloorUnit other)
			{
				return timeToFall[x,z].CompareTo(timeToFall[other.x,other.z]);
			}
		}

		private void InitializeFallTime (FloorUnit[] fl){

			if (PhotonNetwork.isMasterClient) {
				RandomCircle ();
			}

			float distance;
			float maxDistance;
			//直线与大圆的交点
			float fx,fz,x1,x2;
			float a, b, c, k;
			int cnt = 0;

			Circle bef;
			Circle now;
			float[] nowf = (float[])PhotonNetwork.room.CustomProperties ["Circle" + cnt++];
			now = new Circle (nowf[0],nowf[1],nowf[2]);

			while (PhotonNetwork.room.CustomProperties["Circle"+cnt]!=null) {

				bef = now;
				nowf = (float[])PhotonNetwork.room.CustomProperties ["Circle" + cnt++];
				now = new Circle (nowf[0],nowf[1],nowf[2]);

				for (int i = 0; i < fl.Length; i++) {
					if(fl[i].isSureDropOrNot==false){
						int flx = fl [i].x;
						int flz = fl [i].z;
						distance = Mathf.Sqrt (Mathf.Pow ((flx - now.x), 2) + Mathf.Pow ((flz - now.z), 2))-now.r;
						if (distance > 0) {
							fl [i].isSureDropOrNot = true;
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
								//Debug.Log ("distance >= maxDistance || maxDistance <= 0");
								timeToFall[flx,flz] += timeToReset + timeToCollapse;
							}
							else {
								timeToFall[flx,flz] += timeToReset + timeToCollapse + timeToCFinish * (maxDistance - distance) / maxDistance;
							}
						} 
						else {
							timeToFall[flx,flz] += timeToReset + timeToCollapse + timeToCFinish;
						}
						//Debug.Log ("fl"+"["+i+"]:"+fl[i].timeToFall);
					}
				}

                if (playerUI.list_circles == null) {
                    playerUI.list_circles = new LinkedList<Circles>();
                }
                playerUI.list_circles.AddLast(new Circles(new float[] { now.x, now.z, now.r }, new float[] { bef.x, bef.z, bef.r }, timeToCFinish));
                Debug.Log(playerUI.list_circles.Last.Value.circle_now[0] + ","+ playerUI.list_circles.Last.Value.circle_now[1] + ","+ playerUI.list_circles.Last.Value.circle_now[2] + " " + playerUI.list_circles.Last.Value.circle_bef[0] + ","+ playerUI.list_circles.Last.Value.circle_bef[1] + ","+ playerUI.list_circles.Last.Value.circle_bef[2]+ " " + playerUI.list_circles.Last.Value.shrinkTime);

                //重置时间
                timeToReset *= shrinkRatio;
				timeToCollapse *= shrinkRatio;
				timeToCFinish *= shrinkRatio;
			}
		}

		private void RandomCircle(){

			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable ();

			int cnt = 0;
			float firstx=maxX / 2;
			float firstz=maxZ / 2;
			float firstr=Mathf.Sqrt ((Mathf.Pow (firstx, 2) + Mathf.Pow (firstz, 2)));
			Circle now = new Circle (firstx,firstz,firstr);
			Circle bef = null;
			hashtable.Add ("Circle"+cnt++,new float[3]{now.x,now.z,now.r});

			while (now.r > 1) {
				bef = now;
				Vector2 cPos = UnityEngine.Random.insideUnitCircle * (bef.r * (1 - shrinkRatio)) + new Vector2(bef.x, bef.z);
				now = new Circle(cPos.x, cPos.y, bef.r * shrinkRatio);
				hashtable.Add ("Circle"+cnt++,new float[3]{now.x,now.z,now.r});
			}
            //hashtable.Add("NumOfCircle", cnt);
			//set room customproperties
			PhotonNetwork.room.SetCustomProperties (hashtable,null,false);
		}

		public class Circle{
			public float x;
			public float z;
			public float r;

			public Circle(float x,float z,float r){
				this.x=x;
				this.z=z;
				this.r=r;
			}
		}

		private GameObject GetMapChunk(int x,int z,out GameObject obj){
			obj = null;
			if (elapsed < timeToFall [x, z]) {
				combineMesh.GetChunkByXZ (x,z,out obj);
			}
			if (obj != null) {
				obj.transform.SetParent (transform);
			}
			return obj;
		}
	}
}
