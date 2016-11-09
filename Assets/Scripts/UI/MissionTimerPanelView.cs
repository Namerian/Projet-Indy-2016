using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class MissionTimerPanelView : MonoBehaviour, IMissionListener
{
    private CanvasGroup canvasGroup;
    private Text missionNameText;
    private Text missionTimerText;

    private MissionResultPanelView resultView;

    private TimeSpan timeSpan;

    // Use this for initialization
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        missionNameText = GameObject.Find("UI/InGameUI/MissionUI/MissionTimerPanel/MissionNameText").GetComponent<Text>();
        missionTimerText = GameObject.Find("UI/InGameUI/MissionUI/MissionTimerPanel/MissionTimerText").GetComponent<Text>();

        resultView = GameObject.Find("UI/InGameUI/MissionUI/MissionResultPanel").GetComponent<MissionResultPanelView>();

        canvasGroup.alpha = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Initialize(Mission mission)
    {
        mission.AddListener(this);
    }

    void IMissionListener.OnMissionStarted(Mission mission)
    {
        canvasGroup.alpha = 1;
        missionNameText.text = mission.name;
        timeSpan = TimeSpan.FromSeconds(mission.timer);
        missionTimerText.text = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
    }

    void IMissionListener.OnTimerUpdated(Mission mission)
    {
        timeSpan = TimeSpan.FromSeconds(mission.timer + 1);
        missionTimerText.text = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
    }

    void IMissionListener.OnMissionEnded(Mission mission)
    {
        canvasGroup.alpha = 0;
        resultView.Activate(mission);
    }

    
}
