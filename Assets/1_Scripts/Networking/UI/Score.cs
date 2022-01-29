using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class Score : MonoBehaviour
{
	[SerializeField] private int score = 0;
	[SerializeField] private PhotonView view;
	[SerializeField] private TextMeshProUGUI scoreText;

	private void Start()
	{
		view = GetComponent<PhotonView>();
	}

	public void AddScore()
	{
		view.RPC( "AddScoreRPC", RpcTarget.All );
	}

	[PunRPC]
	public void AddScoreRPC()
	{
		score++;
		scoreText.text = $"Combined Score\n{score}";
	}
}
