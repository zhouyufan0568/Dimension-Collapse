using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DimensionCollapse{
	public class MiniMapIconContraller : MonoBehaviour {

		private Transform player;

		// Use this for initialization
		void Start () {

		}

		// Update is called once per frame
		void Update () {

			if (player == null) {
				player = PlayerManager.LocalPlayerInstance.transform;
			}

			this.transform.position = player.position + new Vector3 (0, 50, 0);
			Vector3 playerQuaternion = player.rotation.eulerAngles;
			this.transform.eulerAngles = new Vector3 (90, playerQuaternion.y, playerQuaternion.z);
		}
	}

}
