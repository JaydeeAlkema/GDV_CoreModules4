using Photon.Pun;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
	[SerializeField] private PhotonView view;
	[SerializeField] private TextMeshProUGUI scoreText;
	[SerializeField] private TextMeshProUGUI gameOverScoreText;

	private GameManager gameManager;
	private List<PlayerController> players = new List<PlayerController>();

	private void Start()
	{
		gameManager = FindObjectOfType<GameManager>();
		view = GetComponent<PhotonView>();

		scoreText.text = $"Scores";
	}

	public void UpdateScoreCounter()
	{
		view.RPC( "UpdateScoreCounterRPC", RpcTarget.All );
	}

	[PunRPC]
	private void UpdateScoreCounterRPC()
	{
		if( players.Count == 0 ) { players = gameManager.Players; }
		players.Sort( SortByScore );
		scoreText.text = $"Scores\n{players[0].UsernameText.text}: {players[0].score}\n{players[1].UsernameText.text}: {players[1].score}";
		gameOverScoreText.text = $"Scores\n{players[0].UsernameText.text}: {players[0].score}\n{players[1].UsernameText.text}: {players[1].score}";
	}

	static int SortByScore( PlayerController p1, PlayerController p2 )
	{
		return p2.score.CompareTo( p1.score );
	}
}
