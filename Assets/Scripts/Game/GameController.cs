using UnityEngine;
using System.Collections;
using System;

public class GameController : MonoBehaviour, IMissionListener
{
	private PlayerController redPlayer;
	private PlayerController greenPlayer;
	private PlayerController bluePlayer;
	private PlayerController yellowPlayer;

	private GameObject missionViewObject;

	private Mission currentMission;

	void Awake ()
	{
		//UI
		missionViewObject = GameObject.Find ("UI/InGameUI/MissionUI/MissionTimerPanel");

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
	}

	// Update is called once per frame
	void Update ()
	{
		/*if (currentMission == null && Input.GetKeyDown (KeyCode.E)) {
			currentMission = new Mission ();
			currentMission.AddListener (this);
			missionViewObject.GetComponent<MissionTimerPanelView> ().Initialize (currentMission);
			currentMission.Start ();
		} else if (currentMission != null) {
			currentMission.Update ();
		}*/
	}

	//==========================================================================================================
	// IMissionListener Implementation
	//==========================================================================================================

	void IMissionListener.OnMissionStarted (Mission mission)
	{
	}

	void IMissionListener.OnTimerUpdated (Mission mission)
	{
	}

	void IMissionListener.OnMissionEnded (Mission mission)
	{
		currentMission = null;
	}
}
