/*using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Mission
{
	/*public string name { get; private set; }
    public float timer { get; private set; }
    public bool success { get; private set; }

    private bool active;

    private List<IMissionListener> listeners;

    public Mission()
    {
        name = "Test Mission";
        timer = 20;

        listeners = new List<IMissionListener>();
    }

    public void Start()
    {
        active = true;
        MissionStarted();
    }

    public void Update()
    {
        if (active)
        {
            if (timer == 0)
            {
                active = false;
                MissionEnded();
            }
            else
            {
                timer = Mathf.Clamp(timer - Time.deltaTime, 0, float.MaxValue);
                TimerUpdated();
            }
        }
    }

    public void AddListener(IMissionListener listener)
    {
        if (!listeners.Contains(listener))
        {
            listeners.Add(listener);
        }
    }

    public void RemoveListener(IMissionListener listener)
    {
        listeners.Remove(listener);
    }

    private void MissionStarted()
    {
        foreach (IMissionListener listener in listeners)
        {
            listener.OnMissionStarted(this);
        }
    }

    private void MissionEnded()
    {
        foreach (IMissionListener listener in listeners)
        {
            listener.OnMissionEnded(this);
        }
    }

    private void TimerUpdated()
    {
        foreach (IMissionListener listener in listeners)
        {
            listener.OnTimerUpdated(this);
        }
    }
}*/
