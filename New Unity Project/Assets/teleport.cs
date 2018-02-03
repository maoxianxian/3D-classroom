using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class teleport : MonoBehaviour {
	GameObject ground;
	RaycastHit hit;
	float time=0;

	// Use this for initialization
	void Start () {
		ground = GameObject.FindGameObjectWithTag ("floor");
	}
	// Update is called once per frame
	void Update () {
		Debug.Log (transform.transform.rotation);
		if (Physics.Raycast (transform.position, transform.forward, out hit)) {
			//Debug.Log (hit.rigidbody.gameObject.transform.parent.gameObject.transform.parent.gameObject);
			if (hit.rigidbody.gameObject == ground) {
				time += Time.deltaTime;
				Debug.Log (time);
				if (time > 3) {
					time = 0;
					transform.position = hit.point;
				}
			} else {
				time = 0;
			}
		}
	}
}
