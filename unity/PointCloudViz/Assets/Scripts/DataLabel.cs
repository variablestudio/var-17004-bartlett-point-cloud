using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[ExecuteInEditMode]
public class DataLabel : MonoBehaviour {
	[TextArea]
	public string data;

	public float animationSpeed = 1.0f;

	private string[] values;
	private float valueIndex = 0;
	private string label = "";

	// Use this for initialization
	void Start () {
		try {
			this.values = this.data.Split(new Char[] {'\n'});
		} catch (Exception e) {
			this.data = "" + e;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (values == null || values.Length == 0) return;

		int currIdx = (int)Mathf.Floor(this.valueIndex);

		currIdx %= this.values.Length;

		this.label = this.values[currIdx];
		this.valueIndex += Time.deltaTime * animationSpeed;
		if (this.valueIndex > this.values.Length) {
			this.valueIndex -= this.values.Length;
		}
	}

	void OnSceneGUI () {
		Handles.BeginGUI();

		//GUILayout.Window(2, new Rect(10, 0, 100, 100), (id)=> {
			// Content of window here
			GUILayout.Button("A Button");
		//}, "Title");

		Handles.EndGUI();
	}

	void OnGUI () {
		GUI.color = Color.white;
		//GUI.backgroundColor = Color.white;
		GUI.Button (new Rect (25, 25, 100, 30), this.label);
	}



}
