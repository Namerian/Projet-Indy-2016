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
	private bool isBumped;


	private float dashTimer;
	private float bumpTimer;
	private float dashCooldown;

	private Vector3 movementAcceleration;
	private Vector3 dashAcceleration;
	private Vector3 bumpAcceleration;

	private Vector3 velocity;

	// Items
	private Item currentItem;
	private GameObject currentItemGO;
	private List<Item> overlappingItems;

	//Death
	private Vector3 spawnPosition;
	private bool isDead;
	private float respawnTimer;

	//#############################################################################

	protected abstract void OnStart ();

	protected abstract void OnUpdate ();

	//#############################################################################

	void Awake ()
	{
        characterController = GetComponent<CharacterController>();

        GameObject _gameControllerObj = GameObject.FindGameObjectWithTag("GameController");
        gameController = _gameControllerObj.GetComponent<GameController>();

        movementAcceleration = new Vector3 ();
		velocity = new Vector3 ();
		dashAcceleration = new Vector3 ();
		bumpAcceleration = new Vector3 ();

		currentItem = null;
		overlappingItems = new List<Item> ();
	}

	// Use this for initialization
	void Start ()
	{
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
				isBumped = false;

				movementAcceleration.Set (0, -PlayerConstants.FALL_ACCELERATION, 0);
			}
		} else if (isDead) {
			if (respawnTimer >= PlayerConstants.RESPAWN_COOLDOWN) {
				isDead = false;

				transform.position = spawnPosition;
				transform.forward = new Vector3 (0, 0, 1);

				movementAcceleration.Set (0, 0, 0);
				velocity.Set (0, 0, 0);
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

			_totalAcceleration += movementAcceleration;
			//Debug.Log ("PlayerController " + joystickIndex + ": Update: movementVelocity=" + movement_velocity.ToString ());
		}
			//dashing movement
			else if (isDashing) {
			if (dashTimer >= PlayerConstants.DASH_DURATION) {
				isDashing = false;
				dashCooldown = PlayerConstants.DASH_COOLDOWN;
			} else {
				dashTimer += Time.deltaTime;
				_totalAcceleration += dashAcceleration;
				//Debug.Log ("PlayerController " + joystickIndex + ": Update: dashVelocity=" + dash_velocity.ToString ());
			}
		}

		if (isBumped) {
			if (bumpTimer >= PlayerConstants.BUMP_DURATION) {
				isBumped = false;
			} else {
				bumpTimer += Time.deltaTime;
				_totalAcceleration += bumpAcceleration;
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
		if (!isDead) {
			if (hit.gameObject.tag == "Player" && (isDashing || isBumped)) {
				PlayerController _otherPlayer = hit.gameObject.GetComponent<PlayerController> ();
				Vector3 _pushDirection = _otherPlayer.transform.position - this.transform.position;
				_pushDirection.Normalize ();
				_otherPlayer.Bump (_pushDirection);

				isDashing = false;
			}
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

			movementAcceleration.Set (0, 0, 0);

			movementAcceleration.x = stickState.x;
			movementAcceleration.z = stickState.y;

			movementAcceleration.Normalize ();
			movementAcceleration *= _force;
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
				dashAcceleration = movementAcceleration.normalized * PlayerConstants.DASH_VELOCITY;
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

	public void Bump (Vector3 direction)
	{
		isBumped = true;
		bumpTimer = 0f;
		bumpAcceleration = direction * PlayerConstants.BUMP_VELOCITY;

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
}
