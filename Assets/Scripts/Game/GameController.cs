using UnityEngine;
using System.Collections;


public class GameController : MonoBehaviour
{
	private PlayerController redPlayer;
	private PlayerController greenPlayer;
	private PlayerController bluePlayer;
	private PlayerController yellowPlayer;

	// Use this for initialization
	void Start ()
	{
		redPlayer = GameObject.Find ("PlayerHolder/RedPlayer").GetComponent<PlayerController> ();
		greenPlayer = GameObject.Find ("PlayerHolder/GreenPlayer").GetComponent<PlayerController> ();
		bluePlayer = GameObject.Find ("PlayerHolder/BluePlayer").GetComponent<PlayerController> ();
		yellowPlayer = GameObject.Find ("PlayerHolder/YellowPlayer").GetComponent<PlayerController> ();

		redPlayer.Initialize (0);
		greenPlayer.Initialize (1);
		bluePlayer.Initialize (2);
		yellowPlayer.Initialize (3);
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}
