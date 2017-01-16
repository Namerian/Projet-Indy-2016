using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class InputHandler : MonoBehaviour
{
	public static readonly string[] JOYSTICK_NAMES = {
		"Joystick_1",
		"Joystick_2",
		"Joystick_3",
		"Joystick_4"
	};

	public static InputHandler Instance{ get; private set; }

	//===========================

	private List<IInputListener>[] inputListeners;

	//################################################################################

	void Awake ()
	{
		if (Instance == null) {
			Instance = this;

			inputListeners = new List<IInputListener>[4];

			for (int i = 0; i < 4; i++) {
				inputListeners [i] = new List<IInputListener> ();
			}
		} else {
			Destroy (this);
		}
	}

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Escape)) {
			#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
			#else
			Application.Quit ();
			#endif
		}

		for (int joystickIndex = 0; joystickIndex < 4; joystickIndex++) {
			/*if (inputListeners [_joystickIndex].Count == 0) {
				continue;
			}*/

			List<IInputListener> listenerList = new List<IInputListener> (inputListeners [joystickIndex]);

			//######################################################################
			// left stick

			float xAxis = Input.GetAxis (JOYSTICK_NAMES [joystickIndex] + "_X_Axis");
			float yAxis = Input.GetAxis (JOYSTICK_NAMES [joystickIndex] + "_Y_Axis");
			Vector2 leftStickState = new Vector2 (xAxis, yAxis);

			/*if (_xAxis != 0 || _yAxis != 0) {
				Debug.Log ("InputHandler: Update: Joystick " + _joystickIndex + ": leftStick=" + _leftStickState.ToString ());
			}*/

			foreach (IInputListener listener in listenerList) {
				listener.OnHandleLeftStick (joystickIndex, leftStickState);
			}

			//######################################################################
			// a button

			float aButton = Input.GetAxis (JOYSTICK_NAMES [joystickIndex] + "_A_Button");
			bool aPressed = false;

			if (aButton > 0) {
				aPressed = true;
			}

			foreach (IInputListener listener in listenerList) {
				listener.OnHandleAButton (joystickIndex, aPressed);
			}

			//######################################################################
			// b button

			float bButton = Input.GetAxis (JOYSTICK_NAMES [joystickIndex] + "_B_Button");
			bool bPressed = false;

			if (bButton > 0) {
				bPressed = true;
			}

			foreach (IInputListener listener in listenerList) {
				listener.OnHandleBButton (joystickIndex, bPressed);
			}

			//######################################################################
			// x button

			bool xPressed = Input.GetButton (JOYSTICK_NAMES [joystickIndex] + "_X_Button");

			/*if (_xPressed) {
				Debug.Log ("InputHandler: Update: Joystick " + _joystickIndex + ": X Button pressed (" + _xPressed + ")");
			}*/

			foreach (IInputListener listener in listenerList) {
				listener.OnHandleXButton (joystickIndex, xPressed);
			}

			//######################################################################
			// y button

			bool yPressed = Input.GetButton (JOYSTICK_NAMES [joystickIndex] + "_Y_Button");

			/*if (_xPressed) {
				Debug.Log ("InputHandler: Update: Joystick " + _joystickIndex + ": X Button pressed (" + _xPressed + ")");
			}*/

			foreach (IInputListener listener in listenerList) {
				listener.OnHandleYButton (joystickIndex, yPressed);
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
	/*public void AddInputListener (IInputListener listener)
	{
		
	}*/

	/// <summary>
	/// Deregisters an InputListener.
	/// </summary>
	public void RemoveInputListener (IInputListener listener)
	{
		for (int i = 0; i < 4; i++) {
			if (inputListeners [i].Contains (listener)) {
				inputListeners [i].Remove (listener);
			}
		}
	}
}
