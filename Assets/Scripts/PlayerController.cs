using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

public enum PlayerName
{
	BluePlayer,
	RedPlayer,
	GreenPlayer,
	PurplePlayer,
	None
}

public class PlayerController : MonoBehaviour, IInputListener
{
	public PlayerName _playerName;
	public int _controllerIndex = 0;

	//
	private GameController _gameController;

	//
	private CircleCollider2D _collider;
	private Rigidbody2D _rigidbody;

	// UI
	private Text _uiCurrentItemText;

	// Input
	private bool _xButtonCurrentlyPressed;
	private bool _xButtonPreviouslyPressed;
	private bool _aButtonCurrentlyPressed;
	private bool _aButtonPreviouslyPressed;
	private bool _bButtonCurrentlyPressed;
	private bool _bButtonPreviouslyPressed;

	//
	private bool _isDashing;
	private float _dashTimer;
	private float _dashCooldown;

	//
	private bool _isBumped;
	private float _bumpTimer;
	private PlayerName _bumpingPlayer = PlayerName.None;

	private Vector3 _movementAcceleration = new Vector3 ();
	private Vector3 _dashAcceleration = new Vector3 ();
	private Vector3 _bumpAcceleration = new Vector3 ();

	private Vector3 _velocity = new Vector3 ();

	// Items
	public Item CurrentItem{ get; private set; }

	private GameObject _currentItemGO;
	private List<Item> _overlappingItems = new List<Item> ();

	//Death
	private Vector3 _spawnPosition;
	private bool _isDead;
	private float _respawnTimer;

	private List<GameObject> _floorColliders = new List<GameObject> ();

	//Machines
	//private MachinePlayerInteraction _currentMachine;
	private bool _isInteracting;

	//#############################################################################

	void Awake ()
	{
		_gameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();

		_collider = this.GetComponent<CircleCollider2D> ();
		_rigidbody = this.GetComponent<Rigidbody2D> ();
	}

	// Use this for initialization
	void Start ()
	{
		_gameController.GetComponent<InputHandler> ().AddInputListener (this, InputHandler.JOYSTICK_NAMES [_controllerIndex]);

		_spawnPosition = transform.position;

		//
		string playerUiPath = "";

		switch (_playerName) {
		case PlayerName.BluePlayer:
			playerUiPath = "UI/InGameUI/BluePlayerUI/CurrentItemText";
			break;
		case PlayerName.GreenPlayer:
			playerUiPath = "UI/InGameUI/GreenPlayerUI/CurrentItemText";
			break;
		case PlayerName.RedPlayer:
			playerUiPath = "UI/InGameUI/RedPlayerUI/CurrentItemText";
			break;
		case PlayerName.PurplePlayer:
			playerUiPath = "UI/InGameUI/YellowPlayerUI/CurrentItemText";
			break;
		}

		_uiCurrentItemText = GameObject.Find (playerUiPath).GetComponent<Text> ();
		_uiCurrentItemText.text = "Item: None";
	}

	//######################################################################################################
	// Update
	//######################################################################################################
	
