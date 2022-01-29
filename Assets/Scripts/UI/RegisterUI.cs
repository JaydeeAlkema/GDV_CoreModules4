using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NaughtyAttributes;

public class RegisterUI : MonoBehaviour
{
	[BoxGroup( "Sign up Inputfields" )] [SerializeField] private TMP_InputField usernameInputField;
	[BoxGroup( "Sign up Inputfields" )] [SerializeField] private TMP_InputField firstnameInputField;
	[BoxGroup( "Sign up Inputfields" )] [SerializeField] private TMP_InputField lastnameInputField;
	[BoxGroup( "Sign up Inputfields" )] [SerializeField] private TMP_InputField passwordInputField;
	[BoxGroup( "Sign up Inputfields" )] [SerializeField] private TMP_InputField repeatPasswordInputField;
	[BoxGroup( "Sign up Inputfields" )] [SerializeField] private TMP_InputField emailInputField;

	[BoxGroup( "Feedback Texts" )] [SerializeField] private TextMeshProUGUI feedbackText;

	private void OnEnable()
	{
		feedbackText.gameObject.SetActive( false );
		feedbackText.text = "";
	}

	public void OnSignupButtonPressed()
	{
		StartCoroutine( SignupCoroutine( usernameInputField.text, firstnameInputField.text, lastnameInputField.text, passwordInputField.text, emailInputField.text ) );
	}

	private IEnumerator SignupCoroutine( string username, string firstname, string lastname, string password, string email )
	{
		feedbackText.gameObject.SetActive( true );
		feedbackText.text = "";

		if( !FailChecks() )
		{
			yield break;
		}

		WWWForm form = new WWWForm();
		form.AddField( "username", username );
		form.AddField( "firstname", firstname );
		form.AddField( "lastname", lastname );
		form.AddField( "password", Hash.getHashSha256( password ) );
		form.AddField( "email", email );

		using UnityWebRequest www = UnityWebRequest.Post( "https://studentdav.hku.nl/~jaydee.alkema/databasing/user_signup.php", form );
		yield return www.SendWebRequest();

		if( www.result == UnityWebRequest.Result.ConnectionError )
		{
			Debug.LogError( www.error );
		}
		else
		{
			Debug.Log( www.downloadHandler.text );
			if( www.downloadHandler.text.Contains( "Username already taken" ) )
			{
				feedbackText.color = Color.red;
				feedbackText.text = "Username already taken!";
			}
			else
			{
				feedbackText.color = Color.green;
				feedbackText.text = $"User created with the ID:{www.downloadHandler.text} \nReturning to Log in Screen...";
			}
		}

		yield return new WaitForSeconds( 4f );
		FindObjectOfType<UiSwitcher>().TogglePanel( 0 );
	}

	private bool FailChecks()
	{
		// First check should be to see if all fields aren't empty.
		if( usernameInputField.text == string.Empty && firstnameInputField.text == string.Empty && lastnameInputField.text == string.Empty &&
			passwordInputField.text == string.Empty && repeatPasswordInputField.text == string.Empty && emailInputField.text == string.Empty )
		{
			feedbackText.color = Color.red;
			feedbackText.text = "All fields are required!";

			return false;
		}

		// Check if passwords match.
		if( passwordInputField.text != repeatPasswordInputField.text )
		{
			feedbackText.color = Color.red;
			feedbackText.text = "Passwords do not match!";

			passwordInputField.text = "";
			repeatPasswordInputField.text = "";

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

		// Check if the email actually contains @
		if( !emailInputField.text.Contains( "@" ) )
		{
			feedbackText.color = Color.red;
			feedbackText.text = "Email adress does not contain '@'!";

			emailInputField.text = "";

			return false;
		}


		return true;
	}
}
