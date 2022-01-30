using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
	[SerializeField] private GameObject restartButton;

	private PhotonView view;

	private void Start()
	{
		view = GetComponent<PhotonView>();

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
