using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class AirQuality : MonoBehaviour {

	public string token;
	public string stationId;
	public string property;

	// Use this for initialization
	IEnumerator Start () {
		Debug.Log("LoadData.Loading data");
		WWW w = new WWW("http://api.waqi.info/feed/@" + stationId + "/?token=" + token);
		yield return w;

		JsonData json = JsonMapper.ToObject(w.text);
		string name = (string)json["data"]["city"]["name"];
		Debug.Log("LoadData.name " + name);

		float value  = float.Parse(json["data"]["iaqi"][property]["v"].ToString());
		Vector3 scale = this.transform.localScale;
		scale.y *= value;
		this.transform.localScale = scale;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
