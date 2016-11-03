using UnityEngine;
using System.Collections;

public class PlayerConstants : MonoBehaviour
{
	[Header ("Movement")]
	public float movement_speed;
	public float movement_max_acceleration;
	public float movement_max_velocity;

	[Header ("Dash")]
	public float dash_duration;
	public float dash_velocity;

	[Header ("Push")]
	public float push_duration;
	public float push_velocity;

	//###########################################################

	public static float MOVEMENT_MAX_ACCELERATION{ get { return instance.movement_max_acceleration; } }

	public static float MOVEMENT_MAX_VELOCITY{ get { return instance.movement_max_velocity; } }

	public static float DASH_DURATION{ get { return instance.dash_duration; } }

	public static float DASH_VELOCITY{ get { return instance.dash_velocity; } }

	public static float PUSH_DURATION{ get { return instance.push_duration; } }

	public static float PUSH_VELOCITY{ get { return instance.push_velocity; } }

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
