using UnityEngine;
using System.Collections;

public interface IInputListener
{
	void OnHandleLeftStick (int joystickIndex, Vector2 stickState);

	void OnHandleXButton (int joystickIndex, bool pressed);
}
