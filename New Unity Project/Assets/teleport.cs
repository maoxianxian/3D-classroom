using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class teleport : MonoBehaviour {
	GameObject ground;
	RaycastHit hit;
	GameObject lefthand;
	GameObject left_far_hand;
	GameObject righthand;
	GameObject current=null;
	GameObject closePlayer;
	GameObject farPlayer;
	Color lineColor=new Color(1,1,1);
	float holdtime=0;
	bool raycastmode=true;
	// Use this for initialization
	void Start () {
		ground = GameObject.FindGameObjectWithTag ("floor");
		closePlayer = GameObject.FindGameObjectWithTag ("closeplayer");
		lefthand = closePlayer.transform.GetChild(2).gameObject;
		righthand = closePlayer.transform.GetChild(4).gameObject;
		farPlayer = GameObject.Find ("LocalAvatar 2");
		left_far_hand = farPlayer.transform.GetChild (2).gameObject;
		farPlayer.SetActive (false);
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

		//raycast
		if (raycastmode) {
			//teleport
			if(OVRInput.Get(OVRInput.Button.One,OVRInput.Controller.LTouch)){
				if (Physics.Raycast (lefthand.transform.position, lefthand.transform.forward, out hit)) {
					if (hit.transform.gameObject == ground) {
						GameObject player = GameObject.FindGameObjectWithTag ("Player");
						player.GetComponent<CharacterController> ().Move(hit.point-	player.transform.position);
					}
				}
			}
			//select
			holdtime += Time.deltaTime;
			if (OVRInput.Get (OVRInput.Button.Two, OVRInput.Controller.LTouch)) {
				if (Physics.Raycast (lefthand.transform.position, lefthand.transform.forward, out hit)) {
					if (current == null && holdtime > 1) {
						GameObject currenthit = hit.transform.gameObject;
						if (currenthit != ground && currenthit.tag != "wall") {
							current = currenthit;
							current.transform.parent = null;
							current.transform.SetParent (lefthand.transform);
							holdtime = 0;
							current.GetComponent<Rigidbody> ().isKinematic = true;
						} 
					} else {
						if (holdtime < 1) {
							holdtime += Time.deltaTime;
						} else {
							current.transform.SetParent (GameObject.Find ("wall").transform);
							current.GetComponent<Rigidbody> ().isKinematic = false;
							current = null;
							holdtime = 0;
						}
					}
				}
			}
			if (current != null) {
				current.transform.position += 0.1f * lefthand.transform.forward * OVRInput.Get (OVRInput.Axis2D.PrimaryThumbstick) [1];
			}
		} else {//gogo
			
		}
	}

	void switchmode()
	{
		if (raycastmode) {
			farPlayer.SetActive (true);
			closePlayer.SetActive (false);
			raycastmode = false;
		} else {
			farPlayer.SetActive (false);
			closePlayer.SetActive (true);
			raycastmode = true;
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
