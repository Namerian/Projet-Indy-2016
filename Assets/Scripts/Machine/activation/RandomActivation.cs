using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class RandomActivation : MonoBehaviour, IMachineListener
{
	public int weight;

	private MachineController machineController;

	void Awake ()
	{

	}

	// Use this for initialization
	void Start ()
	{
		machineController = GetComponent<MachineController> ();
		machineController.AddListener (this);

		Global.MachineActivationManager.AddMachineRandomActivator (this);
	}
	
	// Update is called once per frame
	/*void Update ()
	{
	
	}*/

	public void Activate ()
	{
		if (machineController.currentState == MachineController.MachineState.Idle) {
			machineController.SetState (MachineController.MachineState.Active);
			Global.MachineActivationManager.RemoveMachineRandomActivator (this);
		}
	}

	void IMachineListener.OnStateChange (MachineController.MachineState state)
	{
		//Debug.Log ("RandomActivation: OnStateChange: state=" + state.ToString ());

		if (state == MachineController.MachineState.Idle) {
			Global.MachineActivationManager.AddMachineRandomActivator (this);
		}
	}

}
