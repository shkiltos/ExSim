using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonNavigation : MonoBehaviour {
    int index = 0;
    public int totalLines=3;
    public float yOffset = 30f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            if(index < totalLines-1)
            {
                index++;
                Vector2 position = transform.localPosition;
                position.y -= yOffset;
                transform.localPosition = position;
            }
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (index > 0)
            {
                index--;
                Vector2 position = transform.localPosition;
                position.y += yOffset;
                transform.localPosition = position;
            }
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (index == 0) return;
        }
    }
}
