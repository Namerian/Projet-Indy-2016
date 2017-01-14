using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helm : IMachine
{
	public float _navigationTime = 2f;

	private CanvasGroup _dangerIconCanvasGroup;

	private bool _isActive = false;

	private bool _isNavigatorPresent = false;
	private MachineInteractionState _navigatorInteraction;

	private bool _isHelperPresent = false;
	private MachineInteractionState _helperInteraction;

	private float _navigationTimer = 0;

	public override bool IsActive{ get { return _isActive; } }

	void Awake ()
	{
		GameObject dangerIcon = this.transform.Find ("Canvas/DangerIcon").gameObject;
		_dangerIconCanvasGroup = dangerIcon.GetComponent<CanvasGroup> ();

		_dangerIconCanvasGroup.alpha = 0;
	}

	// Use this for initialization
	void Start ()
	{
		Activate ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!_isActive) {
			return;
		}

		if (_isNavigatorPresent) {
			if (!_navigatorInteraction.interactionUpdated) {
				_isNavigatorPresent = false;
			}

			_navigatorInteraction.interactionUpdated = false;
		}

		if (_isHelperPresent) {
			if (!_helperInteraction.interactionUpdated) {
				_isHelperPresent = false;
			}

			_helperInteraction.interactionUpdated = false;
		}

		if (_isHelperPresent && _isNavigatorPresent) {
			_navigationTimer += Time.deltaTime;
			_navigatorInteraction.progress = _navigationTimer / _navigationTime;
			_helperInteraction.progress = _navigationTimer / _navigationTime;
		} else {
			_navigationTimer = 0;
		}
	}

	public override MachineInteractionState Interact (PlayerController player)
	{
		if (!_isActive) {
			return new MachineInteractionState (player, false);
		}

		if (_isNavigatorPresent && _navigatorInteraction.player == player) {
			return _navigatorInteraction;
		} else if (_isHelperPresent && _helperInteraction.player == player) {
			return _helperInteraction;
		} else if (!_isNavigatorPresent && player.HasItem && player.CurrentItem._itemType == ItemType.wheel) {
			_isNavigatorPresent = true;
			_navigatorInteraction = new MachineInteractionState (player, true);
			return _navigatorInteraction;
		} else if (!_isHelperPresent) {
			_isHelperPresent = true;
			_helperInteraction = new MachineInteractionState (player, true);
			return _helperInteraction;
		}

		return new MachineInteractionState (player, false);
	}

	private void Activate ()
	{
		if (_isActive) {
			return;
		}

		_isActive = true;

		_dangerIconCanvasGroup.alpha = 1;

		Vector3 windForce = new Vector3 ();
		windForce.x = UnityEngine.Random.Range (0f, 1f);
		windForce.y = UnityEngine.Random.Range (0f, 1f);
		Global.GameController.WindForce = windForce;
	}

	private void Deactivate ()
	{
		_isActive = false;

		_dangerIconCanvasGroup.alpha = 0;

		Global.GameController.WindForce.Set (0, 0, 0);
	}
}
