using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class teleport : MonoBehaviour {
	GameObject ground;
	RaycastHit hit;
	GameObject lefthand;
	GameObject righthand;
	//float time=0;

	// Use this for initialization
	void Start () {
		ground = GameObject.FindGameObjectWithTag ("floor");
		lefthand = GameObject.Find ("hand_left");
		righthand = GameObject.Find ("hand_right");

	}
	// Update is called once per frame
	void Update () {
		//Debug.Log (transform.transform.rotation);
		if (Physics.Raycast (transform.position, transform.forward, out hit)) {
			//Debug.Log (hit.rigidbody.gameObject.transform.parent.gameObject.transform.parent.gameObject);
			if (hit.transform.gameObject == ground) {
				//time += Time.deltaTime;
				//Debug.Log (time);
				//if (time > 3) {
					//time = 0;
					//GameObject player = GameObject.FindGameObjectWithTag ("Player");
					//player.GetComponent<CharacterController> ().Move(hit.point-	player.transform.position);
				//}
			} else {
				//time = 0;
			}
			//Debug.Log (OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch).eulerAngles);
			//Debug.Log(GameObject.Find ("hand_left").transform.forward);
			//Debug.Log(GameObject.Find ("controller_left").transform.forward);

		}

		if(OVRInput.Get(OVRInput.Button.One,OVRInput.Controller.LTouch)){
			if (Physics.Raycast (lefthand.transform.position, lefthand.transform.forward, out hit)) {
				if (hit.transform.gameObject == ground) {
					GameObject player = GameObject.FindGameObjectWithTag ("Player");
					player.GetComponent<CharacterController> ().Move(hit.point-	player.transform.position);
				}
			}
		}
	}
}
