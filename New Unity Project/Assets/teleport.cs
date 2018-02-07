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
	Vector3 leftlastLocation;
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
					if (currenthit != ground && currenthit.tag != "wall"&&currenthit.tag!="Player") {
						leftcurrent = currenthit;
						leftcurrent.transform.parent = null;
						leftcurrent.transform.SetParent (lefthand.transform);
						leftlastLocation = leftcurrent.transform.position;
						leftholdtime = 0;
						//leftcurrent.GetComponent<Rigidbody> ().isKinematic = true;
						leftcurrent.GetComponent<Rigidbody> ().useGravity = false;
						leftcurrent.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeRotation;
						leftcurrent.GetComponent<Rigidbody> ().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

						//leftcurrent.GetComponent<Rigidbody> ().detectCollisions = true;
					} 
				} else {
					if (leftholdtime > 1) {
						leftcurrent.transform.SetParent (GameObject.Find ("wall").transform);
						//leftcurrent.GetComponent<Rigidbody> ().isKinematic = false;
						leftcurrent.GetComponent<Rigidbody> ().useGravity = true;
						leftcurrent.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeRotationX|RigidbodyConstraints.FreezeRotationY;
						leftcurrent.GetComponent<Rigidbody> ().collisionDetectionMode = CollisionDetectionMode.Discrete;

						leftpre = leftcurrent;
						leftstilltime = 0;
						leftcurrent = null;
						leftholdtime = 0;
					}
				}
			} else if(leftcurrent !=null){
				if (leftholdtime > 1) {
					leftcurrent.transform.SetParent (GameObject.Find ("wall").transform);
					//leftcurrent.GetComponent<Rigidbody> ().isKinematic = false;
					leftcurrent.GetComponent<Rigidbody> ().useGravity = true;
					leftcurrent.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeRotationX|RigidbodyConstraints.FreezeRotationY;
					leftcurrent.GetComponent<Rigidbody> ().collisionDetectionMode = CollisionDetectionMode.Discrete;

					leftpre = leftcurrent;
					leftstilltime = 0;
					leftcurrent = null;
					leftholdtime = 0;
				}
			}
		}


		if (leftcurrent != null) {
			leftcurrent.GetComponent<Rigidbody> ().velocity = Vector3.zero;
			leftcurrent.transform.position += 0.1f * lefthand.transform.forward * OVRInput.Get (OVRInput.Axis2D.PrimaryThumbstick) [1];
			//leftcurrent.GetComponent<Rigidbody> ().velocity = 0.1f * lefthand.transform.forward * OVRInput.Get (OVRInput.Axis2D.PrimaryThumbstick) [1];
		}

		//select gogo left hand
		rightholdtime += Time.deltaTime;
		Vector3 righthandPos=closePlayer.transform.GetChild(5).transform.localPosition;			
		farPlayer.transform.Translate ((righthandPos + new Vector3 (-0.5f, 0, 0))*10.0f-farPlayer.transform.localPosition);
		DrawLine (righthand.transform.position, righthand.transform.position+righthand.transform.forward*0.5f, Color.blue);
		if (OVRInput.Get (OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch)!=0) {
			if (Physics.Raycast (righthand.transform.position, righthand.transform.forward,out hit,0.5f)) {
				if (rightcurrent == null && rightholdtime > 1) {
					GameObject currenthit = hit.transform.gameObject;
					if (currenthit != ground && currenthit.tag != "wall"&&currenthit.tag!="Player") {
						rightcurrent = currenthit;
						rightcurrent.transform.parent = null;
						rightcurrent.transform.SetParent (righthand.transform);
						rightholdtime = 0;
						rightcurrent.GetComponent<Rigidbody> ().useGravity = false;
						rightcurrent.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeRotation;
						rightcurrent.GetComponent<Rigidbody> ().collisionDetectionMode=CollisionDetectionMode.ContinuousDynamic;

						//rightcurrent.GetComponent<Rigidbody> ().isKinematic = true;
					} 
				} else {
					if (rightholdtime >1) {
						rightcurrent.transform.SetParent (GameObject.Find ("wall").transform);
						//rightcurrent.GetComponent<Rigidbody> ().isKinematic = false;
						rightcurrent.GetComponent<Rigidbody> ().useGravity = true;
						rightcurrent.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeRotationX|RigidbodyConstraints.FreezeRotationY;
						rightcurrent.GetComponent<Rigidbody> ().collisionDetectionMode=CollisionDetectionMode.Discrete;

						rightpre = rightcurrent;
						rightstilltime = 0;
						rightcurrent = null;
						rightholdtime = 0;
					}
				}
			}else if(rightcurrent!=null){
				if (rightholdtime >1) {
					rightcurrent.transform.SetParent (GameObject.Find ("wall").transform);
					//rightcurrent.GetComponent<Rigidbody> ().isKinematic = false;
					rightcurrent.GetComponent<Rigidbody> ().useGravity = true;
					rightcurrent.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeRotationX|RigidbodyConstraints.FreezeRotationY;
					rightcurrent.GetComponent<Rigidbody> ().collisionDetectionMode=CollisionDetectionMode.Discrete;

					rightpre = rightcurrent;
					rightstilltime = 0;
					rightcurrent = null;
					rightholdtime = 0;
				}
			}
		}

		if (rightcurrent != null) {
			rightcurrent.GetComponent<Rigidbody> ().velocity = Vector3.zero;
		}
		//make object upright
		if (leftpre != null) {
			if (leftpre.transform.position != leftPrePos) {
				leftPrePos = leftpre.transform.position;
			} else {
				leftstilltime += Time.deltaTime;
				if (leftstilltime > 0.6f) {
					//Debug.Log (leftpre.transform.forward);
					Vector3 axis = Vector3.Cross (new Vector3 (0, 1, 0), leftpre.transform.forward);
					float dot = Vector3.Dot (new Vector3 (0, 1, 0), Vector3.Normalize (leftpre.transform.forward));
					//leftpre.GetComponentInChildren<BoxCollider> ().enabled=false;
					leftpre.GetComponent<Rigidbody>().isKinematic=true;
					//collider.enabled=false;

					leftpre.transform.RotateAround (leftpre.transform.position, axis, -Mathf.Acos (dot) / Mathf.PI * 180.0f);
					leftpre.GetComponent<Rigidbody>().isKinematic=false;

					//collider.enable = true;
					//leftpre.GetComponentInChildren<BoxCollider> ().enabled=true;

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
