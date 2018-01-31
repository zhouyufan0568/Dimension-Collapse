using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DimensionCollapse {
    public class PlayerManager : Photon.PunBehaviour, IPunObservable {

        #region Public Variables

        [Tooltip("The current health of our player")]
        public float Health = 1f;

        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject LocalPlayerInstance;

        public static Camera LocalPlayerMainCamera;

		[Tooltip("The current direction of our player")]
        public float Direction;

		[Tooltip("The current equipbar of our player")]
		public GameObject[] equipbar;

		[Tooltip("The current equipbar of our player")]
		public GameObject[] skillbar;

		[Tooltip("The current equipbar of our player")]
		public GameObject[] itembar;

		[Tooltip("The prefab to use for representing the player")]
		public GameObject playerPrefab;

		[Tooltip("The prefab to use for representing dead state of the player")]
		public GameObject GhostPlayerfab;

        /// <summary>
        /// The main camera of this player.
        /// Add by SWT.
        /// </summary>
        [HideInInspector]
        public new Camera camera;

		public bool isAlive;

		public float maxHealth = 200;
		public float health;

		public int numOfkill=0;
        //背包
        public Inventory inventory;

		public GameObject[] backpack;
		public GameObject[] equipBar;
		public GameObject[] skills;

        /// <summary>
        /// The item in player's hands.
        /// Add by SWT.
        /// </summary>
        public Item itemInHand;

        /// <summary>
        /// The skills being used.
        /// </summary>
        public Skill skillOne;
        public Skill skillTwo;

        #endregion

		#region Private Variables

		private GameObject survivors;
		private GameObject deaders;

		#endregion

		#region MonoBehaviour Messages

        void Awake() {
            
            Camera[] cameras = gameObject.GetComponentsInChildren<Camera>();
            foreach (var cam in cameras)
            {
                if (cam.tag == "MainCamera")
                {
                    camera = cam;
                    break;
                }
            }

            inventory = GetComponent<Inventory>();

            // #Important
            // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
            if (photonView.isMine)
            {
                PlayerManager.LocalPlayerInstance = this.gameObject;
                PlayerManager.LocalPlayerMainCamera = camera;
            }

        }

        // Use this for initialization
        void Start() {

			survivors = GameObject.Find ("Survivors");
			deaders = GameObject.Find ("Deaders");

			if (photonView.isMine) {
				GameObject.Find ("UIManager").SendMessage ("SetTarget", this, SendMessageOptions.RequireReceiver);
			}

			if (isAlive) {
				gameObject.transform.SetParent (survivors.transform);
			} else {
				gameObject.transform.SetParent (deaders.transform);
			}
				
            Direction = transform.rotation.eulerAngles.y;

        }

        // Update is called once per frame
        void Update() {

			if (!photonView.isMine)
			{
				return;
			}
			if (health < 0)
			{
				DeadDecision ();
			}
			if (Input.GetKeyDown(KeyCode.E))
			{
				if(isAlive==false){Revive ();}
			}

            Direction = transform.rotation.eulerAngles.y;
            Health = health/maxHealth;
    	}

		#endregion

		#region Public Method
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

		#region Private Method
		void DeadDecision(){
			PhotonNetwork.Instantiate (this.GhostPlayerfab.name, transform.position, transform.rotation, 0);
			PhotonNetwork.Destroy (this.gameObject);
            if (photonView.isMine&&GameManager.Instance.currentState==GameManager.gameStates.Gaming) {
                PlayerUI.Instance.GameOver.transform.Find("Result").GetComponent<Text>().text="失败";
                PlayerUI.Instance.GameOver.SetActive(true);
            }
		}

		void Revive(){
			PhotonNetwork.Instantiate (this.playerPrefab.name, transform.position, new Quaternion(), 0);
			PhotonNetwork.Destroy (this.gameObject);
		}
		#endregion
    }
}
