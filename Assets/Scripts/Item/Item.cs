using UnityEngine;
using System.Collections;

public abstract class Item : MonoBehaviour
{
	public string itemName{ protected set; get; }

	protected abstract void OnStart ();

	protected abstract void OnUpdate ();

	// Use this for initialization
	void Start ()
	{
		OnStart ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		OnUpdate ();
	}
}
