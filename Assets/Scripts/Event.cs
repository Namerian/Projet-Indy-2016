using UnityEngine;
using System.Collections;

public delegate void OnGameStartedDelegate ();
public delegate void OnGameEndedDelegate ();
public delegate void OnGamePausedDelegate ();
public delegate void OnGameUnpausedDelegate ();

public class Event
{
	private static Event _instance;

	public static Event Instance {
		get {
			if (_instance == null) {
				_instance = new Event ();
			}

			return _instance;
		}
	}

	//==================================================================================
	//
	//==================================================================================

	public event OnGameStartedDelegate OnGameStartedEvent;
	public event OnGameEndedDelegate OnGameEndedEvent;
	public event OnGamePausedDelegate OnGamePausedEvent;
	public event OnGameUnpausedDelegate OnGameUnpausedEvent;

	//==================================================================================
	//
	//==================================================================================

	public void SendOnGameStartedEvent ()
	{
		this.OnGameStartedEvent ();
	}

	public void SendOnGameEndedEvent ()
	{
		this.OnGameEndedEvent ();
	}

	public void SendOnGamePausedEvent ()
	{
		this.OnGamePausedEvent ();
	}

	public void SendOnGameUnpausedEvent ()
	{
		this.OnGameUnpausedEvent ();
	}
}
