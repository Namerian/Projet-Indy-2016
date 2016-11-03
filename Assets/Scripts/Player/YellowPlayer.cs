using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class YellowPlayer : PlayerController
{

	protected override void OnStart ()
	{
		uiCurrentItemText = GameObject.Find ("UI/InGameUI/YellowPlayerUI/CurrentItemText").GetComponent<Text> ();
		uiCurrentItemText.text = "Item: None";
	}

	protected override void OnUpdate ()
	{

	}
}
