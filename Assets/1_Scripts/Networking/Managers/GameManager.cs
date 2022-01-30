using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
{
	[SerializeField] private List<PlayerController> players = new List<PlayerController>();
	[Space]
	[SerializeField] private GameObject gameOver;

	private void Update()
	{
		while( players.Count < 2 )
		{
			PlayerController[] _players = FindObjectsOfType<PlayerController>();

			foreach( PlayerController player in _players )
			{
				if( !players.Contains( player ) )
				{
					players.Add( player );
				}
			}
		}
	}
}
