using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System;

public class PlayerItem : MonoBehaviourPunCallbacks
{
	[SerializeField] private TextMeshProUGUI playerName;

	[SerializeField] private Image backgroundImage;
	[SerializeField] private Color highlightColor;
	[SerializeField] private GameObject leftArrow;
	[SerializeField] private GameObject rightArrow;

	ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();
	[SerializeField] private Image playerAvatar;
	[SerializeField] private Sprite[] avatars;

	private Player player;

	public void SetPlayerInfo( Player _player )
	{
		playerName.text = _player.NickName;
		player = _player;
		UpdatePlayerItem( player );
	}

	public void ApplyLocalChanges()
	{
		backgroundImage.color = highlightColor;
		leftArrow.SetActive( true );
		rightArrow.SetActive( true );
	}

	public void OnClickLeftArrow()
	{
		if( ( int )playerProperties["PlayerAvatar"] == 0 )
		{
			playerProperties["PlayerAvatar"] = avatars.Length - 1;
		}
		else
		{
			playerProperties["PlayerAvatar"] = ( int )playerProperties["PlayerAvatar"] - 1;
		}
		PhotonNetwork.SetPlayerCustomProperties( playerProperties );
	}

	public void OnClickRightArrow()
	{
		if( ( int )playerProperties["PlayerAvatar"] == avatars.Length - 1 )
		{
			playerProperties["PlayerAvatar"] = 0;
		}
		else
		{
			playerProperties["PlayerAvatar"] = ( int )playerProperties["PlayerAvatar"] + 1;
		}
		PhotonNetwork.SetPlayerCustomProperties( playerProperties );
	}

	public override void OnPlayerPropertiesUpdate( Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps )
	{
		if( player == targetPlayer )
		{
			UpdatePlayerItem( targetPlayer );
		}
	}

	private void UpdatePlayerItem( Player player )
	{
		if( player.CustomProperties.ContainsKey( "PlayerAvatar" ) )
		{
			playerAvatar.sprite = avatars[( int )player.CustomProperties["PlayerAvatar"]];
			playerProperties["PlayerAvatar"] = ( int )player.CustomProperties["PlayerAvatar"];
		}
		else
		{
			playerProperties["PlayerAvatar"] = 0;
		}
	}
}
