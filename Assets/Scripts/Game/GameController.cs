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
    public float _gameTimer;

    private bool _isGameInEndPhase = false;
    private bool _playerDiedInEndPhase = false;

    private List<PlayerController> _players;
    private float _shipHealth;

    private float _flyingPlankTimer = 3f;
    private int _minSpawnOfPlanks = 2;
    private int _maxSpawnOfPlanks = 4;
    private int _maxNumOfPlanks = 15;
    private List<GameObject> _flyingPlanks;
    private Vector3 _flyingPlankDirection;

    //=============================================

    public bool IsPaused { get { return !_isGameRunning; } }

    public bool IsGameInEndPhase { get { return _isGameInEndPhase; } }

    public Vector3 WindForce { get; set; }

    //==========================================================================================================
    //
    //==========================================================================================================

    void Awake()
    {
        Global.GameController = this;
    }

    // Use this for initialization
    void Start()
    {
        //UI
        _gameStateUI = GameObject.Find("UI/InGameUI/GameStateUI").GetComponent<GameStateUIView>();

        //
        _players = new List<PlayerController>();
        _shipHealth = _baseShipHealth;
        _gameTimer = _gameTime;
        _flyingPlanks = new List<GameObject>();
        _flyingPlankDirection = new Vector3(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), 0);
        _flyingPlankDirection.Normalize();

        //
        Global.LevelSelectionMenu.ToggleVisibility(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (_isGameRunning)
        {

            if (_isGameInEndPhase)
            {
                if (_playerDiedInEndPhase)
                {
                    _gameTimer = Mathf.Clamp(_gameTimer - Time.deltaTime, 0f, float.MaxValue);
                    _gameStateUI.UpdateTime(_gameTimer);

                    if (_gameTimer == 0f)
                    {
                        _isGameRunning = false;

                        List<PlayerController> winners = new List<PlayerController>();
                        List<PlayerController> losers = new List<PlayerController>();

                        foreach (PlayerController player in _players)
                        {
                            if (!player.IsDead)
                            {
                                winners.Add(player);
                            }
                            else
                            {
                                losers.Add(player);
                            }
                        }

                        _gameStateUI.ActivateGameResultView(winners, losers);
                    }
                }
            }
            else if (_shipHealth <= 0f)
            {
                _isGameRunning = false;

                List<PlayerController> winners = new List<PlayerController>();
                List<PlayerController> losers = new List<PlayerController>();

                foreach (PlayerController player in _players)
                {
                    if (player.HasItem && player.CurrentItem._itemType == ItemType.parachute)
                    {
                        winners.Add(player);
                    }
                    else
                    {
                        losers.Add(player);
                    }
                }

                _gameStateUI.ActivateGameResultView(winners, losers);
            }
            else if (_gameTimer <= 0f)
            {

                if (_shipHealth > _baseShipHealth * 0.5f)
                {
                    _isGameRunning = false;
                    _gameStateUI.ActivateGameResultView(_players, new List<PlayerController>());
                }
                else
                {
                    _isGameInEndPhase = true;
                    _playerDiedInEndPhase = false;

                    CreateFlyingPlanks();
                    //*****
                    //_isGameRunning = false;
                    //_gameStateUI.ActivateGameResultView (new List<PlayerController> (), _players);
                }
            }
            else
            {
                _gameTimer = Mathf.Clamp(_gameTimer - Time.deltaTime, 0f, float.MaxValue);
                _gameStateUI.UpdateTime(_gameTimer);
            }
        }
    }

    //==========================================================================================================
    //
    //==========================================================================================================

    public void ApplyDamageToShip(float damage)
    {
        if (_isGameRunning && damage > 0 && _shipHealth > 0)
        {
            _shipHealth -= damage;
            _gameStateUI.UpdateShipHealth(_shipHealth);
        }
    }

    public void RegisterPlayer(PlayerController player)
    {
        if (!_players.Contains(player))
        {
            _players.Add(player);
        }
    }

    public List<PlayerController> GetAllPlayers()
    {
        return _players;
    }

    public void OnPlayerDied()
    {
        if (IsGameInEndPhase)
        {
            _playerDiedInEndPhase = true;
            _gameTimer = 60f;
        }
    }

    //==========================================================================================================
    //
    //==========================================================================================================

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        Debug.Log("GameController: OnSceneLoaded: called!");

        Global.MenuCamera.enabled = false;
        Global.LevelCamera.enabled = true;
        //SpawnManager.Instance.CreateAndSpawnPlayers ();

        Event.Instance.SendOnGameStartedEvent();

        _isGameRunning = true;

        // for testing only!
        //_isGameInEndPhase = true;
        //CreateFlyingPlanks();
    }

    public void LoadLevel(string levelName)
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene("Scenes/Levels/" + levelName, LoadSceneMode.Additive);

        Global.LevelSelectionMenu.ToggleVisibility(false);
        Global.InGameUI.ToggleVisibility(true);
    }

    //==========================================================================================================
    //
    //==========================================================================================================

    private void CreateFlyingPlanks()
    {
        if (!_isGameRunning)
        {
            return;
        }

        Debug.Log("GameController:CreateFlyingPlanks:called!");

        int count = _flyingPlanks.Count;
        for(int i = 0;i < count; i++)
        {
            GameObject obj = _flyingPlanks[i];

            if(obj == null)
            {
                _flyingPlanks.RemoveAt(i);
                i--;
                count--;
                Debug.Log("GameController:CreateFlyingPlanks:removed null entry from list");
            }
        }

        int numOfPlanks = UnityEngine.Random.Range(_minSpawnOfPlanks, _maxSpawnOfPlanks);
        numOfPlanks = Mathf.Clamp(numOfPlanks, 0, _maxNumOfPlanks - _flyingPlanks.Count);
        
        if(UnityEngine.Random.Range(0f,1f) > 0.5f)
        {
            _minSpawnOfPlanks++;
            _maxSpawnOfPlanks++;
        }

        for(int i = 0; i < numOfPlanks; i++)
        {
            Vector3 screenPosition = Camera.main.ScreenToWorldPoint(new Vector3(UnityEngine.Random.Range(0, Screen.width), UnityEngine.Random.Range(0, Screen.height), Camera.main.farClipPlane / 2));
            screenPosition.z = 0;

            GameObject plankObj = (GameObject)Instantiate(Resources.Load("Prefabs/FlyingPlank"), screenPosition, Quaternion.identity);
            _flyingPlanks.Add(plankObj);
            plankObj.GetComponent<Rigidbody2D>().velocity = _flyingPlankDirection;
        }

        _flyingPlankTimer -= Mathf.Clamp(_flyingPlankTimer - 0.05f, 0.5f, float.MaxValue);
        Invoke("CreateFlyingPlanks", _flyingPlankTimer);
    }
}
