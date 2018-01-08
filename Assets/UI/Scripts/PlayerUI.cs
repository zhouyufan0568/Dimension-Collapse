using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DimensionCollapse
{
	public class PlayerUI : Photon.PunBehaviour {

		#region Public Properties

		[Tooltip("UI Slider to display Player's Health")]
		public Slider PlayerHealthSlider;

		[Tooltip("UI raw image to display Player's Direction")]
		public RawImage PlayerDirectionSignal;

		[Tooltip("UI canvas to display Player's backpack")]
		public GameObject PlayerBackpackCanvas;

		[Tooltip("UI object shown Dynamically according to states of player")]
		public GameObject dynamicUI;

		[Tooltip("The number of people killed by player")]
		public Text Kill;

		[Tooltip("The number of survivors in current game")]
		public Text Alive;

		public GameObject survivors;
		public GameObject deaders;

        public GameObject circle_green;
        public GameObject circle_red;

        public GameObject gameObject_map;

        //states of map
        public enum mapStates
        {
            WaitforReset,
            WaitforCollapse,
            Collapse
        }

        //seconds of states lasting
        public LinkedList<Circles> list_circles;

        //seconds of states lasting
        [HideInInspector]
        public float SecondsOfWaitReset;
        [HideInInspector]
        public float SecondsOfWaitCollapse;
        [HideInInspector]
        public float SecondsOfCollapse;

        //time of states transition
        [HideInInspector]
        public float timeOfReset;
        [HideInInspector]
        public float timeOfCollapse;
        [HideInInspector]
        public float timeOfFinishCollapse;

        #endregion

        #region Private Properties

        private PlayerManager _target;

        private mapStates state;

        private float currentFPS;

        private int circleCnt = 0;

        private int eachChunckSize;

        //Circles of now and before
        private float[] nowf;
        private float[] beff;

        private float moveSpeed_center_x;
        private float moveSpeed_center_z;
        private float shrinkSpeed_radius;

        private MapDynamicLoading mapdynamicloading;
        private CombineMesh combineMesh;

        #endregion

        #region MonoBehaviour Messages

        void Awake()
		{
			
		}

		// Use this for initialization
		void Start () {
            state = mapStates.WaitforReset;
            combineMesh = gameObject_map.GetComponent<CombineMesh>();
            mapdynamicloading = gameObject_map.GetComponent<MapDynamicLoading>();
            eachChunckSize = combineMesh.ChunkSize;

            SecondsOfWaitReset = mapdynamicloading.originTimeToReset;
            timeOfReset = mapdynamicloading.originTimeToReset;
            SecondsOfWaitCollapse = mapdynamicloading.originTimeToCollapse;
            timeOfCollapse = mapdynamicloading.originTimeToReset + mapdynamicloading.originTimeToCollapse;
            SecondsOfCollapse = mapdynamicloading.originTimeToCFinish;
            timeOfFinishCollapse = mapdynamicloading.originTimeToReset + mapdynamicloading.originTimeToCollapse + mapdynamicloading.originTimeToCFinish;

            circle_green.SetActive(false);
            circle_red.SetActive(false);
        }

		// Update is called once per frame
		void Update () {

            currentFPS = 1 / Time.deltaTime;

            switch (state)
            {

                case (mapStates.WaitforReset):
                    {
                        if (mapdynamicloading.elapsed >= timeOfReset)
                        {
                            state = mapStates.WaitforCollapse;
                            SecondsOfWaitReset *= mapdynamicloading.shrinkRatio;
                            timeOfReset = timeOfFinishCollapse + SecondsOfWaitReset;

                            if (list_circles.Count != 0)
                            {
                                nowf = new float[] { list_circles.First.Value.circle_now[0] * eachChunckSize, list_circles.First.Value.circle_now[1] * eachChunckSize, list_circles.First.Value.circle_now[2] * eachChunckSize };
                                beff = new float[] { list_circles.First.Value.circle_bef[0] * eachChunckSize, list_circles.First.Value.circle_bef[1] * eachChunckSize, list_circles.First.Value.circle_bef[2] * eachChunckSize };
                                moveSpeed_center_x = (beff[0] - nowf[0]) / list_circles.First.Value.shrinkTime;
                                moveSpeed_center_z = (beff[1] - nowf[1]) / list_circles.First.Value.shrinkTime;
                                shrinkSpeed_radius = (beff[2] - nowf[2]) / list_circles.First.Value.shrinkTime;
                                list_circles.RemoveFirst();
                            }
                        }
                        break;
                    }
                case (mapStates.WaitforCollapse):
                    {
                        if (mapdynamicloading.elapsed >= timeOfCollapse)
                        {
                            state = mapStates.Collapse;
                            SecondsOfWaitCollapse *= mapdynamicloading.shrinkRatio;
                            timeOfCollapse = timeOfReset + SecondsOfWaitCollapse;
                        }
                        break;
                    }
                case (mapStates.Collapse):
                    {
                        if (mapdynamicloading.elapsed >= timeOfFinishCollapse)
                        {
                            state = mapStates.WaitforReset;
                            SecondsOfCollapse *= mapdynamicloading.shrinkRatio;
                            timeOfFinishCollapse = timeOfCollapse + SecondsOfCollapse;
                        }
                        break;
                    }
            }

            if (state == mapStates.Collapse) {
                beff[0] -= moveSpeed_center_x * Time.deltaTime;
                beff[1] -= moveSpeed_center_z * Time.deltaTime;
                beff[2] -= shrinkSpeed_radius * Time.deltaTime;
            }

            if (nowf!=null&&beff!=null) {
                if (!circle_green.activeSelf) {
                    circle_green.SetActive(true);
                }
                if (!circle_red.activeSelf)
                {
                    circle_red.SetActive(true);
                }
                circle_green.transform.position = new Vector3(nowf[0], 190, nowf[1]);
                circle_green.transform.localScale = new Vector3(nowf[2] * 2, nowf[2] * 2, 1);

                circle_red.transform.position = new Vector3(beff[0], 190, beff[1]);
                circle_red.transform.localScale = new Vector3(beff[2] * 2, beff[2] * 2, 1);
            }

            if (_target.isAlive == false&&dynamicUI.GetActive()==true) {
				dynamicUI.SetActive (false);
			}else if(_target.isAlive == true&&GameObject.Find ("DynamicUI")==false){
				dynamicUI.SetActive (true);
			}

			// Reflect the Player Health
			if (PlayerHealthSlider != null) 
			{
				PlayerHealthSlider.value = _target.Health;
			}

			if (PlayerDirectionSignal != null) {
				PlayerDirectionSignal.GetComponent<RawImage> ().uvRect = new Rect (_target.Direction/360+0.25f, 0, 0.5f, 1);
			}

			// Destroy itself if the target is null, It's a fail safe when Photon is destroying Instances of a Player over the network
			if (_target == null) 
			{
				Destroy(this.gameObject);
				return;
			}

			if (Kill != null) {
				Kill.text = _target.numOfkill.ToString();
			}

			if (Alive != null) {
				Alive.text = survivors.transform.childCount.ToString();
			}

		}

		#endregion

		#region Public Methods

		public void SetTarget(PlayerManager target)
		{
			if (target == null) 
			{
				Debug.LogError("<Color=Red><a>Missing</a></Color> PlayMakerManager target for PlayerUI.SetTarget.",this);
				return;
			}
			// Cache references for efficiency
			_target = target;
		}

        #endregion

        #region Private Methods

        private void OnGUI()
        {
            GUILayout.Label("FPS:" + currentFPS.ToString("f2"));
            GUILayout.Label("Ping:" + PhotonNetwork.GetPing());
            GUILayout.Label("当前时间：" + mapdynamicloading.elapsed);
            switch (state)
            {
                case (mapStates.WaitforReset):
                    {
                        GUILayout.Label("还有" + (timeOfReset - mapdynamicloading.elapsed) + "s重置安全区");
                        break;
                    }
                case (mapStates.WaitforCollapse):
                    {
                        GUILayout.Label("还有" + (timeOfCollapse - mapdynamicloading.elapsed) + "s开始塌陷");
                        break;
                    }
                case (mapStates.Collapse):
                    {
                        GUILayout.Label("还有" + (timeOfFinishCollapse - mapdynamicloading.elapsed) + "s停止塌陷");
                        break;
                    }
            }
        }

        #endregion
    }

} 
