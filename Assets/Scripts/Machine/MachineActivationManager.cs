using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MachineActivationManager : MonoBehaviour
{
	public float _StartingActivationDelay = 2f;
	public float _MinActivationDelay = 4f;
	public float _MaxActivationDelay = 8f;

	private List<RandomActivation> _machineRandomActivators;

	void Awake ()
	{
		//initialisation
		_machineRandomActivators = new List<RandomActivation> ();

		//
		Global.MachineActivationManager = this;

		//
		Event.Instance.OnGameStartedEvent += new OnGameStartedDelegate (this.OnGameStartedEvent);
	}

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	//=============================================================================
	//
	//=============================================================================

	public void AddMachineRandomActivator (RandomActivation activator)
	{
		if (!_machineRandomActivators.Contains (activator)) {
			_machineRandomActivators.Add (activator);
		}
	}

	public void RemoveMachineRandomActivator (RandomActivation activator)
	{
		_machineRandomActivators.Remove (activator);
	}

	//=============================================================================
	//
	//=============================================================================

	void OnGameStartedEvent ()
	{
		Debug.Log ("MachineActivationManager: OnGameStartedEvent: called!");

		StartCoroutine (ActivationTimer (_StartingActivationDelay));
	}

	//=============================================================================
	//
	//=============================================================================

	private void ActivateMachine ()
	{
		int _sumOfWeights = 0;
		foreach (RandomActivation activator in _machineRandomActivators) {
			_sumOfWeights += activator.weight;
		}

		if (_sumOfWeights > 0) {
			RandomActivation _selectedActivator = null;
			int _currentWeight = 0;
			int _randomNumber = UnityEngine.Random.Range (0, _sumOfWeights);
			foreach (RandomActivation activator in _machineRandomActivators) {
				_currentWeight += activator.weight;
				if (_currentWeight > _randomNumber) {
					_selectedActivator = activator;
					break;
				}
			}

			if (_selectedActivator != null) {
				_selectedActivator.Activate ();
			}
		}

		StartCoroutine (ActivationTimer (UnityEngine.Random.Range (_MinActivationDelay, _MaxActivationDelay)));
	}

	IEnumerator ActivationTimer (float delay)
	{
		while (true) {
			if (delay > 0f) {
				if (!Global.GameController.isPaused) {
					delay -= Time.deltaTime;
				}
				yield return null;
				continue;
			} else {
				ActivateMachine ();
				yield break;
			}
		}
	}
}
