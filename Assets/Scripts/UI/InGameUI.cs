using UnityEngine;
using System.Collections;

public class InGameUI : MonoBehaviour
{
	private CanvasGroup CanvasGroup{ get; set; }

	void Awake ()
	{
		this.CanvasGroup = this.GetComponent<CanvasGroup> ();
		this.CanvasGroup.alpha = 0;

		Global.InGameUI = this;
	}

	// Use this for initialization
	/*void Start ()
	{
	
	}*/
	
	// Update is called once per frame
	/*void Update ()
	{
	
	}*/

	public void ToggleVisibility (bool visible)
	{
		if (visible) {
			this.CanvasGroup.alpha = 1;
		} else {
			this.CanvasGroup.alpha = 0;
		}
	}
}
