using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class RandomActivation : MonoBehaviour, IMachineListener
{
	public int weight;

	private GameController gameController;
	private MachineController machineController;

    void Awake()
    {

    }

	// Use this for initialization
	void Start ()
	{
		gameController = GameObject.FindWithTag ("GameController").GetComponent<GameController> ();
		machineController = GetComponent<MachineController> ();

		gameController.AddMachineRandomActivator (this);
		machineController.AddListener (this);
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

	void IMachineListener.OnStateChange (MachineController.MachineState state)
	{
		//Debug.Log ("RandomActivation: OnStateChange: state=" + state.ToString ());

		if (state == MachineController.MachineState.Idle) {
			gameController.AddMachineRandomActivator (this);
		}
	}

}
