using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OilBarrel : IMachine
{
	public float _maxExpansion = 7f;
	public float _expansionRate = 0.5f;
	public float _interactionMultiplier = 3f;
	public int _scorePerSecond = 1;
	public float _explosionDamage = 15;
	public int _activationChance = 20;

	private SpriteRenderer _renderer;
	private CanvasGroup _dangerIconCanvasGroup;

	private bool _isActive = false;

	private bool _isCleaning = false;
	private MachineInteractionState _cleaningInteraction;
	private float _cleaningTimer = 0f;

	public override bool IsActive{ get { return _isActive; } }

	// Use this for initialization
	void Start ()
	{
		_renderer = this.GetComponent<SpriteRenderer> ();

		_dangerIconCanvasGroup = this.transform.parent.Find ("Canvas/DangerIcon").GetComponent<CanvasGroup> ();
		_dangerIconCanvasGroup.alpha = 0;

		Invoke ("RandomActivation", UnityEngine.Random.Range (_activationIntervalMin, _activationIntervalMax));
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!_isActive) {
			return;
		}

		if (_isCleaning) {
			if (!_cleaningInteraction.interactionUpdated) {
				_isCleaning = false;
			}

			_cleaningInteraction.interactionUpdated = false;
		}

		float scale = this.transform.localScale.x;

		if (_isCleaning) {
			float expansion = _expansionRate * _interactionMultiplier * Time.deltaTime;
			scale = Mathf.Clamp (scale - expansion, 1f, _maxExpansion);
			this.transform.localScale = new Vector3 (scale, scale, 1);

			_cleaningTimer += Time.deltaTime;

			if (_cleaningTimer >= 1f) {
				_cleaningTimer -= 1f;
				_cleaningInteraction.player.AddScore (_scorePerSecond);
			}

			if (scale == 1f) {
				Deactivate ();
			}
		} else {
			float expansion = _expansionRate * Time.deltaTime;
			scale = Mathf.Clamp (scale + expansion, 1f, _maxExpansion);
			this.transform.localScale = new Vector3 (scale, scale, 1);

			Bounds bounds = _renderer.bounds;
			RaycastHit2D[] hits = Physics2D.CircleCastAll (bounds.center, bounds.extents.x, Vector2.zero);

			foreach (RaycastHit2D hit in hits) {
				if (bounds.Contains (hit.transform.position)) {
					ILightEmitter emitter = hit.transform.GetComponent<ILightEmitter> ();

					if (emitter != null && emitter.IsEmittingLight ()) {
						Global.GameController.ApplyDamageToShip (_explosionDamage);
						Deactivate ();
					}
				}
			}
		}
	}

	public override MachineInteractionState Interact (PlayerController player)
	{
		if (!_isActive) {
			return new MachineInteractionState (player, false);
		}

		if (_isCleaning && _cleaningInteraction.player == player) {
			_cleaningInteraction.interactionUpdated = true;

			return _cleaningInteraction;
		} else {
			if (_isCleaning || !player.HasItem || player.CurrentItem._itemType != ItemType.mop) {
				return new MachineInteractionState (player, false);
			}

			_isCleaning = true;
			_cleaningTimer = 0f;
			_cleaningInteraction = new MachineInteractionState (player, true);

			return _cleaningInteraction;
		}
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
		_isCleaning = false;

		this.transform.localScale = new Vector3 (1, 1, 1);
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
