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
}
