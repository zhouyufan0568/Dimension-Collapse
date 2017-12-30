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

        /// <summary>
        /// The main camera of this player.
        /// Add by SWT.
        /// </summary>
        [HideInInspector]
        public new Camera camera;

		public bool isAlive=true;

		public float maxHealth = 200;
		public float health=200;



        #endregion

		#region Private Variables

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

			isAlive = true;
            Direction = transform.rotation.eulerAngles.y;

            Camera[] cameras = gameObject.GetComponentsInChildren<Camera>();
            foreach (var cam in cameras)
            {
                if (cam.tag == "MainCamera")
                {
                    camera = cam;
                    break;
                }
            }
        }

        // Update is called once per frame
        void Update() {

			if (!photonView.isMine)
			{
				return;
			}
			if (health < 0)
			{
				health = 0;
				isAlive = false;
			}
			if (Input.GetKeyDown(KeyCode.E))
			{
				this.health = maxHealth;
				isAlive = true;
			}

            Direction = transform.rotation.eulerAngles.y;
            Health = health/maxHealth;
    	}

		#endregion

		public void OnAttacked(int primaryDamage, Vector3 contact)
		{
			if (!photonView.isMine)
			{
				return;
			}
			//Debug.Log(primaryDamage + "受到的伤害");
			this.health -= primaryDamage;
			//Debug.Log("Hash: " + this.gameObject.GetHashCode() + ", health is: " + health);
		}

		public void OnAttacked(int primaryDamage)
		{
			if (!photonView.isMine)
			{
				return;
			}
			//Debug.Log(primaryDamage + "受到的伤害");
			this.health -= primaryDamage;
		}

		#region Public Method

		#endregion

		#region IPunObservable implementation
		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (stream.isWriting)
			{
				stream.SendNext(health);
			}
			else
			{
				this.health = (float)stream.ReceiveNext();
			}
		}
		#endregion
    }
}
