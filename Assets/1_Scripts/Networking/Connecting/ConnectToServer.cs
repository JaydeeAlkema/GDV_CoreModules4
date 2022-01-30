using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
	private void Start()
	{
		PhotonNetwork.ConnectUsingSettings();
		PhotonNetwork.AutomaticallySyncScene = true;
	}

	public override void OnConnectedToMaster()
	{
		SceneManager.LoadScene( "Lobby" );
	}
}
