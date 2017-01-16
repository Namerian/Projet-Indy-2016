using UnityEngine;
using System.Collections;

public interface IInputListener
{
	void OnHandleLeftStick (int joystickIndex, Vector2 stickState);

	void OnHandleAButton (int joystickIndex, bool pressed);

	void OnHandleBButton (int joystickIndex, bool pressed);

	void OnHandleXButton (int joystickIndex, bool pressed);

	void OnHandleYButton (int joystickIndex, bool pressed);
}
