using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
	public static GameController Instance { get; private set; }

	//=============================================

	public float gameTimer { get; private set; }

	public float shipHealth { get; private set; }

	private bool isGameRunning;

	public bool isPaused { get { return !isGameRunning; } }

	//=============================================

	private GameStateUIView gameStateUI;

	//==========================================================================================================
	//
	//==========================================================================================================

	void Awake ()
	{
		//
		gameTimer = 40f;
		shipHealth = 100f;
		isGameRunning = false;

		//
		Global.GameController = this;
	}

	// Use this for initialization
	void Start ()
	{
		//UI
		gameStateUI = GameObject.Find ("UI/InGameUI/GameStateUI").GetComponent<GameStateUIView> ();

		//
		Global.LevelSelectionMenu.ToggleVisibility (true);
	}

	// Update is called once per frame
	void Update ()
	{
		if (isGameRunning) {
			if (shipHealth <= 0 || gameTimer <= 0) {
				isGameRunning = false;

				bool _gameWon = false;
				if (shipHealth > 0) {
					_gameWon = true;
				}
				gameStateUI.ActivateGameResultView (_gameWon);
			} else {
				//update game timer
				gameTimer = Mathf.Clamp (gameTimer - Time.deltaTime, 0f, float.MaxValue);
			}
		}
	}

	//==========================================================================================================
	//
	//==========================================================================================================

	public void ApplyDamageToShip (float damage)
	{
		if (damage > 0 && shipHealth > 0) {
			shipHealth -= damage;
		}
	}

	void OnSceneLoaded (Scene scene, LoadSceneMode loadSceneMode)
	{
		Debug.Log ("GameController: OnSceneLoaded: called!");

		Global.MenuCamera.enabled = false;
		Global.LevelCamera.enabled = true;
		//SpawnManager.Instance.CreateAndSpawnPlayers ();

		Event.Instance.SendOnGameStartedEvent ();

		isGameRunning = true;
	}

	public void LoadLevel (string levelName)
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
		SceneManager.LoadScene ("Scenes/Levels/" + levelName, LoadSceneMode.Additive);

		Global.LevelSelectionMenu.ToggleVisibility (false);
		Global.InGameUI.ToggleVisibility (true);
	}
}
