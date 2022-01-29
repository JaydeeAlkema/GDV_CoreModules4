using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class EnemyBehaviour : MonoBehaviour
{
	[SerializeField] private float health = 100f;
	[Space]
	[SerializeField] PlayerController[] players;
	[SerializeField] PlayerController nearestPlayer;
	[SerializeField] private float speed;
	[Space]
	[SerializeField] private Image healthBarImage;

	private PhotonView view;
	private Score score;

	private void Start()
	{
		view = GetComponent<PhotonView>();
		players = FindObjectsOfType<PlayerController>();
		score = FindObjectOfType<Score>();
	}

	private void Update()
	{
		if( nearestPlayer != null )
		{
			transform.position = Vector2.MoveTowards( transform.position, nearestPlayer.transform.position, speed * Time.deltaTime );
		}

		GetNearestPlayer();
	}

	private void GetNearestPlayer()
	{
		float distance = Mathf.Infinity;

		for( int i = 0; i < players.Length; i++ )
		{
			float distanceToPlayer = Vector2.Distance( transform.position, players[i].transform.position );
			if( distanceToPlayer < distance )
			{
				distance = distanceToPlayer;
				nearestPlayer = players[i];
			}
		}

	}

	private void OnTriggerEnter2D( Collider2D collision )
	{
		if( collision.CompareTag( "Player" ) )
		{
			health -= 25f;
			healthBarImage.fillAmount = health / 100f;

			if( PhotonNetwork.IsMasterClient )
			{
				if( health <= 0 )
				{
					score.AddScore();
					PhotonNetwork.Destroy( this.gameObject );
				}
			}
		}
	}
}
