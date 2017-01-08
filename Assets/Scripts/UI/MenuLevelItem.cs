using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuLevelItem : MonoBehaviour
{
	public Text Text{ get; private set; }

	public Image Background{ get; private set; }

	void Awake ()
	{
		Text = this.GetComponentInChildren<Text> ();
		Background = this.GetComponent<Image> ();
	}

	// Use this for initialization
	/*void Start ()
	{
	
	}*/
	
	// Update is called once per frame
	/*void Update ()
	{
	
	}*/
}
