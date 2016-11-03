using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputHandler : MonoBehaviour
{
	public static readonly string[] JOYSTICK_NAMES = { "Joystick_1", "Joystick_2", "Joystick_3", "Joystick_4" };

	private List<IInputListener>[] inputListeners;

	//################################################################################

	void Awake ()
	{
		inputListeners = new List<IInputListener>[4];

		for (int i = 0; i < 4; i++) {
			inputListeners [i] = new List<IInputListener> ();
		}
	}

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		for (int _joystickIndex = 0; _joystickIndex < 4; _joystickIndex++) {
			/*if (inputListeners [_joystickIndex].Count == 0) {
				continue;
			}*/

			//######################################################################
			// left stick

			float _xAxis = Input.GetAxis (JOYSTICK_NAMES [_joystickIndex] + "_X_Axis");
			float _yAxis = Input.GetAxis (JOYSTICK_NAMES [_joystickIndex] + "_Y_Axis");
			Vector2 _leftStickState = new Vector2 (_xAxis, _yAxis);

			/*if (_xAxis != 0 || _yAxis != 0) {
				Debug.Log ("InputHandler: Update: Joystick " + _joystickIndex + ": leftStick=" + _leftStickState.ToString ());
			}*/

			foreach (IInputListener listener in inputListeners[_joystickIndex]) {
				listener.OnHandleLeftStick (_joystickIndex, _leftStickState);
			}

			//######################################################################
			// x button

			float _xButton = Input.GetAxis (JOYSTICK_NAMES [_joystickIndex] + "_X_Button");
			bool _xPressed = false;

			if (_xButton > 0) {
				_xPressed = true;
				//Debug.Log ("InputHandler: Update: Joystick " + _joystickIndex + ": X Button pressed (" + _xButton + ")");
			}

			foreach (IInputListener listener in inputListeners[_joystickIndex]) {
				listener.OnHandleXButton (_joystickIndex, _xPressed);
			}

			//######################################################################
			// a button

			float _aButton = Input.GetAxis (JOYSTICK_NAMES [_joystickIndex] + "_A_Button");
			bool _aPressed = false;

			if (_aButton > 0) {
				_aPressed = true;
			}

			foreach (IInputListener listener in inputListeners[_joystickIndex]) {
				listener.OnHandleAButton (_joystickIndex, _aPressed);
			}
		}
	}

	/// <summary>
	/// Registers an InputListener that listens to a specific Joystick.
	/// </summary>
	public void AddInputListener (IInputListener listener, string joystickName)
	{
		for (int i = 0; i < 4; i++) {
			string _joystickName = JOYSTICK_NAMES [i];
			if (_joystickName == joystickName && !inputListeners [i].Contains (listener)) {
				inputListeners [i].Add (listener);
				return;
			}
		}
	}

	/// <summary>
	/// Registers an InputListener that listens to all Joysticks.
	/// </summary>
	public void AddInputListener (IInputListener listener)
	{
		
	}

	/// <summary>
	/// Deregisters an InputListener.
	/// </summary>
	public void RemoveInputListener (IInputListener listener)
	{
		
	}
}
