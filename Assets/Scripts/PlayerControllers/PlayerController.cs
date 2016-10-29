using UnityEngine;
using System.Collections;

public abstract class PlayerController : MonoBehaviour, IInputListener
{
	private const float DASH_TIME = 0.25f;
	private const float SPEED_DASH = 15;
	private const float SPEED_NORMAL = 5f;

	private CharacterController characterController;
	private GameController gameController;

	private int joystickIndex;

	private Vector3 moveDirection;
	private bool xButtonCurrentlyPressed;
	private bool xButtonPreviouslyPressed;
	private bool isDashing;
	private float dashTimer;

	//#############################################################################

	protected abstract void OnStart ();

	protected abstract void OnUpdate ();

	//#############################################################################

	void Awake ()
	{
		moveDirection = new Vector3 ();

		joystickIndex = 0;
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

		Vector3 _motion = moveDirection * _speed * Time.deltaTime;
		characterController.Move (_motion);

		//
		OnUpdate ();
	}

	void OnControllerColliderHit (ControllerColliderHit hit)
	{
		
	}

	public void OnHandleLeftStick (int joystickIndex, Vector2 stickState)
	{
		if (!isDashing && joystickIndex == this.joystickIndex) {
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
		if (joystickIndex == this.joystickIndex && pressed && !isDashing) {
			isDashing = true;
			dashTimer = 0f;
		}
	}
}
