using UnityEngine;
using System.Collections;

public enum ItemType
{
	canonball,
	torch,
	wheel,
	hammer,
	mop,
	key,
	parachute
}

public class Item : MonoBehaviour
{
	public ItemType _itemType;

	private CircleCollider2D _itemCollider;
	private SpriteRenderer _itemRenderer;

	// Use this for initialization
	void Start ()
	{
		_itemCollider = GetComponent<CircleCollider2D> ();
		_itemRenderer = GetComponent<SpriteRenderer> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
	}

	public void OnPickUp ()
	{
		_itemCollider.enabled = false;
		_itemRenderer.enabled = false;
	}

	public void OnDrop ()
	{
		_itemCollider.enabled = true;
		_itemRenderer.enabled = true;
	}
}
