using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : MonoBehaviourPunCallbacks
{
	[SerializeField] private List<PlayerController> players = new List<PlayerController>();
	[Space]
	[SerializeField] private GameObject gameOver;

	public List<PlayerController> Players { get => players; set => players = value; }

	private void Start()
	{
		Random.InitState( Random.Range( 0, int.MaxValue ) );
	}

	private void Update()
	{
		StartCoroutine( GetPlayers() );
	}

	private IEnumerator GetPlayers()
	{
		if( players.Count < 2 )
		{
			PlayerController[] _players = FindObjectsOfType<PlayerController>();

			foreach( PlayerController player in _players )
			{
				if( !players.Contains( player ) )
				{
					players.Add( player );
				}
			}
			yield return new WaitForSeconds( 1f );
		}
		else
		{
			yield break;
		}
	}

	public void GameOver()
	{
		gameOver.SetActive( true );

		if( PhotonNetwork.IsMasterClient )
		{
			StartCoroutine( InsertScoreIntoDatabase( players[0].score ) );
		}
		else
		{
			StartCoroutine( InsertScoreIntoDatabase( players[1].score ) );
		}
	}

	private IEnumerator InsertScoreIntoDatabase( int score )
	{
		WWWForm form = new WWWForm();
		form.AddField( "sid", Random.Range( 1, 6 ) );
		form.AddField( "score", score );

		using UnityWebRequest www = UnityWebRequest.Post( "https://studentdav.hku.nl/~jaydee.alkema/databasing/insert_score.php", form );
		yield return www.SendWebRequest();

		if( www.result == UnityWebRequest.Result.ConnectionError )
		{
			Debug.LogError( www.error );
		}
		else
		{
			Debug.Log( www.downloadHandler.text );
		}
	}
}
