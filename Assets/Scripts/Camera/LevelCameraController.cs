using UnityEngine;
using System.Collections;

public class LevelCameraController : MonoBehaviour
{

	void Awake ()
	{
		Global.LevelCamera = this.GetComponent<Camera> ();
	}

	// Use this for initialization
	/*void Start ()
	{
	
	}*/
	
	// Update is called once per frame
	/*void Update ()
	{
	
	}*/
}
