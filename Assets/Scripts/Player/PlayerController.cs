using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

public abstract class PlayerController : MonoBehaviour, IInputListener
{
	private CharacterController characterController;
	private GameController gameController;

	// UI
	protected Text uiCurrentItemText;

	//
	private int joystickIndex;

	// Input
	private bool xButtonCurrentlyPressed;
	private bool xButtonPreviouslyPressed;
	private bool aButtonCurrentlyPressed;
	private bool aButtonPreviouslyPressed;

	//
	private bool isDashing;
	private bool isPushed;

	private float dashTimer;
	private float pushTimer;

	private Vector3 movement_acceleration;
	private Vector3 dash_acceleration;
	private Vector3 push_acceleration;

	private Vector3 velocity;


	// Items
	private Item currentItem;
	private GameObject currentItemGO;
	private List<Item> overlappingItems;

	//#############################################################################

	protected abstract void OnStart ();

	protected abstract void OnUpdate ();

	//#############################################################################

	void Awake ()
	{
		movement_acceleration = new Vector3 ();
		velocity = new Vector3 ();
		dash_acceleration = new Vector3 ();
		push_acceleration = new Vector3 ();

		currentItem = null;
		overlappingItems = new List<Item> ();
	}

	// Use this for initialization
	void Start ()
	{
		characterController = GetComponent<CharacterController> ();

		GameObject _gameControllerObj = GameObject.FindGameObjectWithTag ("GameController");
		gameController = _gameControllerObj.GetComponent<GameController> ();

		//
		OnStart ();
	}

	public void Initialize (int joystickIndex)
	{
		this.joystickIndex = joystickIndex;
		gameController.GetComponent<InputHandler> ().AddInputListener (this, InputHandler.JOYSTICK_NAMES [joystickIndex]);
	}
	
	// Update is called once per frame
	void Update ()
	{
		//Debug.Log ("PlayerController " + joystickIndex + ": Update: acceleration=" + movement_acceleration.ToString ());

		Vector3 _totalAcceleration = new Vector3 ();

		//normal movement
		if (!isDashing) {
			_totalAcceleration += movement_acceleration;
			//Debug.Log ("PlayerController " + joystickIndex + ": Update: movementVelocity=" + movement_velocity.ToString ());
		}
		//dashing movement
		else if (isDashing) {
			if (dashTimer >= PlayerConstants.DASH_DURATION) {
				isDashing = false;
			} else {
				dashTimer += Time.deltaTime;
				_totalAcceleration += dash_acceleration;
				//Debug.Log ("PlayerController " + joystickIndex + ": Update: dashVelocity=" + dash_velocity.ToString ());
			}
		}

		if (isPushed) {
			if (pushTimer >= PlayerConstants.PUSH_DURATION) {
				isPushed = false;
			} else {
				pushTimer += Time.deltaTime;
				_totalAcceleration += push_acceleration;
			}
		}

		velocity += _totalAcceleration - PlayerConstants.MOVEMENT_FRICTION * velocity;
		velocity = Vector3.ClampMagnitude (velocity, PlayerConstants.MOVEMENT_MAX_VELOCITY);

		characterController.Move (velocity * Time.deltaTime);

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
		/*if (joystickIndex == this.joystickIndex && !isDashing) {
			moveDirection.Set (0, 0, 0);

			if (stickState.x != 0) {
				moveDirection.x = stickState.x;
			}

			if (stickState.y != 0) {
				moveDirection.z = stickState.y;
			}

			moveDirection.Normalize ();
		}*/

		//Debug.Log ("PlayerController: OnHandleLeftStick: called");

		if (joystickIndex == this.joystickIndex) {
			//Debug.Log ("PlayerController " + joystickIndex + ": OnHandleLeftStick: called");

			float _force = Mathf.Clamp (stickState.magnitude, 0f, 1f) * PlayerConstants.MOVEMENT_MAX_ACCELERATION;
			//Debug.Log ("PlayerController " + joystickIndex + ": stickstate=" + stickState.ToString ());
			//Debug.Log ("PlayerController " + joystickIndex + ": force=" + _force);

			movement_acceleration.Set (0, 0, 0);

			movement_acceleration.x = stickState.x;
			movement_acceleration.z = stickState.y;

			movement_acceleration.Normalize ();
			movement_acceleration *= _force;
		}
	}

	void IInputListener.OnHandleXButton (int joystickIndex, bool pressed)
	{
		xButtonPreviouslyPressed = xButtonCurrentlyPressed;
		xButtonCurrentlyPressed = pressed;

		if (joystickIndex == this.joystickIndex && !xButtonPreviouslyPressed && xButtonCurrentlyPressed && !isDashing) {
			isDashing = true;
			dashTimer = 0f;
			dash_acceleration = movement_acceleration.normalized * PlayerConstants.DASH_VELOCITY;
		}
	}

	void IInputListener.OnHandleAButton (int joystickIndex, bool pressed)
	{
		aButtonPreviouslyPressed = aButtonCurrentlyPressed;
		aButtonCurrentlyPressed = pressed;

		if (joystickIndex == this.joystickIndex && !aButtonPreviouslyPressed && aButtonCurrentlyPressed && !isDashing && overlappingItems.Count > 0) {
			DropCurrentItem ();
			PickUpItem ();
			//Debug.Log ("PlayerController: OnHandleAButton: should have picked up item");
		} else if (joystickIndex == this.joystickIndex && !aButtonPreviouslyPressed && aButtonCurrentlyPressed && !isDashing) {
			DropCurrentItem ();
			//Debug.Log ("PlayerController: OnHandleAButton: should have dropped item");
		}
	}

	//########################################################################
	//
	//########################################################################

	public void Push (Vector3 direction)
	{
		isPushed = true;
		pushTimer = 0f;
		push_acceleration = direction * PlayerConstants.PUSH_VELOCITY;

		DropCurrentItem ();
	}

	//########################################################################
	//
	//########################################################################

	private void DropCurrentItem ()
	{
		if (currentItem != null) {
			currentItemGO.SetActive (true);
			currentItem.transform.position = this.transform.position;

			uiCurrentItemText.text = "Item: None";
			currentItem = null;
			currentItemGO = null;
		} /*else {
			Debug.Log ("PlayerController: DropCurrentItem: called but item = null");
		}*/
	}

	private void PickUpItem ()
	{
		if (currentItem == null) {
			currentItem = overlappingItems [0];
			currentItemGO = currentItem.gameObject;

			uiCurrentItemText.text = "Item: " + currentItem.itemName;

			overlappingItems.Remove (currentItem);
			currentItem.gameObject.SetActive (false);
		}
	}
}
