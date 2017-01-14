using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
	public float _gameTimer = 60f;

	public float _shipHealth = 100f;

	private bool _isGameRunning = false;

	public bool _isPaused { get { return !_isGameRunning; } }

	public Vector3 WindForce { get; set; }

	//=============================================

	private GameStateUIView _gameStateUI;

	//==========================================================================================================
	//
	//==========================================================================================================

	void Awake ()
	{
		Global.GameController = this;
	}

	// Use this for initialization
	void Start ()
	{
		//UI
		_gameStateUI = GameObject.Find ("UI/InGameUI/GameStateUI").GetComponent<GameStateUIView> ();

		//
		Global.LevelSelectionMenu.ToggleVisibility (true);
	}

	// Update is called once per frame
	void Update ()
	{
		if (_isGameRunning) {
			if (_shipHealth <= 0 || _gameTimer <= 0) {
				_isGameRunning = false;

				bool gameWon = false;
				if (_shipHealth > 0) {
					gameWon = true;
				}
				_gameStateUI.ActivateGameResultView (gameWon);
			} else {
				//update game timer
				_gameTimer = Mathf.Clamp (_gameTimer - Time.deltaTime, 0f, float.MaxValue);
			}
		}
	}

	//==========================================================================================================
	//
	//==========================================================================================================

	public void ApplyDamageToShip (float damage)
	{
		if (damage > 0 && _shipHealth > 0) {
			_shipHealth -= damage;
		}
	}

	//==========================================================================================================
	//
	//==========================================================================================================

	void OnSceneLoaded (Scene scene, LoadSceneMode loadSceneMode)
	{
		Debug.Log ("GameController: OnSceneLoaded: called!");

		Global.MenuCamera.enabled = false;
		Global.LevelCamera.enabled = true;
		//SpawnManager.Instance.CreateAndSpawnPlayers ();

		Event.Instance.SendOnGameStartedEvent ();

		_isGameRunning = true;
	}

	public void LoadLevel (string levelName)
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
		SceneManager.LoadScene ("Scenes/Levels/" + levelName, LoadSceneMode.Additive);

		Global.LevelSelectionMenu.ToggleVisibility (false);
		Global.InGameUI.ToggleVisibility (true);
	}
}
