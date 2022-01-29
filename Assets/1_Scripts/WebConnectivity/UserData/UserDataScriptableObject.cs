using System.IO;
using UnityEngine;

[CreateAssetMenu( fileName = "User Data", menuName = "ScriptableObjects/New User Data" )]
public class UserDataScriptableObject : ScriptableObject
{
	public string id;
	public string username;
	public string firstname;
	public string lastname;
	public string email;
	public string registrationdate;
	public string rawJSON;


	private const string FILENAME = "user.dat";

	public void SaveToFile()
	{
		var filePath = Path.Combine( Application.persistentDataPath, FILENAME );

		if( !File.Exists( filePath ) )
		{
			File.Create( filePath );
		}

		var json = JsonUtility.ToJson( this );
		File.WriteAllText( filePath, json );
	}


	public void LoadDataFromFile()
	{
		var filePath = Path.Combine( Application.persistentDataPath, FILENAME );

		if( !File.Exists( filePath ) )
		{
			Debug.LogWarning( $"File \"{filePath}\" not found!", this );
			return;
		}

		var json = File.ReadAllText( filePath );
		JsonUtility.FromJsonOverwrite( json, this );
	}
}
