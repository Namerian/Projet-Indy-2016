using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingPlank : MonoBehaviour
{
    private static bool _INITIALIZED = false;
    private static Vector2 _LOWER_LEFT_POS;
    private static Vector2 _UPPER_RIGHT_POS;

    // Use this for initialization
    void Start()
    {
        if (!_INITIALIZED)
        {
            //_LOWER_LEFT_POS = Camera.main.ScreenToWorldPoint(new Vector3(-Screen.width * 0.5f, -Screen.height * 0.5f, 0f));
            //_UPPER_RIGHT_POS = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f));
            _LOWER_LEFT_POS = Camera.main.ScreenToWorldPoint(new Vector3(0f, 0f, 0f));
            _UPPER_RIGHT_POS = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f));
            _INITIALIZED = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = this.transform.position;

        if (pos.x < _LOWER_LEFT_POS.x - 1f || pos.x > _UPPER_RIGHT_POS.x + 1f || pos.y < _LOWER_LEFT_POS.y - 1f || pos.y > _UPPER_RIGHT_POS.y + 1f)
        {
            Destroy(this.gameObject);
        }
    }

    /*void OnTriggerEnter2d(Collider2D other)
    {
        //if (other.transform.tag == "Player")
        //{
            Debug.Log("Trigger enter");
            //Destroy(this.gameObject);
        //}
    }*/

    /*void OnCollisionEnter2d(Collision2D other)
    {
        //if (other.transform.tag == "Player")
        //{
            Debug.Log("Collision enter");
            //Destroy(this.gameObject);
        //}
    }*/
}
