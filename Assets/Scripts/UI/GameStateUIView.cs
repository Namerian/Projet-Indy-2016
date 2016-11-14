using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class GameStateUIView : MonoBehaviour
{
	private GameController gameController;

	private Text timeLeftText;
	private Slider shipHealthSlider;

	private Text gameResultText;
	private CanvasGroup gameResultCanvasGroup;

	//==========================================================================================================
	//
	//==========================================================================================================

	// Use this for initialization
	void Start ()
	{
		gameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();

		timeLeftText = GameObject.Find ("UI/InGameUI/GameStateUI/GameStatePanel/TimeLeftText").GetComponent<Text> ();
		shipHealthSlider = GameObject.Find ("UI/InGameUI/GameStateUI/GameStatePanel/ShipHealthSlider").GetComponent<Slider> ();

		gameResultText = GameObject.Find ("UI/InGameUI/GameStateUI/GameResultPanel/GameResultText").GetComponent<Text> ();
		gameResultCanvasGroup = GameObject.Find ("UI/InGameUI/GameStateUI/GameResultPanel").GetComponent<CanvasGroup> ();

		gameResultCanvasGroup.alpha = 0;
	}
	
	// Update is called once per frame
	void Update ()
	{
		//timer
		TimeSpan span = TimeSpan.FromSeconds (gameController.gameTimer);
		timeLeftText.text = string.Format ("{0:D2}:{1:D2}", span.Minutes, span.Seconds);

		//slider
		shipHealthSlider.value = gameController.shipHealth;
	}

	//==========================================================================================================
	//
	//==========================================================================================================

	public void ActivateGameResultView (bool gameWon)
	{
		gameResultCanvasGroup.alpha = 1;

		if (gameWon) {
			gameResultText.text = "Victory";
		} else {
			gameResultText.text = "Defeat";
		}
	}
}
