using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class LevelSelectionMenu : MonoBehaviour, IInputListener
{
	private CanvasGroup CanvasGroup{ get; set; }

	private bool _isActive;
	private Transform _levelListPanel;
	private List<MenuLevelItem> _menuLevelItemList;
	private MenuLevelItem _selectedItem;
	private int _selectedItemIndex;
	private bool _hasChangedSelection;

	void Awake ()
	{
		this.CanvasGroup = this.GetComponent<CanvasGroup> ();
		this.CanvasGroup.alpha = 0;

		this._isActive = false;
		this._levelListPanel = this.transform.FindChild ("LevelList");
		this._menuLevelItemList = new List<MenuLevelItem> ();
		this._selectedItem = null;
		this._selectedItemIndex = -1;
		this._hasChangedSelection = false;

		Global.LevelSelectionMenu = this;
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
		if (visible && !_isActive) {
			this.CanvasGroup.alpha = 1;
			this._isActive = true;
			PopulateLevelList ();
			InputHandler.Instance.AddInputListener (this, InputHandler.JOYSTICK_NAMES [0]);
		} else if (!visible && _isActive) {
			this.CanvasGroup.alpha = 0;
			this._isActive = false;
			InputHandler.Instance.RemoveInputListener (this);
		}
	}

	private void PopulateLevelList ()
	{
		DirectoryInfo levelDirectoryPath = new DirectoryInfo (Application.dataPath + "/Scenes/Levels");
		FileInfo[] fileInfoArray = levelDirectoryPath.GetFiles ("*.unity", SearchOption.AllDirectories);

		if (fileInfoArray.Length > 0) {
			Debug.Log ("levelScenes loaded, count = " + fileInfoArray.Length);
		}

		foreach (FileInfo fileInfo in fileInfoArray) {
			GameObject menuItemObj = (GameObject)Instantiate (Resources.Load ("Prefabs/MenuLevelItem"), _levelListPanel);
			MenuLevelItem menuItemScript = menuItemObj.GetComponent<MenuLevelItem> ();
			menuItemScript.Text.text = Path.GetFileNameWithoutExtension (fileInfo.Name);
			_menuLevelItemList.Add (menuItemScript);
		}

		SelectListItem (0);
	}

	private void SelectListItem (int index)
	{
		if (index < 0 || index >= _menuLevelItemList.Count || _hasChangedSelection) {
			return;
		}

		if (_selectedItem != null) {
			_selectedItem.Background.color = Color.white;
			_selectedItem = null;
		}

		_selectedItem = _menuLevelItemList [index];
		_selectedItem.Background.color = Color.yellow;
		_selectedItemIndex = index;

		_hasChangedSelection = true;
		Invoke ("OnSelectionChangeTimerEnded", 0.5f);
	}

	private void OnSelectionChangeTimerEnded ()
	{
		_hasChangedSelection = false;
	}

	//=================================================================================================
	// Input Handling
	//=================================================================================================

	void IInputListener.OnHandleLeftStick (int joystickIndex, Vector2 stickState)
	{
		if (_isActive && joystickIndex == 0 && stickState.y != 0) {
			//Debug.Log ("LevelSelectionMenu: OnHandleLeftStick: stickState.y=" + stickState.y);

			if (stickState.y > 0.4f) {
				SelectListItem (_selectedItemIndex - 1);
			} else if (stickState.y < 0.4f) {
				SelectListItem (_selectedItemIndex + 1);
			}
		}
	}

	void IInputListener.OnHandleXButton (int joystickIndex, bool pressed)
	{
		if (_isActive && joystickIndex == 0 && pressed && _selectedItem != null) {
			Global.GameController.LoadLevel (_selectedItem.Text.text);
		}
	}

	void IInputListener.OnHandleAButton (int joystickIndex, bool pressed)
	{
	}

	void IInputListener.OnHandleBButton (int joystickIndex, bool pressed)
	{
	}
}
