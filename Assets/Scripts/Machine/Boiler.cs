﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boiler : IMachine
{
	public float _basePressure = 100;
	public float _pressionGainPerSecond = 1;
	public float _interactionMultiplier = 3f;
	public float _explosionDamage = 40f;
	public int _scorePerSecond = 1;

	private Slider _pressureSlider;
	private CanvasGroup _dangerIconCanvasGroup;

	private float _pressure = 0;
	private float _halfBasePressure;

	private bool _isFirstPlayerInteracting = false;
	private MachineInteractionState _firstPlayerInteraction;
	private float _firstPlayerTimer = 0f;

	private bool _isSecondPlayerInteracting = false;
	private MachineInteractionState _secondPlayerInteraction;
	private float _secondPlayerTimer = 0;

	public override bool IsActive{ get { return true; } }

	void Awake ()
	{
		GameObject pressureSlider = this.transform.Find ("Canvas/Slider").gameObject;
		_pressureSlider = pressureSlider.GetComponent<Slider> ();

		_pressureSlider.value = 0;

		GameObject dangerIcon = this.transform.Find ("Canvas/DangerIcon").gameObject;
		_dangerIconCanvasGroup = dangerIcon.GetComponent<CanvasGroup> ();

		_dangerIconCanvasGroup.alpha = 0;
	}

	// Use this for initialization
	void Start ()
	{
		_halfBasePressure = 0.5f * _basePressure;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (_isFirstPlayerInteracting) {
			if (!_firstPlayerInteraction.interactionUpdated) {
				_isFirstPlayerInteracting = false;
			}

			_firstPlayerInteraction.interactionUpdated = false;
		}

		if (_isSecondPlayerInteracting) {
			if (!_secondPlayerInteraction.interactionUpdated) {
				_isSecondPlayerInteracting = false;
			}

			_secondPlayerInteraction.interactionUpdated = false;
		}

		//================================================================

		bool pressureOverHalf = _pressure > _halfBasePressure;

		// pressure <= 50% AND first player interacting
		if (_pressure <= _halfBasePressure && _isFirstPlayerInteracting) {
			//Debug.Log ("Boiler:Update:pressure < 50% AND firstPlayerInteracting");
			float newPressure = _pressure - (_interactionMultiplier * _pressionGainPerSecond * Time.deltaTime);
			_pressure = Mathf.Clamp (newPressure, 0f, _basePressure);
			_firstPlayerTimer += Time.deltaTime;
		}
		// pressure > 50% AND two players interacting
		else if (_pressure > _halfBasePressure && _isFirstPlayerInteracting && _isSecondPlayerInteracting) {
			float newPressure = _pressure - (_interactionMultiplier * _pressionGainPerSecond * Time.deltaTime);
			_pressure = Mathf.Clamp (newPressure, 0f, _basePressure);
			_firstPlayerTimer += Time.deltaTime;
			_secondPlayerTimer += Time.deltaTime;
		}
		// no player interacting
		else {
			//Debug.Log ("Boiler:Update:no player interacting");
			float newPressure = _pressure + (_interactionMultiplier * _pressionGainPerSecond * Time.deltaTime);
			_pressure = Mathf.Clamp (newPressure, 0f, _basePressure);
		}

		//===============================================================

		_pressureSlider.value = _pressure / _basePressure;

		if (pressureOverHalf && _pressure <= _halfBasePressure) {
			_dangerIconCanvasGroup.alpha = 0;
		} else if (!pressureOverHalf && _pressure > _halfBasePressure) {
			_dangerIconCanvasGroup.alpha = 1;
		}

		//===============================================================

		if (_pressure >= _basePressure) {
			Global.GameController.ApplyDamageToShip (_explosionDamage);

			_isFirstPlayerInteracting = false;
			_isSecondPlayerInteracting = false;

			_pressure = 0;

			_pressureSlider.value = 0;
			_dangerIconCanvasGroup.alpha = 0;
		}

		//===============================================================

		if (_isFirstPlayerInteracting && _firstPlayerTimer >= 1f) {
			_firstPlayerTimer -= 1f;
			_firstPlayerInteraction.player.AddScore (_scorePerSecond);
		}

		if (_isSecondPlayerInteracting && _secondPlayerTimer >= 1f) {
			_secondPlayerTimer -= 1f;
			_secondPlayerInteraction.player.AddScore (_scorePerSecond);
		}
	}

	public override MachineInteractionState Interact (PlayerController player)
	{
		if (_isFirstPlayerInteracting && _firstPlayerInteraction.player == player) {
			_firstPlayerInteraction.interactionUpdated = true;
			return _firstPlayerInteraction;
		} else if (_isSecondPlayerInteracting && _secondPlayerInteraction.player == player) {
			_secondPlayerInteraction.interactionUpdated = true;
			return _secondPlayerInteraction;
		} else if (!_isFirstPlayerInteracting && player.HasItem && player.CurrentItem._itemType == ItemType.wheel) {
			_isFirstPlayerInteracting = true;
			_firstPlayerInteraction = new MachineInteractionState (player, true);
			_firstPlayerTimer = 0f;

			return _firstPlayerInteraction;
		} else if (!_isSecondPlayerInteracting) {
			_isSecondPlayerInteracting = true;
			_secondPlayerInteraction = new MachineInteractionState (player, true);
			_secondPlayerTimer = 0f;

			return _secondPlayerInteraction;
		}

		return new MachineInteractionState (player, false);
	}
}
