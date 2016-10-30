using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GreenPlayer : PlayerController, IInputListener
{
	protected override void OnStart ()
	{
		joystickIndex = 1;

		uiCurrentItemText = GameObject.Find ("UI/InGameUI/GreenPlayerUI/CurrentItemText").GetComponent<Text> ();
		uiCurrentItemText.text = "Item: None";
	}

	protected override void OnUpdate ()
	{

	}
}
