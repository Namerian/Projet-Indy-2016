using UnityEngine;
using System.Collections;

public class RandomActivation : MonoBehaviour, IMachineListener
{
	public int weight;

	private GameController gameController;
	private MachineController machineController;

	// Use this for initialization
	void Start ()
	{
		gameController = GameObject.FindWithTag ("GameController").GetComponent<GameController> ();
		machineController = GetComponent<MachineController> ();

		gameController.AddMachineRandomActivator (this);
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void Activate ()
	{
		if (machineController.currentState == MachineController.MachineState.Idle) {
			machineController.SetState (MachineController.MachineState.Active);
			gameController.RemoveMachineRandomActivator (this);
		}
	}

	public void OnStateChange (MachineController.MachineState state)
	{
		if (state == MachineController.MachineState.Idle) {
			gameController.AddMachineRandomActivator (this);
		}
	}

}
