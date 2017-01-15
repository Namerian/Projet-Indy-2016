using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour
{
	private SpriteRenderer _renderer;

	private bool _isActive = true;
	private List<ILightEmitter> _lightEmitters;

	// Use this for initialization
	void Start ()
	{
		_renderer = this.GetComponent<SpriteRenderer> ();
		_renderer.enabled = true;

		_lightEmitters = new List<ILightEmitter> ();

		Bounds bounds = _renderer.bounds;

		GameObject[] machines = GameObject.FindGameObjectsWithTag ("Machine");

		foreach (GameObject machine in machines) {
			ILightEmitter emitter = machine.GetComponent<ILightEmitter> ();

			if (emitter != null && bounds.Contains (machine.transform.position) && !_lightEmitters.Contains (emitter)) {
				_lightEmitters.Add (emitter);
				//Debug.Log ("Shadow:Start:found a lamp");
			}
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		bool foundActiveEmitter = false;

		foreach (ILightEmitter emitter in _lightEmitters) {
			if (emitter.IsEmittingLight ()) {
				foundActiveEmitter = true;
				break;
			}
		}

		if (!_isActive && !foundActiveEmitter) {
			_isActive = true;
			_renderer.enabled = true;
		} else if (_isActive && foundActiveEmitter) {
			_isActive = false;
			_renderer.enabled = false;
		}
	}

	void OnCollisionEnter2D (Collision2D collision)
	{
		ILightEmitter emitter = collision.gameObject.GetComponent<ILightEmitter> ();

		if (emitter != null && !_lightEmitters.Contains (emitter)) {
			_lightEmitters.Add (emitter);
		}
	}

	void OnCollisionExit2D (Collision2D collision)
	{
		ILightEmitter emitter = collision.gameObject.GetComponent<ILightEmitter> ();

		if (emitter != null) {
			_lightEmitters.Remove (emitter);
		}
	}
}
