using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MissionResultPanelView : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private Text missionResultText;

    private bool active;
    private float timer;

    // Use this for initialization
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        missionResultText = GameObject.Find("UI/InGameUI/MissionUI/MissionResultPanel/MissionResultText").GetComponent<Text>();

        canvasGroup.alpha = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            if (timer >= 2f)
            {
                active = false;
                canvasGroup.alpha = 0;
            }
            else
            {
                timer += Time.deltaTime;
            }
        }
    }

    public void Activate(Mission mission)
    {
        active = true;
        timer = 0f;
        canvasGroup.alpha = 1;

        if (mission.success)
        {
            missionResultText.text = "Mission Succeeded!";
        }
        else
        {
            missionResultText.text = "Mission Failed!";
        }
    }
}
