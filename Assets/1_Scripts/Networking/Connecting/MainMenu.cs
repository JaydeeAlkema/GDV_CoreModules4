using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class MainMenu : MonoBehaviourPunCallbacks
{
	[SerializeField] private TMP_InputField roomNameInputfield;

	public void CreateRoom()
	{
		RoomOptions roomOptions = new RoomOptions();
		roomOptions.MaxPlayers = 2;
		PhotonNetwork.CreateRoom( roomNameInputfield.text, roomOptions );
	}

	public void JoinRoom()
	{
		PhotonNetwork.JoinRoom( roomNameInputfield.text );
	}

	public override void OnJoinedRoom()
	{
		PhotonNetwork.LoadLevel( "Game" );
	}
}