	// Update is called once per frame
	void Update ()
	{
		//####################################################################################
		//death
		if (!_isDead && !_isDashing) {
			if (_floorColliders.Count == 0) {
				_isDead = true;
				_respawnTimer = 0f;

				_isDashing = false;
				_isBumped = false;

				_movementAcceleration.Set (0, 0, 0);
			}
		} else if (_isDead) {
			if (_respawnTimer >= PlayerConstants.RESPAWN_COOLDOWN) {
				_isDead = false;

				transform.position = _spawnPosition;
				transform.up = new Vector3 (0, 1, 0);
				this.transform.localScale = new Vector3 (1, 1, 1);

				_movementAcceleration.Set (0, 0, 0);
				_velocity.Set (0, 0, 0);
			} else {
				_respawnTimer += Time.deltaTime;

				Vector3 currentScale = this.transform.localScale;
				currentScale.x -= PlayerConstants.RESPAWN_COOLDOWN * 0.8f * Time.deltaTime;
				currentScale.y -= PlayerConstants.RESPAWN_COOLDOWN * 0.8f * Time.deltaTime;
				this.transform.localScale = currentScale;
			}
		}

		//####################################################################################
		//movement

		//Debug.Log ("PlayerController " + joystickIndex + ": Update: acceleration=" + movement_acceleration.ToString ());

		Vector3 totalAcceleration = new Vector3 ();

		//normal movement
		if (!_isDashing) {
			_dashCooldown = Mathf.Clamp (_dashCooldown - Time.deltaTime, 0, PlayerConstants.DASH_COOLDOWN);

			totalAcceleration += _movementAcceleration;
			//Debug.Log ("PlayerController " + joystickIndex + ": Update: movementVelocity=" + movement_velocity.ToString ());
		}
		//dashing movement
		else if (_isDashing) {
			if (_dashTimer >= PlayerConstants.DASH_DURATION) {
				_isDashing = false;
				_dashCooldown = PlayerConstants.DASH_COOLDOWN;
			} else {
				_dashTimer += Time.deltaTime;
				totalAcceleration += _dashAcceleration;
				//Debug.Log ("PlayerController " + joystickIndex + ": Update: dashVelocity=" + dash_velocity.ToString ());
			}
		}

		if (_isBumped) {
			if (_bumpTimer >= PlayerConstants.BUMP_DURATION) {
				_isBumped = false;
				_bumpingPlayer = PlayerName.None;
			} else {
				_bumpTimer += Time.deltaTime;
				totalAcceleration += _bumpAcceleration;
			}
		}

		_velocity += totalAcceleration - PlayerConstants.MOVEMENT_FRICTION * _velocity;
		_velocity = Vector3.ClampMagnitude (_velocity, PlayerConstants.MOVEMENT_MAX_VELOCITY);

		//this.transform.position = this.transform.position + (_velocity * Time.deltaTime);
		_rigidbody.velocity = _velocity;

		if (_velocity.normalized.magnitude != 0 && !_isDead) {
			//transform.rotation = Quaternion.LookRotation (velocity.normalized);

			//transform.forward = _velocity.normalized;
			transform.up = _velocity.normalized;
		}

		//####################################################################################
		//mashine interaction

		/*bool interruptInteraction = false;

		MachinePlayerInteraction nearestInteractableMachine;
		bool hasHitInteractableMachine = CheckForInteractableMachine (out nearestInteractableMachine);

		if (_isInteracting && (_isDashing || _isBumped)) {
			interruptInteraction = true;
		} else if (hasHitInteractableMachine && _isInteracting && nearestInteractableMachine != _currentMachine) {
			interruptInteraction = true;
		} else if (_isInteracting && !hasHitInteractableMachine) {
			interruptInteraction = true;
		} else if (_bButtonCurrentlyPressed != _bButtonPreviouslyPressed) {
			if (_isInteracting && _bButtonPreviouslyPressed && !_bButtonCurrentlyPressed) {
				interruptInteraction = true;
			} else if (!_isInteracting && hasHitInteractableMachine && !_bButtonPreviouslyPressed && _bButtonCurrentlyPressed) {
				_isInteracting = true;
				_currentMachine = nearestInteractableMachine;
				_currentMachine.OnStartInteraction (this);
			}
		}

		if (interruptInteraction) {
			_isInteracting = false;
			_currentMachine.OnEndInteraction (this);
			_currentMachine = null;
		}*/
	}

	//########################################################################
	// Collison
	//########################################################################

	/*void OnControllerColliderHit (ControllerColliderHit hit)
	{
		if (!_isDead) {
			if (hit.gameObject.tag == "Player" && (_isDashing || _isBumped)) {
				PlayerController otherPlayer = hit.gameObject.GetComponent<PlayerController> ();
				Vector3 pushDirection = otherPlayer.transform.position - this.transform.position;
				pushDirection.Normalize ();
				otherPlayer.Bump (pushDirection);

				_isDashing = false;
			}
		}
	}*/

