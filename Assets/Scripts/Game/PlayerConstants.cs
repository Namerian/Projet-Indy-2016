using UnityEngine;
using System.Collections;

public class PlayerConstants : MonoBehaviour
{
	public float normal_speed;

	public float dash_duration;
	public float dash_speed;

	public float push_duration;
	public float push_speed;

	//###########################################################

	private static PlayerConstants instance;

	public static PlayerConstants Instance {
		get{ return instance; }
	}

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
