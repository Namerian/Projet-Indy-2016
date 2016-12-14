using UnityEngine;
using System.Collections;

public class SpawnController : MonoBehaviour
{

    void Awake()
    {
        SpawnManager.Instance.RegisterSpawn(this);
    }

    // Use this for initialization
    /*void Start()
    {
        
    }*/

    // Update is called once per frame
    /*void Update()
    {

    }*/
}
