using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planks : IMachine
{
	public float _repairTime = 2;

	private SpriteRenderer _renderer;

	private bool _isActive = false;

	private bool _isRepairing = false;
	private float _repairTimer = 0;
	private MachineInteractionState _repairInteraction;

	public override bool IsActive{ get { return _isActive; } }

	void Awake ()
	{
		_renderer = this.GetComponent<SpriteRenderer> ();
		_renderer.enabled = false;
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

		if (_isRepairing) {
			if (!_repairInteraction.interactionUpdated) {
				_isRepairing = false;
			}

			_repairInteraction.interactionUpdated = false;
		}
	}

	public override MachineInteractionState Interact (PlayerController player)
	{
		if (!_isActive) {
			return new MachineInteractionState (player, false);
		}

		if (_isRepairing && _repairInteraction.player == player) {
			_repairTimer += Time.deltaTime;
			_repairInteraction.progress = _repairTimer / _repairTime;
			_repairInteraction.interactionUpdated = true;

			if (_repairInteraction.progress >= 1f) {
				_repairInteraction.progress = 1f;

				Deactivate ();

				Debug.Log ("Planks:Interaction:planks repaired!");
			}

			return _repairInteraction;
		} else {
			if (!player.HasItem || player.CurrentItem._itemType != ItemType.hammer || _isRepairing) {
				return new MachineInteractionState (player, false);
			}

			_isRepairing = true;
			_repairTimer = 0f;
			_repairInteraction = new MachineInteractionState (player, true);

			return _repairInteraction;
		}
	}

	private void Activate ()
	{
		if (_isActive) {
			return;
		}

		_isActive = true;
		_renderer.enabled = true;
	}

	private void Deactivate ()
	{
		_isActive = false;
		_renderer.enabled = false;

		_isRepairing = false;
	}
}
