using UnityEngine;
using System.Collections;

public class Global
{
	// GameController

	public static GameController GameController{ get; set; }

	// UI
	public static LevelSelectionMenu LevelSelectionMenu{ get; set; }

	public static InGameUI InGameUI{ get; set; }

	// Cameras

	public static Camera MenuCamera{ get; set; }

	public static Camera LevelCamera{ get; set; }

	// Managers

	//public static MachineActivationManager MachineActivationManager{ get; set; }
}
