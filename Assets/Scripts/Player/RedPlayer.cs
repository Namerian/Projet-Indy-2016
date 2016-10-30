using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RedPlayer : PlayerController
{

	protected override void OnStart ()
	{
		joystickIndex = 0;

		uiCurrentItemText = GameObject.Find ("UI/InGameUI/RedPlayerUI/CurrentItemText").GetComponent<Text> ();
		uiCurrentItemText.text = "Item: None";
	}

	protected override void OnUpdate ()
	{
	
	}
}
