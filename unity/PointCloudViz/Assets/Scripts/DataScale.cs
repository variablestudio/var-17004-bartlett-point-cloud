using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DataScale : MonoBehaviour {
	[TextArea]
	public string data;

	public float animationSpeed = 1.0f;

	private float[] values;
	private float valueIndex = 0;
	private Vector3 initialScale;

	// Use this for initialization
	void Start () {
		try {
			string[] lines = this.data.Split(new Char[] {'\n'});
			this.values = new float[lines.Length];
			for (int i = 0; i < lines.Length; i++) {
				string[] tokens = lines[i].Split(new Char[] {' ', '\t'});
				values[i] = Single.Parse(tokens[0]);
			}
		} catch (Exception e) {
			this.data = "" + e;
		}

		initialScale = this.transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
		if (values == null || values.Length == 0) return;

		int currIdx = (int)Mathf.Floor(this.valueIndex);
		int nextIdx = (int)Mathf.Ceil(this.valueIndex);

		currIdx %= this.values.Length;
		nextIdx %= this.values.Length;

		float currValue = this.values[currIdx];
		float nextValue = this.values[nextIdx];

		float frac = this.valueIndex - Mathf.Floor(this.valueIndex);
		float value = Mathf.Lerp(currValue, nextValue, frac);

		this.transform.localScale = this.initialScale * value;

		this.valueIndex += Time.deltaTime * animationSpeed;
		if (this.valueIndex > this.values.Length) {
			this.valueIndex -= this.values.Length;
		}
	}
}
