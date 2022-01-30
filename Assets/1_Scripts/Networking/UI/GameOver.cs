using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI scoreText;
	[SerializeField] private GameObject restartButton;

	private PhotonView view;
	private Score score;

	private void Start()
	{
		view = GetComponent<PhotonView>();

		score = FindObjectOfType<Score>();
		scoreText.text = $"Combined Score:\n{score.score}";

		if( PhotonNetwork.IsMasterClient == false )
		{
			restartButton.GetComponentInChildren<TextMeshProUGUI>().text = "Waiting for Host...";
			restartButton.GetComponent<Button>().interactable = false;
		}
	}

	public void OnRestartButtonPressed()
	{
		view.RPC( "RestartRPC", RpcTarget.All );
	}

	[PunRPC]
	private void RestartRPC()
	{
		PhotonNetwork.LoadLevel( "Game" );
	}

	public void OnQuitButtonPressed()
	{
		SceneManager.LoadScene( "Login" );
	}
}
