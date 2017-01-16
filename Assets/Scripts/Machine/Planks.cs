using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planks : IMachine, IActivableMachine
{
	public int _requiredButtonPresses = 20;
	public float _damagePerSecond = 2;
	public int _activationChance = 20;
	public int _repairScore = 15;

	private SpriteRenderer _renderer;

	private CanvasGroup _dangerIconCanvasGroup;

	private bool _isActive = false;

	private bool _isRepairing = false;
	private int _currentButtonPresses = 0;
	private MachineInteractionState _repairInteraction;

	public override bool IsActive{ get { return _isActive; } }

	// Use this for initialization
	void Start ()
	{
		_renderer = this.GetComponent<SpriteRenderer> ();
		_renderer.enabled = false;

		GameObject dangerIcon = this.transform.Find ("Canvas/DangerIcon").gameObject;
		_dangerIconCanvasGroup = dangerIcon.GetComponent<CanvasGroup> ();
		_dangerIconCanvasGroup.alpha = 0;

		Global.GameController.RegisterActivableMachine (this);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Global.GameController.IsGameInEndPhase) {
			Deactivate ();
		}
	}

	//==========================================================================================================
	//
	//==========================================================================================================

	public override MachineInteractionState Interact (PlayerController player)
	{
		if (!_isActive) {
			return new MachineInteractionState (player, false);
		}

		if (player.HasItem && player.CurrentItem._itemType == ItemType.hammer) {
			if (_isRepairing && _repairInteraction.player == player) {
				_currentButtonPresses++;
				_repairInteraction.progress = (float)_currentButtonPresses / (float)_requiredButtonPresses;

				Debug.Log ("Planks:Interact:current progress = " + _currentButtonPresses);

				if (_currentButtonPresses == _requiredButtonPresses) {
					_repairInteraction.player.AddScore (_repairScore);
					Deactivate ();
				}

				return _repairInteraction;
			} else {
				_isRepairing = true;
				_currentButtonPresses = 0;
				_repairInteraction = new MachineInteractionState (player, true);

				return _repairInteraction;
			}
		}

		return new MachineInteractionState (player, false);
	}

	public void Activate ()
	{
		if (_isActive) {
			return;
		}

		_isActive = true;

		_renderer.enabled = true;
		_dangerIconCanvasGroup.alpha = 1;

		Invoke ("DoDamage", 1f);
	}

	//==========================================================================================================
	//
	//==========================================================================================================

	private void Deactivate ()
	{
		_isActive = false;
		_isRepairing = false;
		_currentButtonPresses = 0;

		_renderer.enabled = false;
		_dangerIconCanvasGroup.alpha = 0;
	}

	private void DoDamage ()
	{
		if (_isActive) {
			Global.GameController.ApplyDamageToShip (_damagePerSecond);

			Invoke ("DoDamage", 1f);
		}
	}

	/*private void RandomActivation ()
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
	}*/
}
