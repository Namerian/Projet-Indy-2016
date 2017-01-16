using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : IMachine
{
	public Sprite _spriteOpen;
	public Sprite _spriteClosed;
	public bool _requiresKey = false;
	public ItemType _droppedItem;

	private SpriteRenderer _renderer;

	private Dictionary<ItemType, int> _dropAmounts;

	private bool _isActive = true;
	private GameObject _spawnedItem;

	public override bool IsActive{ get { return _isActive; } }

	// Use this for initialization
	void Start ()
	{
		_renderer = this.GetComponent<SpriteRenderer> ();
		_renderer.sprite = _spriteClosed;
	}

	void Update ()
	{
		if (!_isActive && _spawnedItem == null) {
			_isActive = true;
			_renderer.sprite = _spriteClosed;
		}
	}

	public override MachineInteractionState Interact (PlayerController player)
	{
		if (!_isActive) {
			return new MachineInteractionState (player, false);
		}

		if (_requiresKey && player.HasItem && player.CurrentItem._itemType == ItemType.key) {
			SpawnItem ();
			Deactivate ();
		} else if (!_requiresKey) {
			SpawnItem ();
			Deactivate ();
		}

		return new MachineInteractionState (player, false);
	}

	private void Deactivate ()
	{
		_isActive = false;

		_renderer.sprite = _spriteOpen;
	}

	private void SpawnItem ()
	{
		//Debug.Log ("Chest:SpawnItem:called! item =" + _droppedItem);

		switch (_droppedItem) {
		case ItemType.canonball:
			_spawnedItem = (GameObject)Instantiate (Resources.Load ("Prefabs/Items/Canonball"), this.transform.position, Quaternion.identity);
			break;
		case ItemType.hammer:
			_spawnedItem = (GameObject)Instantiate (Resources.Load ("Prefabs/Items/Hammer"), this.transform.position, Quaternion.identity);
			break;
		case ItemType.key:
			_spawnedItem = (GameObject)Instantiate (Resources.Load ("Prefabs/Items/Key"), this.transform.position, Quaternion.identity);
			break;
		case ItemType.mop:
			_spawnedItem = (GameObject)Instantiate (Resources.Load ("Prefabs/Items/Mop"), this.transform.position, Quaternion.identity);
			break;
		case ItemType.parachute:
			_spawnedItem = (GameObject)Instantiate (Resources.Load ("Prefabs/Items/Parachute"), this.transform.position, Quaternion.identity);
			break;
		case ItemType.torch:
			_spawnedItem = (GameObject)Instantiate (Resources.Load ("Prefabs/Items/Torch"), this.transform.position, Quaternion.identity);
			break;
		case ItemType.wheel:
			_spawnedItem = (GameObject)Instantiate (Resources.Load ("Prefabs/Items/Wheel"), this.transform.position, Quaternion.identity);
			break;
		}
	}
}
