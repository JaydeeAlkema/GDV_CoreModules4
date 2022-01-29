using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NaughtyAttributes;

public class ServerLoginUI : MonoBehaviour
{
	[BoxGroup( "Log in Inputfields" )] [SerializeField] private TMP_InputField serverNameInputField;
	[BoxGroup( "Log in Inputfields" )] [SerializeField] private TMP_InputField serverPasswordInputField;

	[BoxGroup( "Toggles" )] [SerializeField] private Toggle rememberCredentialsToggle;

	[BoxGroup( "Feedback Texts" )] [SerializeField] private TextMeshProUGUI feedbackText;

	[BoxGroup( "ScriptableObjects" )] [SerializeField] private ServerDataScriptableObject serverDataScriptableObject;

	private void OnEnable()
	{
		feedbackText.gameObject.SetActive( false );
		feedbackText.text = "";
	}

	private void Start()
	{
		feedbackText.gameObject.SetActive( false );
		if( PlayerPrefs.GetInt( "RememberServerCredentials" ) == 1 )
		{
			rememberCredentialsToggle.isOn = true;
			serverNameInputField.text = PlayerPrefs.GetString( "servername" );
			serverPasswordInputField.text = PlayerPrefs.GetString( "serverpassword" );
		}
	}

	public void OnLoginButtonPressed()
	{
		StartCoroutine( ServerLoginCoroutine( serverNameInputField.text, serverPasswordInputField.text ) );

		if( rememberCredentialsToggle.isOn )
		{
			PlayerPrefs.SetInt( "RememberServerCredentials", 1 );
			PlayerPrefs.SetString( "servername", serverNameInputField.text );
			PlayerPrefs.SetString( "serverpassword", serverPasswordInputField.text );
		}
		else
		{
			PlayerPrefs.SetInt( "RememberServerCredentials", 0 );
			PlayerPrefs.SetString( "servername", "" );
			PlayerPrefs.SetString( "serverpassword", "" );
		}
	}

	private IEnumerator ServerLoginCoroutine( string username, string password )
	{
		feedbackText.gameObject.SetActive( true );
		feedbackText.text = "";

		if( !FailChecks() )
		{
			yield break;
		}

		WWWForm form = new WWWForm();
		form.AddField( "servername", username );
		form.AddField( "serverpassword", password );

		using UnityWebRequest www = UnityWebRequest.Post( "https://studentdav.hku.nl/~jaydee.alkema/databasing/server_login.php", form );
		yield return www.SendWebRequest();

		if( www.result == UnityWebRequest.Result.ConnectionError )
		{
			Debug.LogError( www.error );
		}
		else
		{
			Debug.Log( www.downloadHandler.text );
			if( www.downloadHandler.text.Contains( "Incorrect" ) )
			{
				feedbackText.color = Color.red;
				feedbackText.text = "Incorrect server/password!";
			}
			else
			{
				feedbackText.color = Color.green;
				feedbackText.text = "Log in successfull! \nLoading Server Panel...";

				GetServerData( www );
			}
		}

		// Get session ID
		WWWForm sessionForm = new WWWForm();

		using UnityWebRequest SessionWWW = UnityWebRequest.Post( "https://studentdav.hku.nl/~jaydee.alkema/databasing/get_session.php", sessionForm );
		yield return SessionWWW.SendWebRequest();

		if( SessionWWW.result == UnityWebRequest.Result.ConnectionError )
		{
			Debug.LogError( SessionWWW.error );
		}
		else
		{
			Debug.Log( SessionWWW.downloadHandler.text );
			if( SessionWWW.downloadHandler.text != string.Empty )
			{
				serverDataScriptableObject.sessionID = SessionWWW.downloadHandler.text;
			}

		}

		yield return new WaitForSeconds( 2f );
		FindObjectOfType<UiSwitcher>().TogglePanel( 3 );
	}

	private void GetServerData( UnityWebRequest www )
	{
		ServerData serverData = new ServerData();
		serverData = ServerData.CreateFromJSON( www.downloadHandler.text );

		serverDataScriptableObject.id = serverData.id;
		serverDataScriptableObject.name = serverData.name;
	}

	private bool FailChecks()
	{
		if( serverNameInputField.text == string.Empty )
		{
			feedbackText.color = Color.red;
			feedbackText.text = "server name is empty!";

			return false;
		}

		// Check if username is not empty and/or contains illegal characters.
		if( !CheckForNotLatin( serverNameInputField.text ) )
		{
			feedbackText.color = Color.red;
			feedbackText.text = "server name contains illegal characters!";

			serverNameInputField.text = "";

			return false;
		}

		return true;
	}

	//char can be casted to int to acquire letter's code
	//returns true if letters are latin
	bool CheckForNotLatin( string stringToCheck )
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
}
