using System;
using System.Collections;
using UnityEngine.UI;

using UnityEngine;
using UnityEngine.SceneManagement;


namespace DimensionCollapse
{
    public class GameManager : Photon.PunBehaviour
    {


        static public GameManager Instance;

        [Tooltip("The prefab to use for representing the player")]
        public GameObject playerPrefab;

        [Tooltip("The prefab to use for representing dead state of the player")]
        public GameObject GhostPlayerfab;

        public MapDynamicLoading mapDynamicLoading;

        public GameObject HUDCanvas;

        public GameObject survivors;

        public GameObject GameOver;

        public GameObject deaders;

        public GameObject delayStart;

        public enum gameStates
        {
            Waiting,
            Gaming,
            GameOver
        }
        [HideInInspector]
        public gameStates currentState;

        private bool canStart = false;

        #region MonoBehaviour CallBacks

        private void Awake()
        {
            Instance = this;

            currentState = gameStates.Waiting;

            if (playerPrefab == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
            }
            else
            {
                Debug.Log("We are Instantiating LocalPlayer from " + Application.loadedLevelName);
                // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(50f, 50f, 50f), Quaternion.identity, 0);
                Debug.Log(this.playerPrefab.name);
            }
        }

        void Start()
        {
            
        }

        void Update()
        {
            if (currentState == gameStates.Waiting && survivors.transform.childCount >= 3 && canStart == false)
            {
                canStart = true;
                StartCoroutine(delayStartGame());
            }

            if (currentState == gameStates.Gaming && survivors.transform.childCount == 1 ) {
                currentState = gameStates.GameOver;
            }

            if (currentState == gameStates.GameOver&&(GameOver.GetActive()==false)) {
                if (PlayerManager.LocalPlayerInstance.GetComponent<PlayerManager>().isAlive)
                {
                    PlayerUI.Instance.GameOver.transform.Find("Result").GetComponent<Text>().text = "胜利";
                }
                else {
                    PlayerUI.Instance.GameOver.transform.Find("Result").GetComponent<Text>().text = "失败";
                }
                GameOver.SetActive(true);
                GameObject player = PlayerManager.LocalPlayerInstance;
                if (player.GetComponent<PlayerManager>().isAlive == true)
                {
                    player.transform.GetComponent<FirstViewCamera>().enabled = false;
                    player.transform.GetComponent<CharacterController>().enabled = false;
                    player.transform.GetComponent<Player>().enabled = false;
                }
                else {
                    player.transform.GetComponent<GhostMoveController>().enabled = false;
                }
                
                Transform camera = player.transform.Find("Center").Find("Camera");
                StartCoroutine("MoveCamera", camera);
            }

            //Debug.Log(currentState);
        }


        #endregion

        #region Photon Messages


        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene("LoginUI");
            //Debug.Log("SceneManager.LoadScene(0);");
        }


        #endregion


        #region Public Methods


        public void LeaveRoom()
        {
            //Debug.Log("PhotonNetwork.LeaveRoom();");
            PhotonNetwork.LeaveRoom();
        }

        public void WatchGame()
        {
            if (survivors.transform.childCount == 1) {
                GameOver.transform.GetChild(1).GetComponent<CanvasGroup>().interactable=false;
            }
            else
            {
                GameOver.SetActive(false);
            }
        }

        #endregion

        #region Private Methods

        IEnumerator MoveCamera(Transform camera) {
            Transform center = camera.parent;
            Vector3 centerOffsetRotation = (new Vector3(0, 0, 0) - center.localRotation.eulerAngles) / 2;
            Vector3 offsetPosition = (new Vector3(5, 0, 0) - camera.localPosition) / 2;
            Vector3 offsetRotation = (new Vector3(0, -90, 0) - camera.localRotation.eulerAngles) / 2;
            float time = 0;

            while (time < 2) {
                center.Rotate(centerOffsetRotation * Time.deltaTime);
                camera.localPosition += offsetPosition * Time.deltaTime;
                camera.Rotate(offsetRotation * Time.deltaTime);
                time += Time.deltaTime;
                yield return null;
            }
            camera.GetComponent<Animator>().enabled = true;
        }

        IEnumerator delayStartGame()
        {
            int time = 0;
            while (time < 5)
            {
                delayStart.transform.GetChild(time).gameObject.SetActive(true);
                if (time - 1 >= 0)
                {
                    delayStart.transform.GetChild(time - 1).gameObject.SetActive(false);
                }
                time++;
                yield return new WaitForSeconds(1);
            }

            delayStart.transform.GetChild(time - 1).gameObject.SetActive(false);

            //if (PhotonNetwork.isMasterClient)
            //{
            //    PhotonNetwork.room.IsVisible = false;
            //}

            if (PhotonNetwork.room.CustomProperties["StartTime"] == null)
            {
                PhotonNetwork.room.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "StartTime", PhotonNetwork.time } });
            }

            mapDynamicLoading.whenToStart = (float)(double)PhotonNetwork.room.CustomProperties["StartTime"];

            currentState = gameStates.Gaming;
        }


        #endregion

        #region Photon Messages


        public override void OnPhotonPlayerConnected(PhotonPlayer other)
        {
            Debug.Log("OnPhotonPlayerConnected() " + other.NickName); // not seen if you're the player connecting


            if (PhotonNetwork.isMasterClient)
            {
                Debug.Log("OnPhotonPlayerConnected isMasterClient " + PhotonNetwork.isMasterClient); // called before OnPhotonPlayerDisconnected

            }
        }


        public override void OnPhotonPlayerDisconnected(PhotonPlayer other)
        {
            Debug.Log("OnPhotonPlayerDisconnected() " + other.NickName); // seen when other disconnects


            if (PhotonNetwork.isMasterClient)
            {
                Debug.Log("OnPhotonPlayerDisonnected isMasterClient " + PhotonNetwork.isMasterClient); // called before OnPhotonPlayerDisconnected
            }
        }


        #endregion
    }
}