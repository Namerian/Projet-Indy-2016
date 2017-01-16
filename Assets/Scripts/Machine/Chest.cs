using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : IMachine
{
	public Sprite _spriteOpen;
	public Sprite _spriteClosed;
	public bool _requiresKey = false;
	public int _activationChance = 20;

	[Header ("Drop Amounts")]
	public int _canonball = 1;
	public int _torch = 1;
	public int _wheel = 1;
	public int _hammer = 1;
	public int _mop = 1;
	public int _key = 1;
	public int _parachute = 0;

	private SpriteRenderer _renderer;

	private Dictionary<ItemType, int> _dropAmounts;

	private bool _isActive = false;
	private ItemType _itemToSpawn = ItemType.none;

	public override bool IsActive{ get { return _isActive; } }

	void Awake ()
	{
		_renderer = this.GetComponent<SpriteRenderer> ();

		_dropAmounts = new Dictionary<ItemType, int> ();
		_dropAmounts.Add (ItemType.canonball, _canonball);
		_dropAmounts.Add (ItemType.torch, _torch);
		_dropAmounts.Add (ItemType.wheel, _wheel);
		_dropAmounts.Add (ItemType.hammer, _hammer);
		_dropAmounts.Add (ItemType.mop, _hammer);
		_dropAmounts.Add (ItemType.key, _key);
		_dropAmounts.Add (ItemType.parachute, _parachute);
	}

	// Use this for initialization
	void Start ()
	{
		if (ChooseItemToSpawn ()) {
			Activate ();
		} else {
			Invoke ("RandomActivation", UnityEngine.Random.Range (_activationIntervalMin, _activationIntervalMax));
		}
	}
	
	// Update is called once per frame
	/*void Update ()
	{
		
	}*/

	public override MachineInteractionState Interact (PlayerController player)
	{
		if (!_isActive) {
			return new MachineInteractionState (player, false);
		}

		if (_requiresKey && player.HasItem && player.CurrentItem._itemType == ItemType.key) {
			if (SpawnItem ()) {
				player.DestroyCurrentItem ();
				Deactivate ();
			}
		} else if (!_requiresKey) {
			if (SpawnItem ()) {
				Deactivate ();
			}
		}

		return new MachineInteractionState (player, false);
	}

	private void Activate ()
	{
		if (_isActive) {
			return;
		}

		_isActive = true;

		_renderer.sprite = _spriteClosed;
	}

	private void Deactivate ()
	{
		_isActive = false;

		_renderer.sprite = _spriteOpen;

		Invoke ("RandomActivation", UnityEngine.Random.Range (_activationIntervalMin, _activationIntervalMax));
	}

	private void RandomActivation ()
	{
		if (_isActive || Global.GameController.IsPaused) {
			return;
		}

		int diceRoll = UnityEngine.Random.Range (1, 100);

		if (diceRoll <= _activationChance && ChooseItemToSpawn ()) {
			Activate ();
		} else {
			Invoke ("RandomActivation", UnityEngine.Random.Range (_activationIntervalMin, _activationIntervalMax));
		}
	}

	private bool SpawnItem ()
	{
		switch (_itemToSpawn) {
		case ItemType.canonball:
			Instantiate (Resources.Load ("Prefabs/Items/Canonball"), this.transform.position, Quaternion.identity);
			break;
		case ItemType.hammer:
			Instantiate (Resources.Load ("Prefabs/Items/Hammer"), this.transform.position, Quaternion.identity);
			break;
		case ItemType.key:
			Instantiate (Resources.Load ("Prefabs/Items/Key"), this.transform.position, Quaternion.identity);
			break;
		case ItemType.mop:
			Instantiate (Resources.Load ("Prefabs/Items/Mop"), this.transform.position, Quaternion.identity);
			break;
		case ItemType.parachute:
			Instantiate (Resources.Load ("Prefabs/Items/Parachute"), this.transform.position, Quaternion.identity);
			break;
		case ItemType.torch:
			Instantiate (Resources.Load ("Prefabs/Items/Torch"), this.transform.position, Quaternion.identity);
			break;
		case ItemType.wheel:
			Instantiate (Resources.Load ("Prefabs/Items/Wheel"), this.transform.position, Quaternion.identity);
			break;
		default:
			return false;
		}

		return true;
	}

	private bool ChooseItemToSpawn ()
	{
		List<ItemType> availableTypes = new List<ItemType> ();

		foreach (KeyValuePair<ItemType, int> entry in _dropAmounts) {
			if (Item.GetItemAmount (entry.Key) < entry.Value) {
				availableTypes.Add (entry.Key);
			}
		}

		if (availableTypes.Count == 0) {
			_itemToSpawn = ItemType.none;
			return false;
		}

		int diceRoll = UnityEngine.Random.Range (0, availableTypes.Count - 1);
		_itemToSpawn = availableTypes [diceRoll];

		return true;
	}
}
