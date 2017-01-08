using UnityEngine;
using System.Collections;

public class UniqueDamage : MonoBehaviour, IMachineListener
{
	public int damage;
	public float delay;

	private GameController gameController;
	private MachineController machineController;

	private bool isActive;
	private float timer;
	private bool causedDamage;

	// Use this for initialization
	void Start ()
	{
		gameController = GameObject.FindWithTag ("GameController").GetComponent<GameController> ();
		machineController = GetComponent<MachineController> ();

		machineController.AddListener (this);
		isActive = false;
		timer = 0f;
		causedDamage = false;
	}
	
	// Update is called once per frame
	/*void Update ()
	{
		if (isActive && !gameController.isPaused) {
			if (timer >= delay) {
				machineController.SetState (MachineController.MachineState.Idle);
				gameController.ApplyDamageToShip (damage);

				causedDamage = true;
				isActive = false;
			}

			timer += Time.deltaTime;
		}
	}*/

	public void OnStateChange (MachineController.MachineState state)
	{
		if (state == MachineController.MachineState.Active && !causedDamage) {
			isActive = true;
            Invoke("DoDammage", delay);
		} else {
			isActive = false;
		}
	}

    private void DoDammage()
    {
        if (isActive && !gameController.isPaused)
        {
            gameController.ApplyDamageToShip(damage);
            causedDamage = true;
        }
    }
}
