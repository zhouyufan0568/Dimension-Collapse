using System;
using System.Collections;


using UnityEngine;
using UnityEngine.SceneManagement; 


namespace DimensionCollapse
{
	public class GameManager : Photon.PunBehaviour {


		static public GameManager Instance;

		[Tooltip("The prefab to use for representing the player")]
		public GameObject playerPrefab;

		#region MonoBehaviour CallBacks

		void Start()
		{
			Instance = this;

			if (playerPrefab == null) 
			{
				Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'",this);
			} 
			else
			{
				Debug.Log("We are Instantiating LocalPlayer from "+Application.loadedLevelName);
				// we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
				PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(50f,50f,50f), Quaternion.identity, 0);
			}
		}


		#endregion

		#region Photon Messages


		/// <summary>
		/// Called when the local player left the room. We need to load the launcher scene.
		/// </summary>
		public override void OnLeftRoom()
		{
			SceneManager.LoadScene(0);
		}


		#endregion


		#region Public Methods


		public void LeaveRoom()
		{
			PhotonNetwork.LeaveRoom();
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