using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameStateUIView : MonoBehaviour
{
	private Text _timeLeftText;
	private Slider _shipHealthSlider;

	private CanvasGroup _gameResultCanvasGroup;
	private Transform _gameResultListTransform;

	//==========================================================================================================
	//
	//==========================================================================================================

	// Use this for initialization
	void Start ()
	{
		_timeLeftText = GameObject.Find ("UI/InGameUI/GameStateUI/GameStatePanel/TimeLeftText").GetComponent<Text> ();
		_shipHealthSlider = GameObject.Find ("UI/InGameUI/GameStateUI/GameStatePanel/ShipHealthSlider").GetComponent<Slider> ();

		GameObject resultPanelObj = GameObject.Find ("UI/InGameUI/GameStateUI/GameResultPanel");
		_gameResultCanvasGroup = resultPanelObj.GetComponent<CanvasGroup> ();
		_gameResultListTransform = resultPanelObj.transform.Find ("ResultList");
		_gameResultCanvasGroup.alpha = 0;
	}
	
	// Update is called once per frame
	void Update ()
	{
	}

	//==========================================================================================================
	//
	//==========================================================================================================

	public void UpdateTime (float timeLeft)
	{
		TimeSpan span = TimeSpan.FromSeconds (timeLeft);
		_timeLeftText.text = string.Format ("{0:D2}:{1:D2}", span.Minutes, span.Seconds);
	}

	public void UpdateShipHealth (float healthLeft)
	{
		_shipHealthSlider.value = healthLeft / Global.GameController._baseShipHealth;
	}

	public void ActivateGameResultView (List<PlayerController> winners, List<PlayerController> losers)
	{
		_gameResultCanvasGroup.alpha = 1;

		if (winners.Count > 0) {
			CreateResultListTitle ("WINNERS");
			CreateResultListElements (winners);
		}

		if (losers.Count > 0) {
			CreateResultListTitle ("LOOSERS");
			CreateResultListElements (losers);
		}
	}

	//==========================================================================================================
	//
	//==========================================================================================================

	private void CreateResultListTitle (string title)
	{
		GameObject titleObj = (GameObject)Instantiate (Resources.Load ("Prefabs/UI/ResultListTitle"), _gameResultListTransform);
		titleObj.transform.Find ("TitleText").GetComponent<Text> ().text = title;
	}

	private void CreateResultListElements (List<PlayerController> players)
	{
		List<PlayerController> unsortedPlayers = new List<PlayerController> (players);
		//List<PlayerController> sortedPlayers = new List<PlayerController> ();

		while (unsortedPlayers.Count > 0) {
			PlayerController bestPlayer = unsortedPlayers [0];
			int bestScore = bestPlayer.Score;

			if (unsortedPlayers.Count > 1) {
				for (int i = 1; i < unsortedPlayers.Count; i++) {
					PlayerController currentPlayer = unsortedPlayers [i];
					int currentScore = currentPlayer.Score;

					if (currentScore > bestScore) {
						bestPlayer = currentPlayer;
						bestScore = currentScore;
					}
				}
			}

			GameObject elementObj = (GameObject)Instantiate (Resources.Load ("Prefabs/UI/ResultListElement"), _gameResultListTransform);
			Text nameText = elementObj.transform.Find ("NameText").GetComponent<Text> ();
			Text scoreText = elementObj.transform.Find ("ScoreText").GetComponent<Text> ();

			switch (bestPlayer._playerName) {
			case PlayerName.BluePlayer:
				nameText.text = "Blue Player";
				scoreText.text = bestScore.ToString ();

				nameText.color = scoreText.color = Color.blue;
				break;
			case PlayerName.GreenPlayer:
				nameText.text = "Green Player";
				scoreText.text = bestScore.ToString ();

				nameText.color = scoreText.color = Color.green;
				break;
			case PlayerName.PurplePlayer:
				nameText.text = "Purple Player";
				scoreText.text = bestScore.ToString ();

				nameText.color = scoreText.color = Color.magenta;
				break;
			case PlayerName.RedPlayer:
				nameText.text = "Red Player";
				scoreText.text = bestScore.ToString ();

				nameText.color = scoreText.color = Color.red;
				break;
			}

			unsortedPlayers.Remove (bestPlayer);
		}
	}
}
