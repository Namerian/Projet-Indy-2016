using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MachineInteractionState
{
	public PlayerController player;
	public bool isLegal;
	public float progress;
	public bool interactionUpdated;

	public MachineInteractionState (PlayerController player, bool isLegal)
	{
		this.player = player;
		this.isLegal = isLegal;
		this.progress = 0;
		this.interactionUpdated = true;
	}
}

public abstract class IMachine : MonoBehaviour
{
	protected const float _activationIntervalMin = 2f;
	protected const float _activationIntervalMax = 4f;

	public abstract bool IsActive{ get; }

	public abstract MachineInteractionState Interact (PlayerController player);
}