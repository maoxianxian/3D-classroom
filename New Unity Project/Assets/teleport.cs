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
	float grouptime=0;
	bool finishgroup = false;
	bool endgroup=false;
	GameObject[] selectboxes;
	Queue<Material> matque;
	bool copying=false;
	float copytime=0;
	List<GameObject> currentgroup;
	GameObject copy;
	List<GameObject> copies;
	GameObject distancetext;
	Queue<Material> matcopy;
	GameObject ballone;
	GameObject balltwo;
	float disttime=0;
	// Use this for initialization
	void Start () {
		matque = new Queue<Material> ();
		ground = GameObject.FindGameObjectWithTag ("floor");
		closePlayer = GameObject.FindGameObjectWithTag ("closeplayer");
		lefthand = closePlayer.transform.GetChild(2).gameObject;
		farPlayer = GameObject.Find ("LocalAvatar 2");
		righthand = farPlayer.transform.GetChild (4).gameObject;
		selectboxes=new GameObject[3];
		selectboxes[0]=GameObject.Find ("group");
		selectboxes[1]=GameObject.Find ("copy");
		selectboxes[2]=GameObject.Find ("distance");
		distancetext = GameObject.FindGameObjectWithTag ("distance");
		distancetext.SetActive (false);
		currentgroup = new List<GameObject> ();
		copies = new List<GameObject> ();
		ballone = GameObject.Find ("pointone");
		ballone.SetActive (false);
		balltwo = GameObject.Find ("pointtwo");
		balltwo.SetActive (false);
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
						if (currenthit != ground && currenthit.tag != "wall" && currenthit.tag != "Player"&&currenthit.tag != "selectbox"&&currenthit.name!="room") {
							leftcurrent = currenthit;
							leftcurrent.transform.parent = null;
							leftholdtime = 0;
							hold (leftcurrent, lefthand);
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
				if (leftcurrent.GetComponent<Rigidbody>() != null) {
					leftcurrent.GetComponent<Rigidbody> ().velocity = Vector3.zero;
				}
				leftcurrent.transform.position += 0.1f * lefthand.transform.forward * OVRInput.Get (OVRInput.Axis2D.PrimaryThumbstick) [1];
			}

			//select gogo left hand
			rightholdtime += Time.deltaTime;
			Vector3 righthandPos = closePlayer.transform.GetChild (5).transform.localPosition;			
			farPlayer.transform.Translate ((righthandPos + new Vector3 (0.5f, -0.2f, -0.1f)) * 10.0f - farPlayer.transform.localPosition);
			DrawLine (righthand.transform.position, righthand.transform.position + righthand.transform.forward * 0.25f, Color.blue);
			if (OVRInput.Get (OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch) != 0) {
				if (Physics.Raycast (righthand.transform.position, righthand.transform.forward, out hit, 0.5f)) {
					if (rightcurrent == null && rightholdtime > 1) {
						GameObject currenthit = hit.transform.gameObject;
						if (currenthit != ground && currenthit.tag != "wall" && currenthit.tag != "Player"&&currenthit.tag != "selectbox") {
							rightcurrent = currenthit;
							rightcurrent.transform.parent = null;
							rightholdtime = 0;
							hold (rightcurrent, righthand);
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
				if (rightcurrent.GetComponent<Rigidbody>() != null) {
					rightcurrent.GetComponent<Rigidbody> ().velocity = Vector3.zero;
				}
			}
		}
		//make object upright
		straightFall();
		

		//grouping

		grouptime+=Time.deltaTime;
		if (currentmode == groupmode) {
			if (OVRInput.Get (OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.LTouch)&&!finishgroup) {
				if (grouptime > 0.5) {
					if (Physics.Raycast (lefthand.transform.position, lefthand.transform.forward, out hit)) {
						GameObject currenthit = hit.transform.gameObject;
						if (currenthit != ground && currenthit.tag != "wall" && currenthit.tag != "Player" && currenthit.tag != "selectbox"&&!currentgroup.Contains(currenthit)) {
							currentgroup.Add (currenthit);

							Renderer[] rends=currenthit.GetComponentsInChildren<Renderer> ();
							foreach (Renderer r in rends) {
								Material[] ms = r.materials;
								foreach (Material m in ms){
									matque.Enqueue (new Material(m));
								}
							}
							setcolor (Color.red, currenthit);
							grouptime = 0;
						}
					}
				}
			}
			if (OVRInput.Get (OVRInput.Button.Two, OVRInput.Controller.LTouch)) {
				if (!finishgroup) {
					grouptime = 0;
					finishgroup = true;
					foreach (GameObject g in currentgroup) {
						hold (g, lefthand);
					}
				} else if (grouptime > 0.5) {
					if (!endgroup) {
						endgroup = true;
						grouptime = 0;
						resetGroupMat ();
					} 
				}
			}
			if (finishgroup&&!endgroup) {
				foreach (GameObject g in currentgroup) {
					g.transform.position += 0.1f * lefthand.transform.forward * OVRInput.Get (OVRInput.Axis2D.PrimaryThumbstick) [1];
					if (g.GetComponent<Rigidbody> () != null) {
						g.GetComponent<Rigidbody> ().velocity = Vector3.zero;
					}
				}
			}
			else if (endgroup&&grouptime > 1) {
				foreach (GameObject g in currentgroup) {
					pointup (g);
				}
				setMode (groupmode);
			}
		}

		//copy
		copytime += Time.deltaTime;
		if (copying) {
			if (currentmode == selectmode) {
				if (rightcurrent != null) {
					copy = GameObject.Instantiate (rightcurrent, rightcurrent.transform.position, righthand.transform.rotation, righthand.transform.parent);
					release (copy);
				}
			}
			if (currentmode == groupmode) {
				matcopy = new Queue<Material> (matque);
				foreach (GameObject g in currentgroup) {
					GameObject copyg=GameObject.Instantiate (g,g.transform.position,g.transform.rotation,g.transform.parent);
					release (copyg);
					Renderer[] mats = copyg.GetComponentsInChildren<Renderer> ();
					foreach (Renderer m in mats) {
						Material[] mat = m.materials;
						for (int i = 0; i < mat.Length; i++) {
							mat [i] = matcopy.Dequeue ();
						}
						m.materials = mat;
					}
					copies.Add (copyg);
				}
			}
			copying = false;
			copytime = 0;
		}
		if (copytime > 1) {
			setcolor (Color.white, selectboxes [1]);
			if (currentmode == selectmode) {
				if (copy != null) {
					pointup (copy);
					copy = null;
				}
			}
			if (currentmode == groupmode) {
				foreach (GameObject g in copies) {
					pointup (g);
				}
				copies.Clear ();
			}
		}
		//distance
		disttime+=Time.deltaTime;
		if (currentmode == distmode) {
			if (OVRInput.Get (OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.LTouch)) {
				if (disttime > 0.5) {
					if (Physics.Raycast (lefthand.transform.position, lefthand.transform.forward, out hit)) {
						if (ballone.activeInHierarchy==false) {
							ballone.SetActive (true);
							ballone.transform.position = hit.point;
							disttime = 0;
						} else if (balltwo.activeInHierarchy==false) {
							balltwo.SetActive (true);
							balltwo.transform.position = hit.point;
							UnityEngine.UI.Text tex=distancetext.GetComponent<UnityEngine.UI.Text>();
							tex.text = Vector3.Magnitude(ballone.transform.position-balltwo.transform.position).ToString();
							disttime = 0;
						}
					}
				}
			}
		}
	}

	void resetGroupMat(){
		if (matque.Count != 0) {
			foreach (GameObject g in currentgroup) {
				g.transform.SetParent (GameObject.Find ("wall").transform);
				Renderer[] mats = g.GetComponentsInChildren<Renderer> ();
				foreach (Renderer m in mats) {
					Material[] mat = m.materials;
					for (int i = 0; i < mat.Length; i++) {
						mat [i] = matque.Dequeue ();
					}
					m.materials = mat;
				}
				release (g);
			}
		}
	}
	void decideMode()
	{
		modetime += Time.deltaTime;
		if (modetime > 0.5) {
			if (OVRInput.Get (OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch)) {
				if (Physics.Raycast (lefthand.transform.position, lefthand.transform.forward, out hit)) {
					GameObject hitob = hit.transform.gameObject;
					if (hitob.tag == "selectbox") {
						modetime = 0;
						if (hitob.name == "group") {
							setMode (groupmode);
						}
						if (hitob.name == "copy") {
							if (copytime > 1) {
								copying = true;
								setcolor (Color.red, selectboxes [1]);
							}
						}
						if (hitob.name == "distance") {
							setMode (distmode);
						}
					}
				}
			}
		}
	}
		
	void hold(GameObject g, GameObject hand)
	{
		g.transform.SetParent (hand.transform);
		if (g.GetComponent<Rigidbody> () != null) {
			g.GetComponent<Rigidbody> ().useGravity = false;
			g.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeRotation;
			g.GetComponent<Rigidbody> ().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
		}
	}
	void release(GameObject g){
		g.transform.SetParent (GameObject.Find ("wall").transform);
		if (g.GetComponent<Rigidbody> () != null) {
			g.GetComponent<Rigidbody> ().useGravity = true;
			g.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
			g.GetComponent<Rigidbody> ().collisionDetectionMode = CollisionDetectionMode.Discrete;
		}
	}
	void setMode(int i)
	{
		if (currentmode == selectmode) {
			clearleftselection ();
			clearrightselection ();
		} else {
			setcolor (Color.white,selectboxes [currentmode - 1]);
		}
		if (currentmode == groupmode) {
			grouptime = 0;
			finishgroup = false;
			endgroup = false;
			resetGroupMat ();
			currentgroup.Clear ();
		}
		if (currentmode == distmode) {
			ballone.SetActive (false);
			balltwo.SetActive (false);
			distancetext.SetActive (false);
		}

		if (currentmode != i) {
			currentmode = i;
			setcolor (Color.red, selectboxes [i - 1]);
			if (i == groupmode) {
				finishgroup = false;
				endgroup = false;
				grouptime = 0;
			}
			if (i == distmode) {
				distancetext.SetActive (true);
				UnityEngine.UI.Text tex=distancetext.GetComponent<UnityEngine.UI.Text>();
				tex.text = "pick your points";
				disttime = 0;
			}
		} else {
			setcolor (Color.white, selectboxes [i - 1]);
			currentmode = selectmode;
		}


	}

	void setcolor(Color cor,GameObject obj)
	{
		Renderer[] render = obj.GetComponentsInChildren<Renderer> ();
		foreach (Renderer r in render){
			Material[] oldmats = r.materials;
			for(int i=0;i<oldmats.Length;i++) {
				Material newmat = new Material (oldmats[i]);
				newmat.SetColor ("_Color", cor);
				oldmats[i] = newmat;
			}
			r.materials = oldmats;
		}
	}

	void clearleftselection(){
		if (leftcurrent != null) {
			release (leftcurrent);
			leftpre = leftcurrent;
			leftstilltime = 0;
			leftcurrent = null;
			leftholdtime = 0;
		}
	}

	void clearrightselection(){
		if (rightcurrent != null) {
			release (rightcurrent);
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
					pointup (leftpre);
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
					pointup (rightpre);
					rightpre = null;
					rightstilltime = 0;
				}
			}
		}
	}

	void pointup(GameObject g)
	{
		Vector3 axis = Vector3.Cross (new Vector3 (0, 1, 0), g.transform.forward);
		float dot = Vector3.Dot (new Vector3 (0, 1, 0), Vector3.Normalize (g.transform.forward));
		if (g.GetComponent<Rigidbody> () != null) {
			g.GetComponent<Rigidbody> ().isKinematic = true;
		}
		g.transform.RotateAround (g.transform.position, axis, -Mathf.Acos (dot) / Mathf.PI * 180.0f);
		if (g.GetComponent<Rigidbody> () != null) {
			g.GetComponent<Rigidbody> ().isKinematic = false;
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
