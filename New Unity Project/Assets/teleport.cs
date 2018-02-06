using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class teleport : MonoBehaviour {
	GameObject ground;
	RaycastHit hit;
	GameObject lefthand;
	GameObject righthand;
	GameObject leftcurrent=null;
	GameObject closePlayer;
	GameObject farPlayer;
	GameObject leftpre;
	GameObject rightpre;
	Vector3 leftPrePos;
	Vector3 rightPrePos;
	float leftstilltime=0;
	float rightstilltime=0;
	Color lineColor=new Color(1,1,1);
	float leftholdtime=0;
	float rightholdtime=0;
	GameObject rightcurrent;
	bool raycastmode=true;
	// Use this for initialization
	void Start () {
		ground = GameObject.FindGameObjectWithTag ("floor");
		closePlayer = GameObject.FindGameObjectWithTag ("closeplayer");
		lefthand = closePlayer.transform.GetChild(2).gameObject;
		farPlayer = GameObject.Find ("LocalAvatar 2");
		righthand = farPlayer.transform.GetChild (4).gameObject;
		//farPlayer.SetActive (false);
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
		//select by raycast
		leftholdtime += Time.deltaTime;
		if (OVRInput.Get (OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.LTouch)) {
			if (Physics.Raycast (lefthand.transform.position, lefthand.transform.forward, out hit)) {
				if (leftcurrent == null && leftholdtime > 1) {
					GameObject currenthit = hit.transform.gameObject;
					if (currenthit != ground && currenthit.tag != "wall") {
						leftcurrent = currenthit;
						leftcurrent.transform.parent = null;
						leftcurrent.transform.SetParent (lefthand.transform);
						leftholdtime = 0;
						leftcurrent.GetComponent<Rigidbody> ().isKinematic = true;
					} 
				} else {
					if (leftholdtime > 1) {
						leftcurrent.transform.SetParent (GameObject.Find ("wall").transform);
						leftcurrent.GetComponent<Rigidbody> ().isKinematic = false;
						leftpre = leftcurrent;
						leftstilltime = 0;
						leftcurrent = null;
						leftholdtime = 0;
					}
				}
			}
		}


		if (leftcurrent != null) {
			leftcurrent.transform.position += 0.1f * lefthand.transform.forward * OVRInput.Get (OVRInput.Axis2D.PrimaryThumbstick) [1];
		}

		//select gogo left hand
		rightholdtime += Time.deltaTime;
		Vector3 righthandPos=closePlayer.transform.GetChild(5).transform.localPosition;
		farPlayer.transform.Translate ((righthandPos + new Vector3 (0.15f, 0, 0))*10.0f-farPlayer.transform.localPosition);
		DrawLine (righthand.transform.position, righthand.transform.position+righthand.transform.forward*0.5f, Color.blue);
		if (OVRInput.Get (OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch)!=0) {
			if (Physics.Raycast (righthand.transform.position, righthand.transform.forward,out hit,0.5f)) {
				if (rightcurrent == null && rightholdtime > 1) {
					GameObject currenthit = hit.transform.gameObject;
					if (currenthit != ground && currenthit.tag != "wall") {
						rightcurrent = currenthit;
						rightcurrent.transform.parent = null;
						rightcurrent.transform.SetParent (righthand.transform);
						rightholdtime = 0;
						rightcurrent.GetComponent<Rigidbody> ().isKinematic = true;
					} 
				} else {
					if (rightholdtime >1) {
						rightcurrent.transform.SetParent (GameObject.Find ("wall").transform);
						rightcurrent.GetComponent<Rigidbody> ().isKinematic = false;
						rightpre = rightcurrent;
						rightstilltime = 0;
						rightcurrent = null;
						rightholdtime = 0;
					}
				}
			}
		}

		//make object upright
		if (leftpre != null) {
			if (leftpre.transform.position != leftPrePos) {
				leftPrePos = leftpre.transform.position;
			} else {
				leftstilltime += Time.deltaTime;
				if (leftstilltime > 0.6f) {
					Vector3 axis = Vector3.Cross (new Vector3 (0, 1, 0), leftpre.transform.forward);
					float dot = Vector3.Dot (new Vector3 (0, 1, 0), Vector3.Normalize (leftpre.transform.up));
					Debug.Log (Mathf.Acos (dot) / Mathf.PI * 180.0f);
					leftpre.transform.RotateAround (leftpre.transform.position, axis, -Mathf.Acos (dot) / Mathf.PI * 180.0f);
					leftpre = null;
					leftstilltime = 0;
				}
			}
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
