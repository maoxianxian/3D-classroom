using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class teleport : MonoBehaviour {
	GameObject ground;
	RaycastHit hit;
	GameObject lefthand;
	GameObject righthand;
	GameObject current=null;
	Color lineColor=new Color(1,1,1);
	float holdtime=0;

	// Use this for initialization
	void Start () {
		ground = GameObject.FindGameObjectWithTag ("floor");
		lefthand = GameObject.Find ("hand_left");
		righthand = GameObject.Find ("hand_right");

	}
	// Update is called once per frame
	void Update () {
		DrawLine (lefthand.transform.position, lefthand.transform.position+lefthand.transform.forward * 5, Color.blue);
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
		//teleport
		if(OVRInput.Get(OVRInput.Button.One,OVRInput.Controller.LTouch)){
			if (Physics.Raycast (lefthand.transform.position, lefthand.transform.forward, out hit)) {
				if (hit.transform.gameObject == ground) {
					GameObject player = GameObject.FindGameObjectWithTag ("Player");
					player.GetComponent<CharacterController> ().Move(hit.point-	player.transform.position);
				}
			}
		}
		//raycast select
		holdtime+=Time.deltaTime;
		if(OVRInput.Get(OVRInput.Button.Two,OVRInput.Controller.LTouch)){
			if (Physics.Raycast (lefthand.transform.position, lefthand.transform.forward, out hit)) {
				if (current == null && holdtime>1) {
					GameObject currenthit = hit.transform.gameObject;
					if (currenthit != ground && currenthit.tag != "wall") {
						current = currenthit;
						current.transform.parent = null;
						current.transform.SetParent (lefthand.transform);
						holdtime = 0;
						current.GetComponent<Rigidbody>().isKinematic = true;
					} 
				} else {
					if (holdtime < 1) {
						holdtime += Time.deltaTime;
					} else {
						current.transform.SetParent (GameObject.Find("wall").transform);
						current.GetComponent<Rigidbody>().isKinematic = false;
						current = null;
						holdtime = 0;
					}
				}
			}
		}
		if (current != null) {
			current.transform.position += 0.1f*lefthand.transform.forward * OVRInput.Get (OVRInput.Axis2D.PrimaryThumbstick) [1];
		}
	}

	void DrawLine(Vector3 start, Vector3 end,Color color,float duration=0.1f)
	{
		GameObject myline = new GameObject ();
		myline.transform.position = start;
		myline.AddComponent<LineRenderer> ();
		LineRenderer lr = myline.GetComponent<LineRenderer> ();
		lr.material = new Material (Shader.Find("Specular"));
		lr.SetColors (color,color);
		lr.SetWidth (0.01f, 0.01f);
		lr.SetPosition (0, start);
		lr.SetPosition (1, end);
		GameObject.Destroy (myline, duration);
	}
}