	void OnCollisionEnter2D (Collision2D collision)
	{
		if (!_isDead && collision.gameObject.tag == "Player" && (_isDashing || _isBumped)) {
			//Debug.Log (_playerName + " collision pos=" + this.transform.position + " forward=" + this.transform.up);
			RaycastHit2D[] rayHits = Physics2D.RaycastAll (this.transform.position + this.transform.up * _collider.radius, this.transform.up, 0.1f);

			foreach (RaycastHit2D rayHit in rayHits) {
				if (rayHit.collider.gameObject == collision.gameObject) {
					PlayerController otherPlayer = collision.gameObject.GetComponent<PlayerController> ();

					Debug.Log (_playerName + " has hit " + otherPlayer._playerName);

					if (otherPlayer._playerName != _bumpingPlayer) {
						Vector3 pushDirection = otherPlayer.transform.position - this.transform.position;
						pushDirection.Normalize ();

						otherPlayer.Bump (pushDirection, _playerName);

						_isDashing = false;
					}
				}
			}
		}
	}

	/*void OnCollisionExit2D (Collision2D collision)
	{
		
	}*/

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.tag == "Item") {
			Item item = other.GetComponent<Item> ();

			if (!_overlappingItems.Contains (item)) {
				_overlappingItems.Add (item);
				//Debug.Log ("PlayerController: OnTriggerEnter: overlapping with " + _item.itemName);
			}
		} else if (other.tag == "Floor") {
			if (!_floorColliders.Contains (other.gameObject)) {
				_floorColliders.Add (other.gameObject);
			}
		}
	}

	void OnTriggerExit2D (Collider2D other)
	{
		if (other.tag == "Item") {
			Item item = other.GetComponent<Item> ();

			if (_overlappingItems.Contains (item)) {
				_overlappingItems.Remove (item);
			}
		} else if (other.tag == "Floor") {
			_floorColliders.Remove (other.gameObject);
		}
	}

	//########################################################################
	// Input
	//########################################################################

	void IInputListener.OnHandleLeftStick (int joystickIndex, Vector2 stickState)
	{
		//Debug.Log ("PlayerController: OnHandleLeftStick: called");

		if (joystickIndex == _controllerIndex && !_isDead && !_gameController._isPaused) {
			//Debug.Log ("PlayerController " + joystickIndex + ": OnHandleLeftStick: called");

			float force = Mathf.Clamp (stickState.magnitude, 0f, 1f) * PlayerConstants.MOVEMENT_MAX_ACCELERATION;
			//Debug.Log ("PlayerController " + joystickIndex + ": stickstate=" + stickState.ToString ());
			//Debug.Log ("PlayerController " + joystickIndex + ": force=" + _force);

			_movementAcceleration.Set (0, 0, 0);

			_movementAcceleration.x = stickState.x;
			_movementAcceleration.y = stickState.y;

			_movementAcceleration.Normalize ();
			_movementAcceleration *= force;
		}
	}

	void IInputListener.OnHandleXButton (int joystickIndex, bool pressed)
	{
		_xButtonPreviouslyPressed = _xButtonCurrentlyPressed;
		_xButtonCurrentlyPressed = pressed;

		if (joystickIndex == _controllerIndex && !_isDead && !_gameController._isPaused) {
			if (!_xButtonPreviouslyPressed && _xButtonCurrentlyPressed && !_isDashing && _dashCooldown == 0) {
				_isDashing = true;
				_dashTimer = 0f;
				_dashAcceleration = _movementAcceleration.normalized * PlayerConstants.DASH_VELOCITY;
			}
		}
	}

	void IInputListener.OnHandleAButton (int joystickIndex, bool pressed)
	{
		_aButtonPreviouslyPressed = _aButtonCurrentlyPressed;
		_aButtonCurrentlyPressed = pressed;

		if (joystickIndex == _controllerIndex && !_isDead && !_gameController._isPaused) {
			if (!_aButtonPreviouslyPressed && _aButtonCurrentlyPressed && !_isDashing && _overlappingItems.Count > 0) {
				DropCurrentItem ();
				PickUpItem ();
				//Debug.Log ("PlayerController: OnHandleAButton: should have picked up item");
			} else if (!_aButtonPreviouslyPressed && _aButtonCurrentlyPressed && !_isDashing) {
				DropCurrentItem ();
				//Debug.Log ("PlayerController: OnHandleAButton: should have dropped item");
			}
		}
	}

	void IInputListener.OnHandleBButton (int joystickIndex, bool pressed)
	{
		_bButtonPreviouslyPressed = _bButtonCurrentlyPressed;
		_bButtonCurrentlyPressed = pressed;
	}

	//########################################################################
	// Public Methods
	//########################################################################

	public void Bump (Vector3 direction, PlayerName bumpingPlayer)
	{
		_isBumped = true;
		_bumpTimer = 0f;
		_bumpingPlayer = bumpingPlayer;

		_bumpAcceleration = direction * PlayerConstants.BUMP_VELOCITY;

		DropCurrentItem ();
	}
		
	//########################################################################
	// Private Methods
	//########################################################################

	private void DropCurrentItem ()
	{
		if (CurrentItem != null) {
			CurrentItem.OnDrop ();
			CurrentItem.transform.position = this.transform.position;

			_uiCurrentItemText.text = "Item: None";
			CurrentItem = null;
			_currentItemGO = null;
		}
	}

	private void PickUpItem ()
	{
		if (CurrentItem == null) {
			CurrentItem = _overlappingItems [0];
			_currentItemGO = CurrentItem.gameObject;

			_uiCurrentItemText.text = "Item: " + CurrentItem.name;

			_overlappingItems.Remove (CurrentItem);
			CurrentItem.OnPickUp ();
		}
	}

	private bool IsAboveVoid ()
	{
		if (_floorColliders.Count == 0) {
			return true;
		}

		return false;

		/*Vector3 position = transform.position;
		float radius = _collider.radius;
		Collider[] colliders = Physics.OverlapBox (new Vector3 (position.x, 0f, position.z), new Vector3 (radius, radius, radius));

		bool isAboveFloor = false;
		bool isAboveVoid = false;

		foreach (Collider _collider in colliders) {
			if (_collider.tag == "Floor") {
				isAboveFloor = true;
			} else if (_collider.tag == "Void") {
				isAboveVoid = true;
			}
		}

		if (isAboveVoid && !isAboveFloor) {
			return true;
		}

		return false;*/
	}

	/*private bool CheckForInteractableMachine (out MachinePlayerInteraction interactableMachine)
	{
		Ray ray = new Ray (this.transform.position - new Vector3 (0f, -0.5f, 0f), this.transform.forward);
		RaycastHit[] hits = Physics.RaycastAll (ray, _collider.radius * 2f);

		RaycastHit nearestMachineHit = new RaycastHit ();
		float smallestDistance = _collider.radius * 2.1f;
		bool hasHitMachine = false;

		foreach (RaycastHit hit in hits) {
			if (hit.collider.tag == "Machine") {
				float distance = Vector3.Distance (hit.point, this.transform.position);
				if (distance < smallestDistance) {
					smallestDistance = distance;
					nearestMachineHit = hit;
					hasHitMachine = true;
				}
			}
		}

		bool hasHitInteractableMachine = false;
		interactableMachine = null;
		if (hasHitMachine) {
			interactableMachine = nearestMachineHit.collider.GetComponent<MachinePlayerInteraction> ();
			if (!interactableMachine.Equals (null)) {
				hasHitInteractableMachine = true;
			}
		}

		return hasHitInteractableMachine;
	}*/
}
