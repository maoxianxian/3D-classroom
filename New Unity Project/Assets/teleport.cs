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
	float modetime=0;
	GameObject rightcurrent;
	int currentmode = 0;
	int selectmode=0;
	int groupmode=1;
	int copymode=2;
	int distmode=3;
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
		decideMode ();

		DrawLine (lefthand.transform.position, lefthand.transform.position+lefthand.transform.forward * 5, Color.blue);
		if (Physics.Raycast (transform.position, transform.forward, out hit)) {
			if (hit.transform.gameObject == ground) {
			} else {
			}
		}

		//teleport
		if (currentmode==selectmode) {
			if (OVRInput.Get (OVRInput.Button.One, OVRInput.Controller.LTouch)) {
				if (Physics.Raycast (lefthand.transform.position, lefthand.transform.forward, out hit)) {
					if (hit.transform.gameObject == ground) {
						GameObject player = GameObject.FindGameObjectWithTag ("Player");
						player.GetComponent<CharacterController> ().Move (hit.point -	player.transform.position);
					}
				}
			}
			//select by raycast
			leftholdtime += Time.deltaTime;
			if (OVRInput.Get (OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.LTouch)) {
				if (Physics.Raycast (lefthand.transform.position, lefthand.transform.forward, out hit)) {
					if (leftcurrent == null && leftholdtime > 1) {
						GameObject currenthit = hit.transform.gameObject;
						if (currenthit != ground && currenthit.tag != "wall" && currenthit.tag != "Player"&&currenthit.tag != "selectbox") {
							leftcurrent = currenthit;
							leftcurrent.transform.parent = null;
							leftcurrent.transform.SetParent (lefthand.transform);
							leftlastLocation = leftcurrent.transform.position;
							leftholdtime = 0;
							leftcurrent.GetComponent<Rigidbody> ().useGravity = false;
							leftcurrent.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeRotation;
							leftcurrent.GetComponent<Rigidbody> ().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

							//leftcurrent.GetComponent<Rigidbody> ().detectCollisions = true;
						} 
					} else {
						if (leftholdtime > 1) {
							clearleftselection ();
						}
					}
				} else if (leftcurrent != null) {
					if (leftholdtime > 1) {
						clearleftselection ();
					}
				}
			}

			//thumbstick
			if (leftcurrent != null) {
				leftcurrent.GetComponent<Rigidbody> ().velocity = Vector3.zero;
				leftcurrent.transform.position += 0.1f * lefthand.transform.forward * OVRInput.Get (OVRInput.Axis2D.PrimaryThumbstick) [1];
			}

			//select gogo left hand
			rightholdtime += Time.deltaTime;
			Vector3 righthandPos = closePlayer.transform.GetChild (5).transform.localPosition;			
			farPlayer.transform.Translate ((righthandPos + new Vector3 (-0.5f, 0, 0)) * 10.0f - farPlayer.transform.localPosition);
			DrawLine (righthand.transform.position, righthand.transform.position + righthand.transform.forward * 0.5f, Color.blue);
			if (OVRInput.Get (OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch) != 0) {
				if (Physics.Raycast (righthand.transform.position, righthand.transform.forward, out hit, 0.5f)) {
					if (rightcurrent == null && rightholdtime > 1) {
						GameObject currenthit = hit.transform.gameObject;
						if (currenthit != ground && currenthit.tag != "wall" && currenthit.tag != "Player"&&currenthit.tag != "selectbox") {
							rightcurrent = currenthit;
							rightcurrent.transform.parent = null;
							rightcurrent.transform.SetParent (righthand.transform);
							rightholdtime = 0;
							rightcurrent.GetComponent<Rigidbody> ().useGravity = false;
							rightcurrent.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeRotation;
							rightcurrent.GetComponent<Rigidbody> ().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

							//rightcurrent.GetComponent<Rigidbody> ().isKinematic = true;
						} 
					} else {
						if (rightholdtime > 1) {
							clearrightselection ();
						}
					}
				} else if (rightcurrent != null) {
					if (rightholdtime > 1) {
						clearrightselection ();
					}
				}
			}

			if (rightcurrent != null) {
				rightcurrent.GetComponent<Rigidbody> ().velocity = Vector3.zero;
			}
		}
		//make object upright
		straightFall();
		

		//grouping
		if (currentmode == groupmode) {
			
		}
	}

	void decideMode()
	{
		modetime += Time.deltaTime;
		if (modetime > 1) {
			if (OVRInput.Get (OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch)) {
				if (Physics.Raycast (lefthand.transform.position, lefthand.transform.forward, out hit)) {
					GameObject hitob = hit.transform.gameObject;
					if (hitob.tag == "selectbox") {
						modetime = 0;
						Debug.Log (hitob.name);

						if (hitob.name == "group") {
							if (currentmode != groupmode) {
								currentmode = groupmode;
								Renderer render = hitob.GetComponent<Renderer> ();
								Material newmat = new Material(render.material);
								Color red = new Color (1, 0, 0, 1);
								newmat.SetColor ("_Color", red);
								render.material = newmat;
								clearleftselection ();
								clearrightselection ();
							} else {
								Renderer render = hitob.GetComponent<Renderer> ();
								Material newmat = new Material(render.material);
								Color white = new Color (1, 1, 1, 1);
								newmat.SetColor ("_Color", white);
								render.material = newmat;
								currentmode = selectmode;
							}
						}
					}
				}
			}
		}
	}

	void clearleftselection(){
		if (leftcurrent != null) {
			leftcurrent.transform.SetParent (GameObject.Find ("wall").transform);
			leftcurrent.GetComponent<Rigidbody> ().useGravity = true;
			leftcurrent.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
			leftcurrent.GetComponent<Rigidbody> ().collisionDetectionMode = CollisionDetectionMode.Discrete;
			leftpre = leftcurrent;
			leftstilltime = 0;
			leftcurrent = null;
			leftholdtime = 0;
		}
	}
	void clearrightselection(){
		if (rightcurrent != null) {
			rightcurrent.transform.SetParent (GameObject.Find ("wall").transform);
			rightcurrent.GetComponent<Rigidbody> ().useGravity = true;
			rightcurrent.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
			rightcurrent.GetComponent<Rigidbody> ().collisionDetectionMode = CollisionDetectionMode.Discrete;
			rightpre = rightcurrent;
			rightstilltime = 0;
			rightcurrent = null;
			rightholdtime = 0;
		}
	}
	void straightFall()
	{
		if (leftpre != null) {
			if (leftpre.transform.position != leftPrePos) {
				leftPrePos = leftpre.transform.position;
			} else {
				leftstilltime += Time.deltaTime;
				if (leftstilltime > 0.6f) {
					Vector3 axis = Vector3.Cross (new Vector3 (0, 1, 0), leftpre.transform.forward);
					float dot = Vector3.Dot (new Vector3 (0, 1, 0), Vector3.Normalize (leftpre.transform.forward));
					leftpre.GetComponent<Rigidbody> ().isKinematic = true;
					leftpre.transform.RotateAround (leftpre.transform.position, axis, -Mathf.Acos (dot) / Mathf.PI * 180.0f);
					leftpre.GetComponent<Rigidbody> ().isKinematic = false;
					leftpre = null;
					leftstilltime = 0;
				}
			}
		}
		if (rightpre != null) {
			if (rightpre.transform.position != rightPrePos) {
				rightPrePos = rightpre.transform.position;
			} else {
				rightstilltime += Time.deltaTime;
				if (rightstilltime > 0.6f) {
					Vector3 axis = Vector3.Cross (new Vector3 (0, 1, 0), rightpre.transform.forward);
					float dot = Vector3.Dot (new Vector3 (0, 1, 0), Vector3.Normalize (rightpre.transform.forward));
					rightpre.GetComponent<Rigidbody> ().isKinematic = true;
					rightpre.transform.RotateAround (rightpre.transform.position, axis, -Mathf.Acos (dot) / Mathf.PI * 180.0f);
					rightpre.GetComponent<Rigidbody> ().isKinematic = false;
					rightpre = null;
					rightstilltime = 0;
				}
			}
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
