using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MachineController : MonoBehaviour
{
	public enum MachineState
	{
		Idle,
		Danger,
		Active,
		Destroyed
	}

	public MachineState state{ get; private set; }

	private bool isStarted;

	private List<IMachineListener> listeners;

	//==========================================================================
	//
	//==========================================================================

	void Awake ()
	{
		state = MachineState.Idle;
		listeners = new List<IMachineListener> ();
	}

	// Use this for initialization
	void Start ()
	{
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
		if (isStarted) {

		}
	}
}
