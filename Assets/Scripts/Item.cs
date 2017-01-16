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
	parachute,
	none
}

public class Item : MonoBehaviour, ILightEmitter
{
	public ItemType _itemType;

	private CircleCollider2D _itemCollider;
	private SpriteRenderer _itemRenderer;

	private SpriteRenderer ItemRenderer {
		get {
			if (_itemRenderer == null) {
				_itemRenderer = GetComponent<SpriteRenderer> ();
			}

			return _itemRenderer;
		}
	}

	// Use this for initialization
	void Start ()
	{
		_itemCollider = GetComponent<CircleCollider2D> ();
		_itemRenderer = GetComponent<SpriteRenderer> ();
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

	public bool IsEmittingLight ()
	{
		if (ItemRenderer.enabled && _itemType == ItemType.torch) {
			return true;
		}

		return false;
	}
}
