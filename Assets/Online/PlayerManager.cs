using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DimensionCollapse
{
    public class PlayerManager : Photon.PunBehaviour, IPunObservable
    {

        #region Public Variables

        [Tooltip("The current health of our player")]
        public float Health = 1f;

        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject LocalPlayerInstance;

        public static Camera LocalPlayerMainCamera;

        [Tooltip("The current direction of our player")]
        public float Direction;

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

        public float maxChargeTime = 3f;

        public float maxChargeForce = 50f;

        public bool isAlive;

        public float maxHealth = 200;
        public float health;

        public int numOfkill = 0;
        //背包
        [ReadOnlyInInspector]
        public Inventory inventory;
        [ReadOnlyInInspector]
        public PickupManager pickupManager;

        /// <summary>
        /// The item in player's hands.
        /// Add by SWT.
        /// </summary>
        public Item itemInHand;

        public GameObject itemStore;

        /// <summary>
        /// The skills being used.
        /// </summary>
        public Skill skillOne;
        public Skill skillTwo;

        public static Dictionary<GameObject, PlayerManager> playerToPlayerManager;
        static PlayerManager()
        {
            playerToPlayerManager = new Dictionary<GameObject, PlayerManager>();
        }

        #endregion

        #region Private Variables

        private GameObject survivors;
        private GameObject deaders;
        private ImpactReceiver impactReceiver;
        private IKManager ikManager;

        #endregion

        #region MonoBehaviour Messages

        void Awake()
        {
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

            pickupManager = GetComponent<PickupManager>();

            impactReceiver = GetComponent<ImpactReceiver>();

            ikManager = GetComponent<IKManager>();

            // #Important
            // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
            if (photonView.isMine)
            {
                PlayerManager.LocalPlayerInstance = this.gameObject;
                PlayerManager.LocalPlayerMainCamera = camera;
            }

            playerToPlayerManager.Add(gameObject, this);
        }

        // Use this for initialization
        void Start()
        {

            survivors = GameObject.Find("Survivors");
            deaders = GameObject.Find("Deaders");

            if (photonView.isMine)
            {
                GameObject.Find("UIManager").SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
            }

            if (isAlive)
            {
                gameObject.transform.SetParent(survivors.transform);
            }
            else
            {
                gameObject.transform.SetParent(deaders.transform);
            }

            Direction = transform.rotation.eulerAngles.y;

        }

        // Update is called once per frame
        void Update()
        {
            if (itemInHand == null && ikManager != null)
            {
                ikManager.DisableIK();
            }

            if (!photonView.isMine)
            {
                return;
            }

            if (health < 0)
            {
                DeadDecision();
            }

            if (transform.position.y < -25)
            {
                if (isAlive)
                {
                    isAlive = false;
                    DeadDecision();
                }

            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (isAlive == false) { Revive(); }
            }

            Direction = transform.rotation.eulerAngles.y;
            Health = health / maxHealth;
        }

        #endregion

        #region Public Method
        public void OnAttacked(float primaryDamage, Vector3 contact)
        {
            if (!photonView.isMine)
            {
                return;
            }
            //Debug.Log(primaryDamage + "受到的伤害");
            this.health -= primaryDamage;
            BloodUI.Instance.displayBlood();
            //Debug.Log("Hash: " + this.gameObject.GetHashCode() + ", health is: " + health);
        }

        public void OnAttacked(float primaryDamage)
        {
            CountVisualizeManager.INSTANCE.ShowDamageCount(primaryDamage, transform);
            if (!photonView.isMine)
            {
                return;
            }
            //Debug.Log(primaryDamage + "受到的伤害");
            BloodUI.Instance.displayBlood();
            this.health -= primaryDamage;
        }

        /// <summary>
        /// 恢复生命值
        /// </summary>
        /// <param name="healing"></param>
        public void OnHeal(float healing)
        {
            OnHeal(healing, 0f, 0f);
        }

        /// <summary>
        /// 持续恢复生命值
        /// </summary>
        /// <param name="healing">总治愈量</param>
        /// <param name="duration">持续时间</param>
        /// <param name="interval">治愈间隔</param>
        public void OnHeal(float healing, float duration, float interval)
        {
            if (Mathf.Approximately(duration, 0f) || Mathf.Approximately(interval, 0f))
            {
                CountVisualizeManager.INSTANCE.ShowHealingCount(healing, transform);
            }

            if (!photonView.isMine)
            {
                return;
            }

            if (Mathf.Approximately(duration, 0f) || Mathf.Approximately(interval, 0f))
            {
                this.health += healing;
            }
            else
            {
                StartCoroutine(HealCoroutine(healing / duration * interval, interval, Mathf.FloorToInt(duration / interval)));
            }
        }

        private IEnumerator HealCoroutine(float healingPerInterval, float interval, int count)
        {
            while (count-- > 0)
            {
                this.health += healingPerInterval;
                CountVisualizeManager.INSTANCE.ShowHealingCount(healingPerInterval, transform);
                yield return new WaitForSeconds(interval);
            }
        }

        /// <summary>
        /// 施加冲击力
        /// </summary>
        /// <param name="force">冲击力</param>
        public void AddImpact(Vector3 force)
        {
            if (photonView.isMine)
            {
                impactReceiver.AddImpact(force);
            }
        }

        /// <summary>
        /// 施加冲击力
        /// </summary>
        /// <param name="direction">冲击力方向</param>
        /// <param name="magnitude">冲击力大小</param>
        public void AddImpact(Vector3 direction, float magnitude)
        {
            if (photonView.isMine)
            {
                impactReceiver.AddImpact(direction, magnitude);
            }
        }

        public void SwitchItemInHand()
        {
            GameObject next = inventory.GetNextItem();
            if (next != null)
            {
                if (itemInHand != null)
                {
                    itemInHand.gameObject.SetActive(false);
                    inventory.AddItem(itemInHand.gameObject);
                }
                next.SetActive(true);
                pickupManager.EquipeWeapon(next);
            }
        }

        public void SetupIK()
        {
            ikManager.ChangeIKObjs();
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
        void DeadDecision()
        {
            DropItemOnDead();
            PhotonNetwork.Instantiate(this.GhostPlayerfab.name, transform.position, transform.rotation, 0);
            PhotonNetwork.Destroy(this.gameObject);

            if (photonView.isMine && GameManager.Instance.currentState == GameManager.gameStates.Gaming)
            {
                PlayerUI.Instance.GameOver.transform.Find("Result").GetComponent<Text>().text = "失败";
                PlayerUI.Instance.GameOver.SetActive(true);
            }
        }

        void Revive()
        {
            PhotonNetwork.Instantiate(this.playerPrefab.name, transform.position, new Quaternion(), 0);
            PhotonNetwork.Destroy(this.gameObject);
        }

        void DropItemOnDead()
        {
            if (itemStore) {
                Debug.Log(itemStore.transform.childCount);
                for (int i = itemStore.transform.childCount-1 ; i >= 0; i--) {
                    pickupManager.DropItem(itemStore.transform.GetChild(i).gameObject);
                }
            }
            if (itemInHand) {
                Debug.Log("exe");
                pickupManager.DropItem(itemInHand.gameObject);
            }
        }

        #endregion
    }
}
