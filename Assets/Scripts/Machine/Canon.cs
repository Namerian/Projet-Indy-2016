﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Canon : IMachine
{
	public float _activeTime = 15f;
	public float _loadingTime = 1f;

	private CanvasGroup _dangerIconCanvasGroup;
	private CanvasGroup _timerCircleCanvasGroup;
	private Image _timerCircleImage;

	private bool _isActive = false;
	private float _activeTimer = 0f;

	private bool _ballLoaded = false;
	private bool _isLoading = false;
	private float _loadingTimer = 0;
	private MachineInteractionState _loadingInteraction;

	public override bool IsActive{ get { return _isActive; } }

	void Awake ()
	{
		GameObject dangerIcon = this.transform.Find ("Canvas/DangerIcon").gameObject;
		_dangerIconCanvasGroup = dangerIcon.GetComponent<CanvasGroup> ();

		_dangerIconCanvasGroup.alpha = 0;

		GameObject timerCircle = this.transform.Find ("Canvas/TimerCircle").gameObject;
		_timerCircleCanvasGroup = timerCircle.GetComponent<CanvasGroup> ();
		_timerCircleImage = timerCircle.GetComponent<Image> ();

		_timerCircleCanvasGroup.alpha = 0;
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

		if (_activeTimer >= _activeTime) {
			Deactivate ();
		} else {
			_activeTimer += Time.deltaTime;

			_timerCircleImage.fillAmount = _activeTimer / _activeTime;
		}

		if (_isLoading) {
			if (!_loadingInteraction.interactionUpdated) {
				_isLoading = false;
			}

			_loadingInteraction.interactionUpdated = false;
		}
	}

	public override MachineInteractionState Interact (PlayerController player)
	{
		if (!_isActive) {
			return new MachineInteractionState (player, false);
		}

		if (_ballLoaded) {
			if (!player.HasItem || player.CurrentItem._itemType != ItemType.torch) {
				return new MachineInteractionState (player, false);
			}
			Debug.Log ("Canon:Interact:canon fired");

			Deactivate ();

			MachineInteractionState result = new MachineInteractionState (player, true);
			result.progress = 1;
			return result;
		} else if (_isLoading && _loadingInteraction.player == player) {
			_loadingTimer += Time.deltaTime;
			_loadingInteraction.progress = _loadingTimer / _loadingTime;
			_loadingInteraction.interactionUpdated = true;

			if (_loadingInteraction.progress >= 1) {
				_loadingInteraction.progress = 1;
				_ballLoaded = true;
				player.DestroyCurrentItem ();
				Debug.Log ("Canon:Interact:ball loaded");
			}

			return _loadingInteraction;
		} else {
			if (!player.HasItem || player.CurrentItem._itemType != ItemType.canonball || _isLoading) {
				return new MachineInteractionState (player, false);
			}

			_isLoading = true;
			_loadingTimer = 0;
			_loadingInteraction = new MachineInteractionState (player, true);

			return _loadingInteraction;
		}
	}

	private void Activate ()
	{
		if (_isActive) {
			return;
		}

		_isActive = true;
		_activeTimer = 0f;

		_dangerIconCanvasGroup.alpha = 1;

		_timerCircleCanvasGroup.alpha = 1;
		_timerCircleImage.fillAmount = 0;
	}

	private void Deactivate ()
	{
		_isActive = false;
		_ballLoaded = false;
		_isLoading = false;

		_dangerIconCanvasGroup.alpha = 0;
		_timerCircleCanvasGroup.alpha = 0;
	}
}
