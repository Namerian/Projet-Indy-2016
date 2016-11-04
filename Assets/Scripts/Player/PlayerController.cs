using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

public abstract class PlayerController : MonoBehaviour, IInputListener
{
	//
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
	private float dashCooldown;

	private Vector3 movement_acceleration;
	private Vector3 dash_acceleration;
	private Vector3 push_acceleration;

	private Vector3 velocity;

	// Items
	private Item currentItem;
	private GameObject currentItemGO;
	private List<Item> overlappingItems;

	//Death
	private const float DEATH_FALL_SPEED_Y = -6f;
	//private const float RESPAWN_TIME = 3f;
	private Vector3 spawnPosition;
	private bool isDead;
	private float respawnTimer;

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

		spawnPosition = transform.position;

		//
		OnStart ();
	}

	public void Initialize (int joystickIndex)
	{
		this.joystickIndex = joystickIndex;
		gameController.GetComponent<InputHandler> ().AddInputListener (this, InputHandler.JOYSTICK_NAMES [joystickIndex]);
	}

	//########################################################################
	// Update
	//########################################################################
	
	// Update is called once per frame
	void Update ()
	{
		//##################################
		//death
		if (!isDead && !isDashing) {
			if (IsAboveVoid ()) {
				isDead = true;
				respawnTimer = 0f;

				isDashing = false;
				isPushed = false;

				movement_acceleration.Set (0, DEATH_FALL_SPEED_Y, 0);
			}
		} else if (isDead) {
			if (respawnTimer >= PlayerConstants.RESPAWN_COOLDOWN) {
				Respawn ();
			} else {
				respawnTimer += Time.deltaTime;
			}
		}

		//#################################
		//movement

		//Debug.Log ("PlayerController " + joystickIndex + ": Update: acceleration=" + movement_acceleration.ToString ());

		Vector3 _totalAcceleration = new Vector3 ();

		//normal movement
		if (!isDashing) {
			dashCooldown = Mathf.Clamp (dashCooldown - Time.deltaTime, 0, PlayerConstants.DASH_COOLDOWN);

			_totalAcceleration += movement_acceleration;
			//Debug.Log ("PlayerController " + joystickIndex + ": Update: movementVelocity=" + movement_velocity.ToString ());
		}
			//dashing movement
			else if (isDashing) {
			if (dashTimer >= PlayerConstants.DASH_DURATION) {
				isDashing = false;
				dashCooldown = PlayerConstants.DASH_COOLDOWN;
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

		if (velocity.normalized.magnitude != 0 && !isDead) {
			//transform.rotation = Quaternion.LookRotation (velocity.normalized);
			transform.forward = velocity.normalized;
		}

		//##################################
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
		//Debug.Log ("PlayerController: OnHandleLeftStick: called");

		if (joystickIndex == this.joystickIndex && !isDead) {
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

		if (joystickIndex == this.joystickIndex && !isDead) {
			if (!xButtonPreviouslyPressed && xButtonCurrentlyPressed && !isDashing && dashCooldown == 0) {
				isDashing = true;
				dashTimer = 0f;
				dash_acceleration = movement_acceleration.normalized * PlayerConstants.DASH_VELOCITY;
			}
		}
	}

	void IInputListener.OnHandleAButton (int joystickIndex, bool pressed)
	{
		aButtonPreviouslyPressed = aButtonCurrentlyPressed;
		aButtonCurrentlyPressed = pressed;

		if (joystickIndex == this.joystickIndex && !isDead) {
			if (!aButtonPreviouslyPressed && aButtonCurrentlyPressed && !isDashing && overlappingItems.Count > 0) {
				DropCurrentItem ();
				PickUpItem ();
				//Debug.Log ("PlayerController: OnHandleAButton: should have picked up item");
			} else if (!aButtonPreviouslyPressed && aButtonCurrentlyPressed && !isDashing) {
				DropCurrentItem ();
				//Debug.Log ("PlayerController: OnHandleAButton: should have dropped item");
			}
		}
	}

	//########################################################################
	// Public Methods
	//########################################################################

	public void Push (Vector3 direction)
	{
		isPushed = true;
		pushTimer = 0f;
		push_acceleration = direction * PlayerConstants.PUSH_VELOCITY;

		DropCurrentItem ();
	}

	//########################################################################
	// Private Methods
	//########################################################################

	private void DropCurrentItem ()
	{
		if (currentItem != null) {
			currentItemGO.SetActive (true);
			currentItem.transform.position = this.transform.position;

			uiCurrentItemText.text = "Item: None";
			currentItem = null;
			currentItemGO = null;
		}
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

	private bool IsAboveVoid ()
	{
		Vector3 _position = transform.position;
		float _radius = GetComponent<CapsuleCollider> ().radius;
		Collider[] _colliders = Physics.OverlapBox (new Vector3 (_position.x, 0f, _position.z), new Vector3 (_radius, _radius, _radius));

		bool isAboveFloor = false;
		bool isAboveVoid = false;

		foreach (Collider _collider in _colliders) {
			if (_collider.tag == "Floor") {
				isAboveFloor = true;
			} else if (_collider.tag == "Void") {
				isAboveVoid = true;
			}
		}

		/*if (joystickIndex == 0) {
			foreach (Collider _collider in _colliders) {
				Debug.Log ("Collider tag=" + _collider.tag);
			}
			Debug.Log ("+++++++++++++++");
		}*/

		if (isAboveVoid && !isAboveFloor) {
			return true;
		}

		return false;
	}

	private void Respawn ()
	{
		isDead = false;
		isDashing = false;
		isPushed = false;

		transform.position = spawnPosition;
		transform.forward = new Vector3 (0, 0, 1);

		movement_acceleration.Set (0, 0, 0);
		velocity.Set (0, 0, 0);
	}
}
