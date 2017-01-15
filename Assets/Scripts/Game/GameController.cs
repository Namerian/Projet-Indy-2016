using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
	public float _gameTime = 60f;
	public float _baseShipHealth = 100f;

	private GameStateUIView _gameStateUI;

	private bool _isGameRunning = false;
	private bool _isGameInEndPhase = false;
	public float _gameTimer;

	private List<PlayerController> _players;
	private float _shipHealth;

	//=============================================

	public bool IsPaused { get { return !_isGameRunning; } }

	public bool IsGameInEndPhase{ get { return _isGameInEndPhase; } }

	public Vector3 WindForce { get; set; }

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
		_players = new List<PlayerController> ();
		_shipHealth = _baseShipHealth;
		_gameTimer = _gameTime;

		//
		Global.LevelSelectionMenu.ToggleVisibility (true);
	}

	// Update is called once per frame
	void Update ()
	{
		if (_isGameRunning) {

			if (_isGameInEndPhase) {
				
			} else if (_shipHealth <= 0f) {
				_isGameRunning = false;

				List<PlayerController> winners = new List<PlayerController> ();
				List<PlayerController> losers = new List<PlayerController> ();

				foreach (PlayerController player in _players) {
					if (player.HasItem && player.CurrentItem._itemType == ItemType.parachute) {
						winners.Add (player);
					} else {
						losers.Add (player);
					}
				}

				_gameStateUI.ActivateGameResultView (winners, losers);
			} else if (_gameTimer <= 0f) {

				if (_shipHealth > _baseShipHealth * 0.5f) {
					_isGameRunning = false;
					_gameStateUI.ActivateGameResultView (_players, new List<PlayerController> ());
				} else {
					//*****
					_isGameRunning = false;
					_gameStateUI.ActivateGameResultView (new List<PlayerController> (), _players);
				}
			} else {
				_gameTimer = Mathf.Clamp (_gameTimer - Time.deltaTime, 0f, float.MaxValue);
				_gameStateUI.UpdateTime (_gameTimer);
			}
		}
	}

	//==========================================================================================================
	//
	//==========================================================================================================

	public void ApplyDamageToShip (float damage)
	{
		if (_isGameRunning && damage > 0 && _shipHealth > 0) {
			_shipHealth -= damage;
			_gameStateUI.UpdateShipHealth (_shipHealth);
		}
	}

	public void RegisterPlayer (PlayerController player)
	{
		if (!_players.Contains (player)) {
			_players.Add (player);
		}
	}

	public List<PlayerController> GetAllPlayers ()
	{
		return _players;
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
