using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using NaughtyAttributes;
using System;
using UnityEngine.Networking;

public class ServerUI : MonoBehaviour
{
	[BoxGroup( "Text" )] [SerializeField] private TextMeshProUGUI serverInfoText;

	[BoxGroup( "Inputfields" )] [SerializeField] private TMP_InputField userID;
	[BoxGroup( "Inputfields" )] [SerializeField] private TMP_InputField score;

	[BoxGroup( "ScriptableObjects" )] [SerializeField] private ServerDataScriptableObject serverData;
	[BoxGroup( "ScriptableObjects" )] [SerializeField] private UserDataScriptableObject userData;

	[BoxGroup( "Feedback Text" )] [SerializeField] private TextMeshProUGUI feedbackText;

	private void OnEnable()
	{
		serverInfoText.text = $"Session ID: {serverData.sessionID}";
		feedbackText.text = string.Empty;
	}

	public void OnInsertScoreButtonPressed()
	{
		StartCoroutine( InsertScoreCoroutine( score.text ) );
	}

	private IEnumerator InsertScoreCoroutine( string score )
	{
		feedbackText.gameObject.SetActive( true );
		feedbackText.text = "";

		if( !FailChecks() )
		{
			yield break;
		}

		WWWForm form = new WWWForm();
		form.AddField( "pid", userID.text );
		form.AddField( "score", score );

		using UnityWebRequest www = UnityWebRequest.Post( "https://studentdav.hku.nl/~jaydee.alkema/databasing/server_insert_score.php", form );
		yield return www.SendWebRequest();

		if( www.result == UnityWebRequest.Result.ConnectionError )
		{
			Debug.LogError( www.error );
		}
		else
		{
			Debug.Log( www.downloadHandler.text );
			if( !www.downloadHandler.text.Contains( "1" ) )
			{
				feedbackText.color = Color.red;
				feedbackText.text = www.downloadHandler.text;
			}
			else
			{
				feedbackText.color = Color.green;
				feedbackText.text = "Score Insert succesfully!";
			}
		}
	}

	private bool FailChecks()
	{
		if( userID.text == string.Empty )
		{
			feedbackText.color = Color.red;
			feedbackText.text = "Username can't be empty!";

			return false;
		}

		if( score.text == string.Empty )
		{
			feedbackText.color = Color.red;
			feedbackText.text = "Score can't be empty!";

			return false;
		}

		return true;
	}
}
