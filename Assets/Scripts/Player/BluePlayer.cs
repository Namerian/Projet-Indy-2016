using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BluePlayer : PlayerController
{

	protected override void OnStart ()
	{
		uiCurrentItemText = GameObject.Find ("UI/InGameUI/BluePlayerUI/CurrentItemText").GetComponent<Text> ();
		uiCurrentItemText.text = "Item: None";
	}

	protected override void OnUpdate ()
	{

	}
}
