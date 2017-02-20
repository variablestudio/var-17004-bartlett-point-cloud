using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfluenceZone : MonoBehaviour {

	public float radius = 10.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnDrawGizmosSelected() {
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, this.radius);
	}
}
