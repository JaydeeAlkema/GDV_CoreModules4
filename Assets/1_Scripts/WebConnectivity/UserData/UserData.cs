using System.IO;
using UnityEngine;

public class UserData
{
	public string id;
	public string username;
	public string first_name;
	public string last_name;
	public string email;
	public string registrationdate;

	public static UserData CreateFromJSON( string jsonString )
	{
		return JsonUtility.FromJson<UserData>( jsonString );
	}

}
