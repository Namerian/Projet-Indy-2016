using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ActiveIcon : MonoBehaviour, IMachineListener
{
	public GameObject iconPrefab;
	public Vector3 relativePosition;

	public float baseAlpha = 0.7f;
	public float blinkSpeed = 3f;

	private MachineController machineController;
	private GameObject mapOverlay;

	private GameObject icon;

	private bool isActive;
	private float blinkTimer;

	// Use this for initialization
	void Start ()
	{
		machineController = GetComponent<MachineController> ();
		mapOverlay = GameObject.Find ("MapOverlay");

		machineController.AddListener (this);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (isActive) {
			float _alpha = baseAlpha + (1 - baseAlpha) * Mathf.Sin (blinkTimer * blinkSpeed);

			Image _image = icon.GetComponent<Image> ();
			Color _color = _image.color;
			_color.a = _alpha;
			_image.color = _color;

			blinkTimer += Time.deltaTime;
		}
	}

	public void OnStateChange (MachineController.MachineState state)
	{
		if (state == MachineController.MachineState.Active && iconPrefab != null) {
			icon = (GameObject)Instantiate (iconPrefab);
			icon.transform.SetParent (mapOverlay.transform, false);

			Vector3 _targetWorldPosition = this.transform.position + relativePosition;
			icon.transform.Translate (_targetWorldPosition);

			icon.transform.rotation = Quaternion.LookRotation (-Camera.main.transform.forward, Camera.main.transform.up);

			isActive = true;
			blinkTimer = 0f;
		} else {
			Destroy (icon);

			isActive = false;
		}
	}

}
