using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GreenPlayer : PlayerController
{
	protected override void OnStart ()
	{
		uiCurrentItemText = GameObject.Find ("UI/InGameUI/GreenPlayerUI/CurrentItemText").GetComponent<Text> ();
		uiCurrentItemText.text = "Item: None";
	}

	protected override void OnUpdate ()
	{

	}
}
