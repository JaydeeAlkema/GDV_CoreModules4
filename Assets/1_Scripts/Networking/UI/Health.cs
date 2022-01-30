using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class Health : MonoBehaviour
{
	[SerializeField] private int health = 10;
	[SerializeField] private TextMeshProUGUI healthText;
	[Space]
	[SerializeField] private GameObject gameOver;

	private PhotonView view;

	private void Start()
	{
		view = GetComponent<PhotonView>();
	}

	public void TakeDamage()
	{
		view.RPC( "TakeDamageRPC", RpcTarget.All );
	}

	[PunRPC]
	private void TakeDamageRPC()
	{
		health--;

		if( health <= 0 )
		{
			gameOver.SetActive( true );
		}

		healthText.text = $"Team Health:\n{health}";
	}
}
