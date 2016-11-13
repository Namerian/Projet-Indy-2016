using UnityEngine;
using System.Collections;

public class ActiveIcon : MonoBehaviour, IMachineListener
{
	public GameObject iconPrefab;
	public Vector3 relativePosition;

	private MachineController machineController;
	private GameObject uiIconHolder;

	private GameObject icon;

	// Use this for initialization
	void Start ()
	{
		machineController = GetComponent<MachineController> ();
		uiIconHolder = GameObject.Find ("UI/InGameUI/IconHolder");

		machineController.AddListener (this);
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void OnStateChange (MachineController.MachineState state)
	{
		if (state == MachineController.MachineState.Active && iconPrefab != null) {
			icon = (GameObject)Instantiate (iconPrefab);
			icon.transform.SetParent (uiIconHolder.transform, false);

			Vector3 _targetWorldPosition = this.transform.position + relativePosition;
			Vector3 _targetScreenPosition = Camera.main.WorldToScreenPoint (_targetWorldPosition) - uiIconHolder.transform.position;

			icon.transform.Translate (_targetScreenPosition);
		}
	}

}
