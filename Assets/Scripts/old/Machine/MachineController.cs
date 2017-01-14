/*using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MachineController : MonoBehaviour
{
	/*public enum MachineState
	{
		//waiting to be activated
		Idle,
		//active: does damage and can be repaired
		Active,
		//is idle but can not be activated again
		Destroyed
	}

	public MachineState currentState{ get; private set; }

	private GameController gameController;

	private bool isStarted;
	private List<IMachineListener> listeners;

	//==========================================================================
	//
	//==========================================================================

	void Awake ()
	{
		currentState = MachineState.Idle;
		listeners = new List<IMachineListener> ();
	}

	// Use this for initialization
	void Start ()
	{
		gameController = GameObject.FindWithTag ("GameController").GetComponent<GameController> ();

		isStarted = true;
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	//==========================================================================
	//
	//==========================================================================

	public void AddListener (IMachineListener listener)
	{
		if (!listeners.Contains (listener)) {
			listeners.Add (listener);
		}
	}

	//==========================================================================
	//
	//==========================================================================

	public void SetState (MachineState state)
	{
		if (isStarted && !gameController._isPaused) {
			if (state != currentState) {
				currentState = state;
				OnStateChange ();
			}
		}
	}

	private void OnStateChange ()
	{
		foreach (IMachineListener listener in listeners) {
			listener.OnStateChange (currentState);
		}
	}
}*/
