using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using NaughtyAttributes;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
	private static UIManager instance;

	public static UIManager Instance { get => instance; set => instance = value; }

	[BoxGroup( "Text" )] [SerializeField] private TextMeshProUGUI exitText;

	private void Awake()
	{
		if( !instance || instance != this )
		{
			Destroy( instance );
			instance = this;
		}
		exitText.gameObject.SetActive( false );
	}

	void OnApplicationQuit()
	{
		StartCoroutine( SendLogRequest( "0" ) );
	}

	public IEnumerator SendLogRequest( string logtype )
	{
		WWWForm form = new WWWForm();
		form.AddField( "logtype", logtype );

		using UnityWebRequest www = UnityWebRequest.Post( "https://studentdav.hku.nl/~jaydee.alkema/databasing/insert_log.php", form );
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

	//char can be casted to int to acquire letter's code
	//returns true if letters are latin
	public bool CheckForNotLatin( string stringToCheck )
	{
		bool boolToReturn = false;
		foreach( char c in stringToCheck )
		{
			int code = ( int )c;
			// for lower and upper cases respectively
			if( ( code > 96 && code < 123 ) || ( code > 64 && code < 91 ) )
				boolToReturn = true;
			// visit http://www.dotnetperls.com/ascii-table for more codes
		}
		return boolToReturn;
	}

	public void OnExitButtonPressed()
	{
		StartCoroutine( ExitCoroutine() );
	}

	private IEnumerator ExitCoroutine()
	{
		FindObjectOfType<UiSwitcher>().DisableAllPanels();
		exitText.gameObject.SetActive( true );
		yield return new WaitForSeconds( 2f );
		Application.Quit();
	}

	public void OnHighscoreButtonPressed( GameObject highscorePanel )
	{
		highscorePanel.SetActive( !highscorePanel.activeInHierarchy );
	}

	public void LoadSceneByString( string sceneName )
	{
		SceneManager.LoadScene( sceneName );
	}
}
