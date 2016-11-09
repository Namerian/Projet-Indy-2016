using UnityEngine;
using System.Collections;

public interface IMissionListener
{
    void OnMissionStarted(Mission mission);
    void OnTimerUpdated(Mission mission);
    void OnMissionEnded(Mission mission);
}
