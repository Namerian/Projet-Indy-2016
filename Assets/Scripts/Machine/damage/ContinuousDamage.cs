using UnityEngine;
using System.Collections;

public class ContinuousDamage : MonoBehaviour, IMachineListener
{
	public int damagePerSecond;

	private GameController gameController;
	private MachineController machineController;

	private bool isActive;
	private float timer;

	// Use this for initialization
	void Start ()
	{
		gameController = GameObject.FindWithTag ("GameController").GetComponent<GameController> ();
		machineController = GetComponent<MachineController> ();

		machineController.AddListener (this);
		isActive = false;
		timer = 0f;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (isActive && !gameController.isPaused) {
			if (timer >= 1) {
				timer -= 1;
				gameController.ApplyDamageToShip (damagePerSecond);
			}

			timer += Time.deltaTime;
		}
	}

	public void OnStateChange (MachineController.MachineState state)
	{
		if (state == MachineController.MachineState.Active) {
			isActive = true;
		} else {
			isActive = false;
		}
	}

}
