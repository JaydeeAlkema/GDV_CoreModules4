using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NaughtyAttributes;

public class LoginUI : MonoBehaviour
{
	[BoxGroup( "Log in Inputfields" )] [SerializeField] private TMP_InputField usernameInputField;
	[BoxGroup( "Log in Inputfields" )] [SerializeField] private TMP_InputField passwordInputField;

	[BoxGroup( "Toggles" )] [SerializeField] private Toggle rememberCredentialsToggle;

	[BoxGroup( "Feedback Texts" )] [SerializeField] private TextMeshProUGUI feedbackText;

	[BoxGroup( "ScriptableObjects" )] [SerializeField] private UserDataScriptableObject userDataScriptableObject;

	private void OnEnable()
	{
		feedbackText.gameObject.SetActive( false );
		feedbackText.text = "";

		if( PlayerPrefs.GetInt( "RememberCredentials" ) == 1 )
		{
			rememberCredentialsToggle.isOn = true;
			usernameInputField.text = PlayerPrefs.GetString( "username" );
			passwordInputField.text = PlayerPrefs.GetString( "password" );
		}
	}

	public void OnLoginButtonPressed()
	{
		StartCoroutine( LoginCoroutine( usernameInputField.text, passwordInputField.text ) );

		if( rememberCredentialsToggle.isOn )
		{
			PlayerPrefs.SetInt( "RememberCredentials", 1 );
			PlayerPrefs.SetString( "username", usernameInputField.text );
			PlayerPrefs.SetString( "password", passwordInputField.text );
		}
		else
		{
			PlayerPrefs.SetInt( "RememberCredentials", 0 );
			PlayerPrefs.SetString( "username", "" );
			PlayerPrefs.SetString( "password", "" );
		}
	}

	private IEnumerator LoginCoroutine( string username, string password )
	{
		feedbackText.gameObject.SetActive( true );
		feedbackText.text = "";

		if( !FailChecks() )
		{
			yield break;
		}

		yield return StartCoroutine( SendLoginrequest( username, password ) );
	}

	private IEnumerator SendLoginrequest( string username, string password )
	{
		WWWForm form = new WWWForm();
		form.AddField( "username", username );
		form.AddField( "password", password );

		using UnityWebRequest www = UnityWebRequest.Post( "https://studentdav.hku.nl/~jaydee.alkema/databasing/user_login.php", form );
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
				feedbackText.text = "Incorrect username/password!";
			}
			else
			{
				feedbackText.color = Color.green;
				feedbackText.text = "Log in successfull! \nLoading Profile Page...";

				StartCoroutine( UIManager.Instance.SendLogRequest( "1" ) );

				GetUserData( www );
			}
		}

		yield return new WaitForSeconds( 2f );
		FindObjectOfType<UiSwitcher>().TogglePanel( 4 );
	}


	private void GetUserData( UnityWebRequest www )
	{
		UserData userData = new UserData();
		userData = UserData.CreateFromJSON( www.downloadHandler.text );

		userDataScriptableObject.id = userData.id;
		userDataScriptableObject.username = usernameInputField.text;
		userDataScriptableObject.firstname = userData.first_name;
		userDataScriptableObject.lastname = userData.last_name;
		userDataScriptableObject.email = userData.email;
		userDataScriptableObject.registrationdate = userData.registrationdate;
		userDataScriptableObject.rawJSON = www.downloadHandler.text;
	}

	private bool FailChecks()
	{
		if( usernameInputField.text == string.Empty )
		{
			feedbackText.color = Color.red;
			feedbackText.text = "Username is empty!";

			return false;
		}

		// Check if username is not empty and/or contains illegal characters.
		if( !UIManager.Instance.CheckForNotLatin( usernameInputField.text ) )
		{
			feedbackText.color = Color.red;
			feedbackText.text = "Username contains illegal characters!";

			usernameInputField.text = "";

			return false;
		}

		return true;
	}
}
