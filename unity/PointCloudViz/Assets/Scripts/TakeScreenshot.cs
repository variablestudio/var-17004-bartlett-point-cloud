using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TakeScreenshot : MonoBehaviour {

	public int Resolution = 8;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update() {
		if (Input.GetKeyDown(KeyCode.Space)) {
			Application.CaptureScreenshot("Screenshot_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png", Resolution);
		}

	}

}
