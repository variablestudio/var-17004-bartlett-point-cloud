using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LatLonDataList : MonoBehaviour {

	[TextArea]
	public string data;
	public OpenStreetMap osm;

	// Use this for initialization
	void Start () {
		float[] values = Array.ConvertAll(this.data.Split(new Char[] {';', ',', ' ', '\n'}), Single.Parse);
		Debug.Log("LatLonDataList " + values.Length + " " + this.data);
		Material dataMaterial = Resources.Load ("Data", typeof(Material)) as Material;
		for (var i = 0; i < values.Length; i+=3) {
			Vector2 pos = osm.GetPosition(values[i + 1], values[i]);
			GameObject point = GameObject.CreatePrimitive (PrimitiveType.Cube);
			point.transform.localScale = new Vector3(5, values[i + 2] * 5, 5);
			point.transform.position = new Vector3(pos.x, 0, pos.y);
			point.gameObject.GetComponent<Renderer>().material = dataMaterial;
			point.transform.parent = this.transform;
		}

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
