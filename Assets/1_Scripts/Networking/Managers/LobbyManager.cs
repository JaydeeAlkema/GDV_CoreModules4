using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class LobbyManager : MonoBehaviourPunCallbacks
{
	public TMP_InputField roomInputfield;
	public GameObject lobbyPanel;
	public GameObject roomPanel;
	public TextMeshProUGUI roomName;
	[Space]
	public RoomItem roomItemPrefab;
	public List<RoomItem> roomItemsList = new List<RoomItem>();
	public Transform contentObject;
	[Space]
	public List<PlayerItem> playerItems = new List<PlayerItem>();
	public PlayerItem playerItemPrefab;
	public Transform playerItemTransformParent;
	[Space]
	public GameObject playButton;


	public float timeBetweenUpdates = 1.5f;
	private float nextUpdateTime;

	private void Update()
	{
		if( PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount >= 2 )
		{
			playButton.SetActive( true );
		}
		else
		{
			playButton.SetActive( false );
		}
	}

	public void OnClickPlayButton()
	{
		PhotonNetwork.LoadLevel( "Game" );
	}

	public void OnClickCreate()
	{
		if( roomInputfield.text.Length >= 1 )
		{
			PhotonNetwork.CreateRoom( roomInputfield.text, new RoomOptions() { MaxPlayers = 2, BroadcastPropsChangeToAll = true } );
		}
	}

	public override void OnJoinedRoom()
	{
		lobbyPanel.SetActive( false );
		roomPanel.SetActive( true );
		roomName.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name;
		UpdatePlayerList();
	}

	public override void OnRoomListUpdate( List<RoomInfo> roomList )
	{
		if( Time.time >= nextUpdateTime )
		{
			UpdateRoomList( roomList );
			nextUpdateTime = Time.time + timeBetweenUpdates;
		}
	}

	public override void OnPlayerEnteredRoom( Player newPlayer )
	{
		UpdatePlayerList();
	}

	public override void OnPlayerLeftRoom( Player otherPlayer )
	{
		UpdatePlayerList();
	}

	public override void OnLeftRoom()
	{
		roomPanel.SetActive( false );
		lobbyPanel.SetActive( true );
	}

	public override void OnConnectedToMaster()
	{
		PhotonNetwork.JoinLobby();
		UpdatePlayerList();
	}

	private void UpdateRoomList( List<RoomInfo> list )
	{
		foreach( RoomItem item in roomItemsList )
		{
			Destroy( item.gameObject );
		}
		roomItemsList.Clear();

		foreach( RoomInfo room in list )
		{
			RoomItem newRoom = Instantiate( roomItemPrefab, contentObject );
			newRoom.SetRoomName( room.Name );
			roomItemsList.Add( newRoom );
		}
	}

	private void UpdatePlayerList()
	{
		foreach( PlayerItem playerItem in playerItems )
		{
			Destroy( playerItem.gameObject );
		}
		playerItems.Clear();

		if( PhotonNetwork.CurrentRoom == null )
		{
			return;
		}

		foreach( KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players )
		{
			PlayerItem newPlayerItem = Instantiate( playerItemPrefab, playerItemTransformParent );
			newPlayerItem.SetPlayerInfo( player.Value );

			if( player.Value == PhotonNetwork.LocalPlayer )
			{
				newPlayerItem.ApplyLocalChanges();
			}

			playerItems.Add( newPlayerItem );
		}
	}

	public void JoinRoom( string roomName )
	{
		PhotonNetwork.JoinRoom( roomName );
	}

	public void OnClickLeaveRoom()
	{
		PhotonNetwork.LeaveRoom();
	}
}
