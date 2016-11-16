using UnityEngine;
using System.Collections;

public class MachinePlayerInteraction : MonoBehaviour, IMachineListener
{
	public bool requiresItem;
	public string[] itemCategories;

	private MachineController machineController;

	private bool isActive;

	// Use this for initialization
	void Start ()
	{
		machineController = GetComponent<MachineController> ();

		machineController.AddListener (this);
		isActive = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	void IMachineListener.OnStateChange (MachineController.MachineState state)
	{
		//Debug.Log ("MachinePlayerInteraction: OnStateChange: state=" + state.ToString ());

		if (state == MachineController.MachineState.Active) {
			isActive = true;
		} else {
			isActive = false;
		}
	}

	public void OnStartInteraction (PlayerController player)
	{
		//Debug.Log ("MachinePlayerInteraction: OnStartInteraction: machineName=" + this.gameObject.name + " playerName=" + player.gameObject.name);

		if (isActive) {
			if (requiresItem) {
				if (player.currentItem != null) {
					string[] _playerItemCategories = player.currentItem.itemCategories;
					bool _interactionAlloxed = false;

					foreach (string _allowedCategory in itemCategories) {
						if (CheckForAllowedCategory (_allowedCategory, _playerItemCategories)) {
							_interactionAlloxed = true;
							break;
						}
					}

					if (_interactionAlloxed) {
						machineController.SetState (MachineController.MachineState.Idle);
					}
				}
			} else {
				machineController.SetState (MachineController.MachineState.Idle);
			}
		}
	}

	public void OnEndInteraction (PlayerController player)
	{
		//Debug.Log ("MachinePlayerInteraction: OnEndInteraction: machineName=" + this.gameObject.name + " playerName=" + player.gameObject.name);
	}



	private bool CheckForAllowedCategory (string allowedCategory, string[] playerItemCategories)
	{
		foreach (string _playerItemCategory in playerItemCategories) {
			if (allowedCategory == _playerItemCategory) {
				return true;
			}
		}

		return true;
	}
}
