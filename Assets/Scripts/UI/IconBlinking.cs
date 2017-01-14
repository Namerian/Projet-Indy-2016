using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconBlinking : MonoBehaviour
{
	public float _baseAlpha = 0.7f;
	public float _blinkSpeed = 3f;

	private float _blinkTimer;

	private CanvasGroup _canvasGroup;

	void Awake ()
	{
		_canvasGroup = this.GetComponent<CanvasGroup> ();
	}

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (_canvasGroup.alpha > 0) {
			float alpha = _baseAlpha + (1 - _baseAlpha) * Mathf.Sin (_blinkTimer * _blinkSpeed);

			_canvasGroup.alpha = alpha;

			_blinkTimer += Time.deltaTime;
		}
	}
}
