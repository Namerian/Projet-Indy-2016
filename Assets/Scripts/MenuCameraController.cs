using UnityEngine;
using System.Collections;

public class MenuCameraController : MonoBehaviour
{
	void Awake ()
	{
		Global.MenuCamera = this.GetComponent<Camera> ();
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
