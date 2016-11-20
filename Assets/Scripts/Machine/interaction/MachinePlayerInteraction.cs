using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MachinePlayerInteraction : MonoBehaviour, IMachineListener
{
	public float interactionTime;
	public bool requiresItem;
	public string[] itemCategories;

	private GameController gameController;
	private MachineController machineController;
	private GameObject mapOverlay;

	private bool isActive;

	private bool isInteracting;
	private PlayerController player;
	private GameObject progressBar;
	private float interactionTimer;

	// Use this for initialization
	void Start ()
	{
		gameController = GameObject.FindWithTag ("GameController").GetComponent<GameController> ();
		machineController = GetComponent<MachineController> ();
		mapOverlay = GameObject.Find ("MapOverlay");

		machineController.AddListener (this);

		isActive = false;
		isInteracting = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!gameController.isPaused && isInteracting) {
			if (interactionTimer >= interactionTime) {
				machineController.SetState (MachineController.MachineState.Idle);

				isInteracting = false;
				interactionTimer = 0f;
				Destroy (progressBar);
				player = null;
			} else {
				progressBar.GetComponent<Image> ().fillAmount = interactionTimer / interactionTime;

				interactionTimer += Time.deltaTime;
			}
		}
	}

	void IMachineListener.OnStateChange (MachineController.MachineState state)
	{
		//Debug.Log ("MachinePlayerInteraction: OnStateChange: state=" + state.ToString ());

		if (state == MachineController.MachineState.Active) {
			isActive = true;
		} else {
			isActive = false;
			if (isInteracting) {
				isInteracting = false;
				interactionTimer = 0f;
				Destroy (progressBar);
				player = null;
			}
		}
	}

	public void OnStartInteraction (PlayerController player)
	{
		//Debug.Log ("MachinePlayerInteraction: OnStartInteraction: machineName=" + this.gameObject.name + " playerName=" + player.gameObject.name);

		if (isActive) {
			if (requiresItem) {
				if (player.currentItem != null) {
					string[] _playerItemCategories = player.currentItem.itemCategories;
					bool _interactionAlloxed = false;

					foreach (string _allowedCategory in itemCategories) {
						if (CheckForAllowedCategory (_allowedCategory, _playerItemCategories)) {
							_interactionAlloxed = true;
							break;
						}
					}

					if (_interactionAlloxed) {
						StartInteraction (player);
					}
				}
			} else {
				StartInteraction (player);
			}
		}
	}

	public void OnEndInteraction (PlayerController player)
	{
		//Debug.Log ("MachinePlayerInteraction: OnEndInteraction: machineName=" + this.gameObject.name + " playerName=" + player.gameObject.name);

		if (isActive && isInteracting) {
			isInteracting = false;
			interactionTimer = 0f;
			Destroy (progressBar);
			player = null;
		}
	}

	private bool CheckForAllowedCategory (string allowedCategory, string[] playerItemCategories)
	{
		foreach (string _playerItemCategory in playerItemCategories) {
			if (allowedCategory == _playerItemCategory) {
				return true;
			}
		}

		return true;
	}

	private void StartInteraction (PlayerController player)
	{
		this.player = player;
		isInteracting = true;
		interactionTimer = 0f;

		progressBar = (GameObject)Instantiate (Resources.Load ("Prefabs/CircleProgressBar"));
		progressBar.transform.SetParent (mapOverlay.transform, false);

		Vector3 _position = player.transform.position;
		_position.y += player.GetComponent<CapsuleCollider> ().height * 0.5f + 1f;
		progressBar.transform.Translate (_position);

		progressBar.transform.rotation = Quaternion.LookRotation (-Camera.main.transform.forward, Camera.main.transform.up);

		Image _image = progressBar.GetComponent<Image> ();
		_image.fillAmount = 0f;
		_image.color = Color.green;
	}
}
