using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

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
	private static Dictionary<ItemType, int> ITEM_AMOUNT;

	public static int GetItemAmount (ItemType type)
	{
		return ITEM_AMOUNT [type];
	}

	//=================================================================

	public ItemType _itemType;

	private CircleCollider2D _itemCollider;
	private SpriteRenderer _itemRenderer;

	void Awake ()
	{
		if (ITEM_AMOUNT == null) {
			ITEM_AMOUNT = new Dictionary<ItemType, int> ();

			Array values = Enum.GetValues (typeof(ItemType));
			foreach (ItemType type in values) {
				ITEM_AMOUNT.Add (type, 0);
			}
		}
	}

	// Use this for initialization
	void Start ()
	{
		_itemCollider = GetComponent<CircleCollider2D> ();
		_itemRenderer = GetComponent<SpriteRenderer> ();

		ITEM_AMOUNT [_itemType]++;
	}
	
	// Update is called once per frame
	/*void Update ()
	{
	}*/

	void OnDestroy ()
	{
		ITEM_AMOUNT [_itemType]--;
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
