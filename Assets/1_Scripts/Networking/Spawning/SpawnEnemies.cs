using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnEnemies : MonoBehaviour
{
	[SerializeField] private Transform[] spawnPoints;
	[SerializeField] private GameObject enemyPrefab;
	[SerializeField] private float startTimeBetweenSpawns;
	private float timeBetweenSpawns;

	private void Start()
	{
		timeBetweenSpawns = startTimeBetweenSpawns;
	}

	private void Update()
	{
		if( PhotonNetwork.IsMasterClient == false || PhotonNetwork.CurrentRoom.PlayerCount != 2 )
		{
			return;
		}

		if( timeBetweenSpawns <= 0 )
		{
			Vector3 spawnPosition = spawnPoints[Random.Range( 0, spawnPoints.Length )].position;
			PhotonNetwork.Instantiate( enemyPrefab.name, spawnPosition, Quaternion.identity );
			timeBetweenSpawns = startTimeBetweenSpawns;
		}
		else
		{
			timeBetweenSpawns -= Time.deltaTime;
		}
	}
}
