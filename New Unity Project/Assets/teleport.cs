using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class teleport : MonoBehaviour {
	GameObject ground;
	RaycastHit hit;

	// Use this for initialization
	void Start () {
		ground = GameObject.FindGameObjectWithTag ("floor");
	}
	
	// Update is called once per frame
	void Update () {
		//if (Physics.Raycast (transform.position, transform.forward, out hit)) {
			
		//}
	}
}
