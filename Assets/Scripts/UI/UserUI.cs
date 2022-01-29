using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using TMPro;

public class UserUI : MonoBehaviour
{
	[BoxGroup( "Text" )] [SerializeField] private TextMeshProUGUI usernameText;
	[BoxGroup( "Text" )] [SerializeField] private TextMeshProUGUI rawJSON;

	[BoxGroup( "ScriptableObjects" )] [SerializeField] private UserDataScriptableObject userData;

	private void OnEnable()
	{
		usernameText.text = $"Currently logged in as {userData.username}";
		rawJSON.text = userData.rawJSON;
	}
}
