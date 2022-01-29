using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
	[SerializeField] private TMP_InputField createInputfield;
	[SerializeField] private TMP_InputField joinInputfield;

	public void CreateRoom()
	{
		PhotonNetwork.CreateRoom( createInputfield.text );
	}

	public void JoinRoom()
	{
		PhotonNetwork.JoinRoom( joinInputfield.text );
	}

	public override void OnJoinedRoom()
	{
		PhotonNetwork.LoadLevel( "Game" );
	}
}
