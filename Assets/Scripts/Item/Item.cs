using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour
{
	public string[] itemCategories;
	public string itemName;

	private Collider itemCollider;
	private MeshRenderer itemRenderer;

	// Use this for initialization
	void Start ()
	{
		itemCollider = GetComponent<Collider> ();
		itemRenderer = GetComponent<MeshRenderer> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
	}

	public void OnPickUp ()
	{
		itemCollider.enabled = false;
		itemRenderer.enabled = false;
	}

	public void OnDrop ()
	{
		itemCollider.enabled = true;
		itemRenderer.enabled = true;
	}
}
