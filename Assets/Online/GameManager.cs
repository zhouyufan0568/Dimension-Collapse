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

        public enum gameStates
        {
            Waiting,
            Gaming,
            GameOver
        }
        [HideInInspector]
        public gameStates currentState;

        #region MonoBehaviour CallBacks

        void Start()
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

        void Update()
        {
            if (currentState == gameStates.Waiting && PhotonNetwork.room.PlayerCount >= 2)
            {

                if (PhotonNetwork.isMasterClient)
                {
                    PhotonNetwork.room.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "StartTime", PhotonNetwork.time } });
                }
                mapDynamicLoading.whenToStart = (float)(double)PhotonNetwork.room.CustomProperties["StartTime"];

                PhotonNetwork.room.IsVisible = false;
                currentState = gameStates.Gaming;
            }

            if (currentState == gameStates.Gaming && survivors.transform.childCount == 1 && deaders.transform.childCount!=0 ) {
                currentState = gameStates.GameOver;
            }

            if (currentState == gameStates.GameOver&&(GameOver.GetActive()==false)) {
                if (PlayerManager.LocalPlayerInstance.GetComponent<PlayerManager>().isAlive) {
                    PlayerUI.Instance.GameOver.transform.Find("Result").GetComponent<Text>().text = "胜利";
                }
                GameOver.SetActive(true);
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