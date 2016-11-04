using UnityEngine;
using System.Collections;

public class PlayerConstants : MonoBehaviour
{
	[Header ("Movement")]
	public float movement_max_acceleration;
	public float movement_max_velocity;
	public float movement_friction;

	[Header ("Dash")]
	public float dash_duration;
	public float dash_acceleration;
	public float dash_cooldown;

	[Header ("Bump")]
	public float bump_duration;
	public float bump_acceleration;

	[Header ("Death")]
	public float respawn_cooldown;
	public float fall_acceleration;

	//###########################################################

	public static float MOVEMENT_MAX_ACCELERATION{ get { return instance.movement_max_acceleration; } }

	public static float MOVEMENT_MAX_VELOCITY{ get { return instance.movement_max_velocity; } }

	public static float MOVEMENT_FRICTION{ get { return instance.movement_friction; } }

	public static float DASH_DURATION{ get { return instance.dash_duration; } }

	public static float DASH_VELOCITY{ get { return instance.dash_acceleration; } }

	public static float DASH_COOLDOWN{ get { return instance.dash_cooldown; } }

	public static float BUMP_DURATION{ get { return instance.bump_duration; } }

	public static float BUMP_VELOCITY{ get { return instance.bump_acceleration; } }

	public static float RESPAWN_COOLDOWN{ get { return instance.respawn_cooldown; } }

	public static float FALL_ACCELERATION{ get { return instance.fall_acceleration; } }

	//###########################################################

	private static PlayerConstants instance;

	/*public static PlayerConstants Instance {
		get{ return instance; }
	}*/

	// Use this for initialization
	void Start ()
	{
		if (instance == null) {
			instance = this;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}
