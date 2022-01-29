using UnityEngine;

[CreateAssetMenu( fileName = "Server Data", menuName = "ScriptableObjects/New Server Data" )]
public class ServerDataScriptableObject : ScriptableObject
{
	public string id;
	public string name;
	public string sessionID;
}
