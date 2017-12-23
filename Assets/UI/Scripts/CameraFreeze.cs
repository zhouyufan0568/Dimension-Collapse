using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DimensionCollapse{
	public class CameraFreeze : MonoBehaviour {

		public GameObject miniMapCameraFreeze;
		public bool isFreeze=false;

		private Transform player;
		private GameObject cameraNotFreeze;

		void Start(){
			
		}

		public void Init(){
			if (MapDynamicLoading.mine != null) {
				player = MapDynamicLoading.mine.transform;
				cameraNotFreeze = player.Find ("MiniMapCamera").gameObject;
			}
		}

		void Update(){
			if (isFreeze) {
				miniMapCameraFreeze.transform.position = player.position + new Vector3 (0, 50, 0);
			}
		}

		public void FreezeRotationOrNot(){
			if (!isFreeze) {
				Debug.Log("Lock execute!");
				isFreeze = true;
				miniMapCameraFreeze.SetActive (true);
				cameraNotFreeze.SetActive (false);
			} else {
				Debug.Log("Unlock execute!");
				isFreeze = false;
				miniMapCameraFreeze.SetActive (false);
				cameraNotFreeze.SetActive (true);
			}
		}
	}
}
