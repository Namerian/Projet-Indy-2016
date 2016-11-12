using UnityEngine;
using System.Collections;
using System;

public class GameController : MonoBehaviour
{
	public float gameTimer{ get; private set; }

	public float shipHealth{ get; private set; }

	private bool isGameRunning;

	private PlayerController redPlayer;
	private PlayerController greenPlayer;
	private PlayerController bluePlayer;
	private PlayerController yellowPlayer;

	private GameStateUIView gameStateUI;

	//==========================================================================================================
	//
	//==========================================================================================================

	void Awake ()
	{
		//UI
		gameStateUI = GameObject.Find ("UI/InGameUI/GameStateUI").GetComponent<GameStateUIView> ();

		//Players
		redPlayer = GameObject.Find ("PlayerHolder/RedPlayer").GetComponent<PlayerController> ();
		greenPlayer = GameObject.Find ("PlayerHolder/GreenPlayer").GetComponent<PlayerController> ();
		bluePlayer = GameObject.Find ("PlayerHolder/BluePlayer").GetComponent<PlayerController> ();
		yellowPlayer = GameObject.Find ("PlayerHolder/YellowPlayer").GetComponent<PlayerController> ();
	}

	// Use this for initialization
	void Start ()
	{
		redPlayer.Initialize (0);
		greenPlayer.Initialize (1);
		bluePlayer.Initialize (2);
		yellowPlayer.Initialize (3);

		gameTimer = 20f;
		shipHealth = 100f;
		isGameRunning = true;
	}

	// Update is called once per frame
	void Update ()
	{
		if (isGameRunning) {
			if (shipHealth <= 0 || gameTimer <= 0) {
				isGameRunning = false;

				bool gameWon = false;
				if (shipHealth > 0) {
					gameWon = true;
				}
				gameStateUI.ActivateGameResultView (gameWon);
			} else {
				gameTimer = Mathf.Clamp (gameTimer - Time.deltaTime, 0f, float.MaxValue);
			}
		}
	}

	//==========================================================================================================
	//
	//==========================================================================================================

	public bool isPaused{ get { return !isGameRunning; } }

	public void ApplyDamageToShip (float damage)
	{
		if (damage > 0 && shipHealth > 0) {
			shipHealth -= damage;
		}
	}
}
