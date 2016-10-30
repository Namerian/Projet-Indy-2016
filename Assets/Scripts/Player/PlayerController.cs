using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

public abstract class PlayerController : MonoBehaviour, IInputListener
{
	private const float TIME_DASH = 0.25f;
	private const float TIME_PUSH = 0.05f;
	private const float SPEED_DASH = 15;
	private const float SPEED_NORMAL = 5f;
	private const float SPEED_PUSH = 25f;

	private CharacterController characterController;
	private GameController gameController;

	protected Text uiCurrentItemText;

	protected int joystickIndex;

	private bool xButtonCurrentlyPressed;
	private bool xButtonPreviouslyPressed;
	private bool aButtonCurrentlyPressed;
	private bool aButtonPreviouslyPressed;

	private bool isDashing;
	private bool isPushed;

	private float dashTimer;
	private float pushTimer;

	private Vector3 moveDirection;
	private Vector3 pushDirection;

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
			if (dashTimer >= TIME_DASH) {
				isDashing = false;
			} else {
				dashTimer += Time.deltaTime;
				_speed = SPEED_DASH;
			}
		}

		Vector3 _motion = moveDirection * _speed;

		if (isPushed) {
			if (pushTimer >= TIME_PUSH) {
				isPushed = false;
			} else {
				pushTimer += Time.deltaTime;
				_motion += pushDirection * SPEED_PUSH;
			}
		}

		characterController.Move (_motion * Time.deltaTime);

		//
		OnUpdate ();
	}

	//########################################################################
	// Collison
	//########################################################################

	void OnControllerColliderHit (ControllerColliderHit hit)
	{
		if (hit.gameObject.tag == "Player" && isDashing) {
			PlayerController _otherPlayer = hit.gameObject.GetComponent<PlayerController> ();
			Vector3 _pushDirection = _otherPlayer.transform.position - this.transform.position;
			_pushDirection.Normalize ();
			_otherPlayer.Push (_pushDirection);

			isDashing = false;
		}
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

	void IInputListener.OnHandleLeftStick (int joystickIndex, Vector2 stickState)
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

	void IInputListener.OnHandleXButton (int joystickIndex, bool pressed)
	{
		xButtonPreviouslyPressed = xButtonCurrentlyPressed;
		xButtonCurrentlyPressed = pressed;

		if (joystickIndex == this.joystickIndex && !xButtonPreviouslyPressed && xButtonCurrentlyPressed && !isDashing) {
			isDashing = true;
			dashTimer = 0f;
		}
	}

	void IInputListener.OnHandleAButton (int joystickIndex, bool pressed)
	{
		aButtonPreviouslyPressed = aButtonCurrentlyPressed;
		aButtonCurrentlyPressed = pressed;

		if (joystickIndex == this.joystickIndex && !aButtonPreviouslyPressed && aButtonCurrentlyPressed && !isDashing && overlappingItems.Count > 0) {
			DropCurrentItem ();

			currentItem = overlappingItems [0];
			currentItem.gameObject.SetActive (false);
			uiCurrentItemText.text = "Item: " + currentItem.itemName;
		}
	}

	//########################################################################
	//
	//########################################################################

	public void Push (Vector3 direction)
	{
		isPushed = true;
		pushTimer = 0f;
		pushDirection = direction;

		DropCurrentItem ();
	}

	//########################################################################
	//
	//########################################################################

	private void DropCurrentItem ()
	{
		if (currentItem != null) {
			currentItem.transform.position = this.transform.position + moveDirection;
			currentItem.gameObject.SetActive (true);

			uiCurrentItemText.text = "Item: None";
			currentItem = null;
		}
	}
}
