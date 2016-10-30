using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

public abstract class PlayerController : MonoBehaviour, IInputListener
{
	private const float DASH_TIME = 0.25f;
	private const float SPEED_DASH = 15;
	private const float SPEED_NORMAL = 5f;

	private CharacterController characterController;
	private GameController gameController;

	protected Text uiCurrentItemText;

	protected int joystickIndex;

	private bool xButtonCurrentlyPressed;
	private bool xButtonPreviouslyPressed;
	private bool aButtonCurrentlyPressed;
	private bool aButtonPreviouslyPressed;

	private Vector3 moveDirection;
	private bool isDashing;
	private float dashTimer;

	private Item currentItem;
	private List<Item> overlappingItems;

	//#############################################################################

	protected abstract void OnStart ();

	protected abstract void OnUpdate ();

	//#############################################################################

	void Awake ()
	{
		moveDirection = new Vector3 ();

		currentItem = null;
		overlappingItems = new List<Item> ();
	}

	// Use this for initialization
	void Start ()
	{
		characterController = GetComponent<CharacterController> ();

		GameObject _gameControllerObj = GameObject.FindGameObjectWithTag ("GameController");
		gameController = _gameControllerObj.GetComponent<GameController> ();
		_gameControllerObj.GetComponent<InputHandler> ().AddInputListener (this, InputHandler.JOYSTICK_NAMES [joystickIndex]);

		//
		OnStart ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		float _speed = SPEED_NORMAL;

		if (isDashing) {
			if (dashTimer >= DASH_TIME) {
				isDashing = false;
			} else {
				dashTimer += Time.deltaTime;
				_speed = SPEED_DASH;
			}
		}

		Vector3 _motion = moveDirection * _speed;

		characterController.Move (_motion * Time.deltaTime);

		//
		OnUpdate ();
	}

	//########################################################################
	// Collison
	//########################################################################

	void OnControllerColliderHit (ControllerColliderHit hit)
	{
		
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Item") {
			Item _item = other.GetComponent<Item> ();

			if (!overlappingItems.Contains (_item)) {
				overlappingItems.Add (_item);
				//Debug.Log ("PlayerController: OnTriggerEnter: overlapping with " + _item.itemName);
			}
		}
	}

	void OnTriggerExit (Collider other)
	{
		if (other.tag == "Item") {
			Item _item = other.GetComponent<Item> ();

			if (overlappingItems.Contains (_item)) {
				overlappingItems.Remove (_item);
			}
		}
	}

	//########################################################################
	// Input
	//########################################################################

	public void OnHandleLeftStick (int joystickIndex, Vector2 stickState)
	{
		if (joystickIndex == this.joystickIndex && !isDashing) {
			moveDirection.Set (0, 0, 0);

			if (stickState.x != 0) {
				moveDirection.x = stickState.x;
			}

			if (stickState.y != 0) {
				moveDirection.z = stickState.y;
			}

			moveDirection.Normalize ();
		}
	}

	public void OnHandleXButton (int joystickIndex, bool pressed)
	{
		xButtonPreviouslyPressed = xButtonCurrentlyPressed;
		xButtonCurrentlyPressed = pressed;

		if (joystickIndex == this.joystickIndex && !xButtonPreviouslyPressed && xButtonCurrentlyPressed && !isDashing) {
			isDashing = true;
			dashTimer = 0f;
		}
	}

	public void OnHandleAButton (int joystickIndex, bool pressed)
	{
		aButtonPreviouslyPressed = aButtonCurrentlyPressed;
		aButtonCurrentlyPressed = pressed;

		if (joystickIndex == this.joystickIndex && !aButtonPreviouslyPressed && aButtonCurrentlyPressed && !isDashing && overlappingItems.Count > 0) {
			if (currentItem != null) {
				
			}

			currentItem = overlappingItems [0];
			currentItem.gameObject.SetActive (false);
			uiCurrentItemText.text = "Item: " + currentItem.itemName;
		}
	}
}
