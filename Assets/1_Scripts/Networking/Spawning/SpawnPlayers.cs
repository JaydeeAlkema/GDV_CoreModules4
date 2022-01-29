using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviour
{
	[SerializeField] private GameObject playerPrefab;
	[Space]
	[SerializeField] private float minX;
	[SerializeField] private float maxX;
	[SerializeField] private float minY;
	[SerializeField] private float maxY;

	private void Start()
	{
		Vector2 randomPosition = new Vector2( Random.Range( minX, maxX ), Random.Range( minY, maxY ) );
		PhotonNetwork.Instantiate( playerPrefab.name, randomPosition, Quaternion.identity );
	}

	private void OnDrawGizmosSelected()
	{
		// Draw Spawn Area.
		Gizmos.color = Color.blue;
		Gizmos.DrawLine( new Vector3( minX, minY, 0 ), new Vector3( minX, maxY, 0 ) );
		Gizmos.DrawLine( new Vector3( minX, maxY, 0 ), new Vector3( maxX, maxY, 0 ) );
		Gizmos.DrawLine( new Vector3( maxX, maxY, 0 ), new Vector3( maxX, minY, 0 ) );
		Gizmos.DrawLine( new Vector3( maxX, minY, 0 ), new Vector3( minX, minY, 0 ) );
	}
}
