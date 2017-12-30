using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DimensionCollapse{
	public class CameraFreeze : MonoBehaviour {

		public GameObject miniMapCameraFreeze;
		public bool isFreeze=false;

		private Transform player;

		void Start(){
			
		}
			
		void Update(){

			if (player == null) {
				player = PlayerManager.LocalPlayerInstance.transform;
			}

			if(Input.GetButtonDown("LockMinimap")){
				FreezeRotationOrNot();
			}

			if (isFreeze) {
				miniMapCameraFreeze.transform.position = player.position + new Vector3 (0, 200, 0);
			} else {
				miniMapCameraFreeze.transform.position = player.position + new Vector3 (0, 200, 0);
				Vector3 playerQuaternion = player.rotation.eulerAngles;
				miniMapCameraFreeze.transform.eulerAngles = new Vector3(90,playerQuaternion.y,playerQuaternion.z);
			}
		}

		public void FreezeRotationOrNot(){
			if (!isFreeze) {
				Debug.Log("Lock execute!");
				isFreeze = true;
				miniMapCameraFreeze.transform.eulerAngles = new Vector3(90,0,0);
			} else {
				Debug.Log("Unlock execute!");
				isFreeze = false;
			}
		}
	}
}