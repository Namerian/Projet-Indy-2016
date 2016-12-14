using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    private List<SpawnController> _spawnControllerList;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            _spawnControllerList = new List<SpawnController>();
        }
        else
        {
            Destroy(this);
        }
    }

    // Use this for initialization
    /*void Start()
    {

    }*/

    // Update is called once per frame
    /*void Update()
    {

    }*/

    public void RegisterSpawn(SpawnController spawn)
    {
        if (!_spawnControllerList.Contains(spawn))
        {
            _spawnControllerList.Add(spawn);
            Debug.Log("SpawnManager: spawn registered");
        }
    }

    public void CreateAndSpawnPlayers()
    {
        if (_spawnControllerList.Count < 4)
        {
            Debug.LogError("Not enough spawnpoints in scene!");
            return;
        }

        GameObject playerHolderObject = new GameObject("PlayerHolder");

        List<GameObject> playerList = new List<GameObject>();
        playerList.Add(Instantiate(Resources.Load("Prefabs/Players/BluePlayer"), playerHolderObject.transform) as GameObject);
        playerList.Add(Instantiate(Resources.Load("Prefabs/Players/GreenPlayer"), playerHolderObject.transform) as GameObject);
        playerList.Add(Instantiate(Resources.Load("Prefabs/Players/RedPlayer"), playerHolderObject.transform) as GameObject);
        playerList.Add(Instantiate(Resources.Load("Prefabs/Players/YellowPlayer"), playerHolderObject.transform) as GameObject);

        for (int i = 0; i < 4; i++)
        {
            PlayerController playerController = playerList[i].GetComponent<PlayerController>();
            SpawnController spawnController = _spawnControllerList[i];

            playerController.Initialize(i);

            Vector3 spawnPos = spawnController.transform.position;
            Vector3 playerPos = playerController.transform.position;
            playerController.transform.position = new Vector3(spawnPos.x, playerPos.y, spawnPos.z);
        }
    }
}
