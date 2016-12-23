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

	void Awake ()
	{
		this.CanvasGroup = this.GetComponent<CanvasGroup> ();
		this.CanvasGroup.alpha = 0;

		this._isActive = false;
		this._levelListPanel = this.transform.FindChild ("LevelList");
		this._menuLevelItemList = new List<MenuLevelItem> ();

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
		if (visible) {
			this.CanvasGroup.alpha = 1;
			this._isActive = true;

			LoadLevelScenes ();

			InputHandler.Instance.AddInputListener (this, InputHandler.JOYSTICK_NAMES [0]);
		} else {
			this.CanvasGroup.alpha = 0;
			this._isActive = false;
			InputHandler.Instance.RemoveInputListener (this);
		}
	}

	private void LoadLevelScenes ()
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
		if (_selectedItem != null) {
			_selectedItem.Background.color = Color.white;
			_selectedItem = null;
		}

		_selectedItem = _menuLevelItemList [index];
		_selectedItem.Background.color = Color.yellow;
	}

	#region IInputListener implementation

	void IInputListener.OnHandleLeftStick (int joystickIndex, Vector2 stickState)
	{
		if (joystickIndex == 0 && stickState.y != 0) {
			Debug.Log ("LevelSelectionMenu: LeftStick test");
		}
	}

	void IInputListener.OnHandleXButton (int joystickIndex, bool pressed)
	{
		if (joystickIndex == 0 && pressed && _selectedItem != null) {
			GameController.Instance.LoadLevel (_selectedItem.Text.text);
		}
	}

	void IInputListener.OnHandleAButton (int joystickIndex, bool pressed)
	{
	}

	void IInputListener.OnHandleBButton (int joystickIndex, bool pressed)
	{
	}

	#endregion
}
