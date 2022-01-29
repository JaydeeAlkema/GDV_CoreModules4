using System.Collections.Generic;
using UnityEngine;

public class UiSwitcher : MonoBehaviour
{
	[SerializeField] private List<GameObject> panels = new List<GameObject>();

	public void TogglePanel( int index )
	{
		for( int i = 0; i < panels.Count; i++ )
		{
			if( i != index )
			{
				panels[i].SetActive( false );
			}
			else
			{
				panels[i].SetActive( true );
			}
		}
	}

	public void DisableAllPanels()
	{
		foreach( GameObject panel in panels )
		{
			panel.SetActive( false );
		}
	}
}
