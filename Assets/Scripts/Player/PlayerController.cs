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
	private bool bButtonCurrentlyPressed;
	private bool bButtonPreviouslyPressed;

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
	public Item currentItem{ get; private set; }

	private GameObject currentItemGO;
	private List<Item> overlappingItems;

	//Death
	private Vector3 spawnPosition;
	private bool isDead;
	private float respawnTimer;

	//Machines
	private MachinePlayerInteraction currentMachine;
	private bool isInteracting;

	//#############################################################################

	protected abstract void OnStart ();

	protected abstract void OnUpdate ();

	//#############################################################################

	void Awake ()
	{
		characterController = GetComponent<CharacterController> ();

		GameObject _gameControllerObj = GameObject.FindGameObjectWithTag ("GameController");
		gameController = _gameControllerObj.GetComponent<GameController> ();

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

	//######################################################################################################
	// Update
	//######################################################################################################
	
	// Update is called once per frame
	void Update ()
	{
		//####################################################################################
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

		//####################################################################################
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

		//####################################################################################
		//mashine interaction

		bool _interruptInteraction = false;

		MachinePlayerInteraction _nearestInteractableMachine;
		bool _hasHitInteractableMachine = CheckForInteractableMachine (out _nearestInteractableMachine);

		if (isInteracting && (isDashing || isBumped)) {
			_interruptInteraction = true;
		} else if (_hasHitInteractableMachine && isInteracting && _nearestInteractableMachine != currentMachine) {
			_interruptInteraction = true;
		} else if (isInteracting && !_hasHitInteractableMachine) {
			_interruptInteraction = true;
		} else if (bButtonCurrentlyPressed != bButtonPreviouslyPressed) {
			if (isInteracting && bButtonPreviouslyPressed && !bButtonCurrentlyPressed) {
				_interruptInteraction = true;
			} else if (!isInteracting && _hasHitInteractableMachine && !bButtonPreviouslyPressed && bButtonCurrentlyPressed) {
				isInteracting = true;
				currentMachine = _nearestInteractableMachine;
				currentMachine.OnStartInteraction (this);
			}
		}

		if (_interruptInteraction) {
			isInteracting = false;
			currentMachine.OnEndInteraction (this);
			currentMachine = null;
		}

		//####################################################################################
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

		if (joystickIndex == this.joystickIndex && !isDead && !gameController.isPaused) {
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

		if (joystickIndex == this.joystickIndex && !isDead && !gameController.isPaused) {
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

		if (joystickIndex == this.joystickIndex && !isDead && !gameController.isPaused) {
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

	void IInputListener.OnHandleBButton (int joystickIndex, bool pressed)
	{
		bButtonPreviouslyPressed = bButtonCurrentlyPressed;
		bButtonCurrentlyPressed = pressed;
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
			currentItem.OnDrop ();
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

			uiCurrentItemText.text = "Item: " + currentItem.name;

			overlappingItems.Remove (currentItem);
			currentItem.OnPickUp ();
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

		if (isAboveVoid && !isAboveFloor) {
			return true;
		}

		return false;
	}

	private bool CheckForInteractableMachine (out MachinePlayerInteraction interactableMachine)
	{
		Ray _ray = new Ray (this.transform.position - new Vector3 (0f, -0.5f, 0f), this.transform.forward);
		RaycastHit[] _hits = Physics.RaycastAll (_ray, characterController.radius * 2f);

		RaycastHit _nearestMachineHit = new RaycastHit ();
		float _smallestDistance = characterController.radius * 2.1f;
		bool _hasHitMachine = false;

		foreach (RaycastHit hit in _hits) {
			if (hit.collider.tag == "Machine") {
				float _distance = Vector3.Distance (hit.point, this.transform.position);
				if (_distance < _smallestDistance) {
					_smallestDistance = _distance;
					_nearestMachineHit = hit;
					_hasHitMachine = true;
				}
			}
		}

		bool _hasHitInteractableMachine = false;
		interactableMachine = null;
		if (_hasHitMachine) {
			interactableMachine = _nearestMachineHit.collider.GetComponent<MachinePlayerInteraction> ();
			if (!interactableMachine.Equals (null)) {
				_hasHitInteractableMachine = true;
			}
		}

		return _hasHitInteractableMachine;
	}
}
