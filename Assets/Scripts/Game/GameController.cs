using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    public float gameTimer { get; private set; }

    public float shipHealth { get; private set; }

    private bool isGameRunning;

    private GameStateUIView gameStateUI;

    private List<RandomActivation> machineRandomActivators;
    private float machineActivationTimer;

    //==========================================================================================================
    //
    //==========================================================================================================

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            //
            machineRandomActivators = new List<RandomActivation>();
        }
        else
        {
            Destroy(this);
        }
    }

    // Use this for initialization
    void Start()
    {
        //UI
        gameStateUI = GameObject.Find("UI/InGameUI/GameStateUI").GetComponent<GameStateUIView>();

        gameTimer = 40f;
        shipHealth = 100f;
        isGameRunning = false;
        machineActivationTimer = 4f;

        //
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene("Scenes/Levels/PatrickLevelTest001", LoadSceneMode.Additive);
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameRunning)
        {
            if (shipHealth <= 0 || gameTimer <= 0)
            {
                isGameRunning = false;

                bool _gameWon = false;
                if (shipHealth > 0)
                {
                    _gameWon = true;
                }
                gameStateUI.ActivateGameResultView(_gameWon);
            }
            else
            {
                //update game timer
                gameTimer = Mathf.Clamp(gameTimer - Time.deltaTime, 0f, float.MaxValue);

                //machine activation
                machineActivationTimer = Mathf.Clamp(machineActivationTimer - Time.deltaTime, 0f, float.MaxValue);

                if (machineActivationTimer == 0)
                {
                    int _sumOfWeights = 0;
                    foreach (RandomActivation activator in machineRandomActivators)
                    {
                        _sumOfWeights += activator.weight;
                    }

                    if (_sumOfWeights > 0)
                    {
                        RandomActivation _selectedActivator = null;
                        int _currentWeight = 0;
                        int _randomNumber = UnityEngine.Random.Range(0, _sumOfWeights);
                        foreach (RandomActivation activator in machineRandomActivators)
                        {
                            _currentWeight += activator.weight;
                            if (_currentWeight > _randomNumber)
                            {
                                _selectedActivator = activator;
                                break;
                            }
                        }

                        if (_selectedActivator != null)
                        {
                            _selectedActivator.Activate();
                        }
                    }

                    machineActivationTimer = UnityEngine.Random.Range(4f, 8f);
                }
            }
        }
    }

    //==========================================================================================================
    //
    //==========================================================================================================

    public bool isPaused { get { return !isGameRunning; } }

    public void ApplyDamageToShip(float damage)
    {
        if (damage > 0 && shipHealth > 0)
        {
            shipHealth -= damage;
        }
    }

    public void AddMachineRandomActivator(RandomActivation activator)
    {
        if (!machineRandomActivators.Contains(activator))
        {
            //Debug.Log ("GameController: AddMachineRandomActivator: machineName=" + activator.gameObject.name);
            machineRandomActivators.Add(activator);
        }
    }

    public void RemoveMachineRandomActivator(RandomActivation activator)
    {
        //Debug.Log ("GameController: RemoveMachineRandomActivator: machineName=" + activator.gameObject.name);
        machineRandomActivators.Remove(activator);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        Debug.Log("GameController: OnSceneLoaded: called!");

        isGameRunning = true;

        SpawnManager.Instance.CreateAndSpawnPlayers();
    }
}
