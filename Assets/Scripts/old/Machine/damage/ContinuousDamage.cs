/*using UnityEngine;
using System.Collections;

public class ContinuousDamage : MonoBehaviour, IMachineListener
{
	/*public int damagePerSecond;

    private GameController gameController;
    private MachineController machineController;

    private bool isActive;

    // Use this for initialization
    void Start()
    {
        gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
        machineController = GetComponent<MachineController>();

        machineController.AddListener(this);
        isActive = false;
    }

    public void OnStateChange(MachineController.MachineState state)
    {
        if (state == MachineController.MachineState.Active)
        {
            isActive = true;
            Invoke("DoDammage", 1f);
        }
        else
        {
            isActive = false;
        }
    }

    private void DoDammage()
    {
        if (gameController._isPaused)
        {
            Invoke("DoDammage", 0f);
        }
        else if (isActive && !gameController._isPaused)
        {
            gameController.ApplyDamageToShip(damagePerSecond);

            Invoke("DoDammage", 1f);
        }
    }
}*/
