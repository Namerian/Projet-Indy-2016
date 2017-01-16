using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lamp : IMachine, ILightEmitter
{
	public float _repairTime = 1f;
	public int _repairScore = 15;
	public int _activationChance = 15;

	private CanvasGroup _dangerIconCanvasGroup;

	private bool _isActive = false;

	private bool _isRepairing = false;
	private MachineInteractionState _repairInteraction;
	private float _repairTimer = 0f;

	public override bool IsActive{ get { return _isActive; } }

	// Use this for initialization
	void Start ()
	{
		GameObject dangerIcon = this.transform.Find ("Canvas/DangerIcon").gameObject;
		_dangerIconCanvasGroup = dangerIcon.GetComponent<CanvasGroup> ();
		_dangerIconCanvasGroup.alpha = 0;

		Invoke ("RandomActivation", UnityEngine.Random.Range (_activationIntervalMin, _activationIntervalMax));
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!_isActive) {
			return;
		}
        else if (Global.GameController.IsGameInEndPhase)
        {
            Deactivate();
        }

        if (_isRepairing) {
			if (!_repairInteraction.interactionUpdated) {
				_isRepairing = false;
			}

			_repairInteraction.interactionUpdated = false;
		}

		if (_isRepairing) {
			_repairTimer += Time.deltaTime;
			_repairInteraction.progress = _repairTimer / _repairTime;

			if (_repairInteraction.progress >= 1f) {
				_repairInteraction.progress = 1f;
				_repairInteraction.player.AddScore (_repairScore);

				Deactivate ();

				//Debug.Log ("Planks:Interaction:planks repaired!");
			}
		}
	}

	public override MachineInteractionState Interact (PlayerController player)
	{
		if (!_isActive) {
			return new MachineInteractionState (player, false);
		}

		if (_isRepairing && _repairInteraction.player == player) {
			_repairInteraction.interactionUpdated = true;

			return _repairInteraction;
		} else {
			if (!player.HasItem || player.CurrentItem._itemType != ItemType.torch || _isRepairing) {
				return new MachineInteractionState (player, false);
			}

			_isRepairing = true;
			_repairTimer = 0f;
			_repairInteraction = new MachineInteractionState (player, true);

			return _repairInteraction;
		}
	}

	public bool IsEmittingLight ()
	{
		if (_isActive) {
			return false;
		}

		return true;
	}

	private void Activate ()
	{
		if (_isActive) {
			return;
		}

		_isActive = true;

		_dangerIconCanvasGroup.alpha = 1;
	}

	private void Deactivate ()
	{
		_isActive = false;
		_isRepairing = false;

		_dangerIconCanvasGroup.alpha = 0;

		Invoke ("RandomActivation", UnityEngine.Random.Range (_activationIntervalMin, _activationIntervalMax));
	}

	private void RandomActivation ()
	{
		if (_isActive || Global.GameController.IsPaused) {
			return;
		}

		int diceRoll = UnityEngine.Random.Range (1, 100);

		if (diceRoll <= _activationChance) {
			Activate ();
		} else {
			Invoke ("RandomActivation", UnityEngine.Random.Range (_activationIntervalMin, _activationIntervalMax));
		}
	}
}
