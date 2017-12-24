using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DimensionCollapse {
    public class PlayerManager : Photon.PunBehaviour, IPunObservable {

        #region Public Variables

        [Tooltip("The current health of our player")]
        public float Health = 1f;

        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject LocalPlayerInstance;

		[Tooltip("The current direction of our player")]
        public float Direction;

		[Tooltip("The current equipbar of our player")]
		public GameObject[] equipbar;

		[Tooltip("The current equipbar of our player")]
		public GameObject[] skillbar;

		[Tooltip("The current equipbar of our player")]
		public GameObject[] itembar;

        #endregion

		#region Private Variables

        private Health health;

		#endregion

		#region MonoBehaviour Messages

        void Awake() {
            // #Important
            // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
            if (photonView.isMine)
            {
                PlayerManager.LocalPlayerInstance = this.gameObject;
            }
        }

        // Use this for initialization
        void Start() {
			if (photonView.isMine) {
				GameObject.Find ("UIManager").SendMessage ("SetTarget", this, SendMessageOptions.RequireReceiver);
			}
            Direction = transform.rotation.eulerAngles.y;
            health = transform.GetComponent<Health>();
        }

        // Update is called once per frame
        void Update() {
            Direction = transform.rotation.eulerAngles.y;
            Health = health.health/health.maxHealth;
    
    	}

		#endregion

		#region IPunObservable implementation
		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			//throw new System.NotImplementedException ();
		}
		#endregion
    }
}
