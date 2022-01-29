using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using NaughtyAttributes;
using UnityEngine.Networking;
using System;

public class HighscoresUI : MonoBehaviour
{
	[BoxGroup( "Score Text Elements" )] [SerializeField] private GameObject playerNameTextElementPrefab;
	[BoxGroup( "Score Text Elements" )] [SerializeField] private GameObject playerScoreTextElementPrefab;
	[Space]
	[BoxGroup( "Score Text Elements" )] [SerializeField] private Transform playerNameTextElementParent;
	[BoxGroup( "Score Text Elements" )] [SerializeField] private Transform playerScoreTextElementParent;

	[BoxGroup( "Misc Text Elements" )] [SerializeField] private TextMeshProUGUI totalPlayersTextElement;

	[BoxGroup( "Lists" )] [SerializeField] private List<string> playerNames = new List<string>();
	[BoxGroup( "Lists" )] [SerializeField] private List<string> playerScores = new List<string>();
	[Space]
	[BoxGroup( "Lists" )] [SerializeField] private List<GameObject> playerNamesInScene = new List<GameObject>();
	[BoxGroup( "Lists" )] [SerializeField] private List<GameObject> playerScoresInScene = new List<GameObject>();

	void OnEnable()
	{
		CleanUp();

		StartCoroutine( GetHighscores( 5 ) );
		StartCoroutine( GetTotalPlaysInThePreviousMonth() );
	}

	private void CleanUp()
	{
		playerNames.Clear();
		playerScores.Clear();

		foreach( GameObject playerNameGO in playerNamesInScene )
		{
			Destroy( playerNameGO );
		}

		foreach( GameObject playerScoreGO in playerScoresInScene )
		{
			Destroy( playerScoreGO );
		}

		playerNamesInScene.Clear();
		playerScoresInScene.Clear();
	}

	private IEnumerator GetHighscores( int limit )
	{
		WWWForm form = new WWWForm();
		form.AddField( "limit", limit );


		using UnityWebRequest www = UnityWebRequest.Post( "https://studentdav.hku.nl/~jaydee.alkema/databasing/get_highscores.php", form );
		yield return www.SendWebRequest();

		if( www.result == UnityWebRequest.Result.ConnectionError )
		{
			Debug.LogError( www.error );
		}
		else
		{
			ConvertJSONtoStringArray( www );

			for( int i = 0; i < playerNames.Count; i++ )
			{
				yield return StartCoroutine( GetPlayerNameByID( playerNames[i], i ) );
			}

			yield return FillHighscorePanel();
		}
	}

	private IEnumerator GetPlayerNameByID( string id, int index )
	{
		WWWForm form = new WWWForm();
		form.AddField( "id", id );

		using UnityWebRequest www = UnityWebRequest.Post( "https://studentdav.hku.nl/~jaydee.alkema/databasing/get_user_by_id.php", form );
		yield return www.SendWebRequest();

		if( www.result == UnityWebRequest.Result.ConnectionError )
		{
			Debug.LogError( www.error );
		}
		else
		{
			playerNames[index] = www.downloadHandler.text;
		}
	}

	private void ConvertJSONtoStringArray( UnityWebRequest www )
	{
		string rawJSON = www.downloadHandler.text;
		string[] _splitJSON = rawJSON.Split( new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries );
		List<string> splitJSON = new List<string>();
		splitJSON.AddRange( _splitJSON );

		for( int i = 0; i < splitJSON.Count; i += 2 )
		{
			playerNames.Add( splitJSON[i] );
		}

		for( int i = 1; i < splitJSON.Count; i += 2 )
		{
			playerScores.Add( splitJSON[i] );
		}

	}

	private IEnumerator FillHighscorePanel()
	{
		for( int i = 0; i < playerNames.Count; i++ )
		{
			GameObject newPlayerNameGO = Instantiate( playerNameTextElementPrefab, playerNameTextElementParent );
			newPlayerNameGO.GetComponentInChildren<TextMeshProUGUI>().text = playerNames[i];
			playerNamesInScene.Add( newPlayerNameGO );

			GameObject newPlayerScoreGO = Instantiate( playerScoreTextElementPrefab, playerScoreTextElementParent ); ;
			newPlayerScoreGO.GetComponentInChildren<TextMeshProUGUI>().text = $"{playerScores[i]}pts";
			playerScoresInScene.Add( newPlayerScoreGO );
		}

		yield return null;
	}

	private IEnumerator GetTotalPlaysInThePreviousMonth()
	{
		WWWForm form = new WWWForm();

		using UnityWebRequest www = UnityWebRequest.Post( "https://studentdav.hku.nl/~jaydee.alkema/databasing/get_total_plays.php", form );
		yield return www.SendWebRequest();

		if( www.result == UnityWebRequest.Result.ConnectionError )
		{
			Debug.LogError( www.error );
		}
		else
		{
			totalPlayersTextElement.text = $"Total Players in the past 30d: {www.downloadHandler.text}";
		}
	}
}
