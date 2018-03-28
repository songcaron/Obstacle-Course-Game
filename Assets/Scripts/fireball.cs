using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityEngine.SceneManagement;

public class patrolenemy : MonoBehaviour {

	public AudioSource roar;
	public GameObject player;
	public GameObject deathCam;
	public GameObject patrol3;
	public GameObject patrol4;
	public Transform camPos;
	private NavMeshAgent nav;
	private string state = "idle";
	private bool alive = true;
	private bool atPoint = true;
	public Transform vision;
	private float wait = 0f;


	void Start () {
		nav = gameObject.GetComponent<NavMeshAgent> ();
		nav.speed = 10f;
		GoToNextPoint ();

	}
	//check if we can see player
	public void checkSight(){
		if (alive) {
			RaycastHit rayHit;
			if (Physics.Linecast (vision.position, player.transform.position, out rayHit))
				print("hit " + rayHit.collider.gameObject.name);
			if (rayHit.collider.gameObject.name == "player") {
				if (state != "kill") {
					state = "chase";
					nav.speed = 10f;
					roar.pitch = 1.2f;
					roar.Play ();
				}
			}
		}
	}
	void GoToNextPoint(){
		Vector3 patrolpoint3 = patrol3.transform.position; //picks a random place to walk to within a sphere of radius 20
		Vector3 patrolpoint4 = patrol4.transform.position;
		if (atPoint) {
			nav.destination = patrolpoint4;
			atPoint = false;
		} 
		else {
			nav.destination = patrolpoint3;
			atPoint = true;
		}
	}

	void Update () {
		//Debug.DrawLine (vision.position, player.transform.position, Color.green);
		if (alive) {
			//idle state 
			if (state == "idle") {
				//				Vector3 patrolpoint1 = patrol1.transform.position; 
				//				Vector3 patrolpoint2 = patrol2.transform.position;
				//				NavMeshHit hit;
				//				NavMesh.SamplePosition (patrolpoint2, out hit, 20f, NavMesh.AllAreas);

				//get near player bc on high alert
				//				if (highAlert) {
				//					NavMesh.SamplePosition (player.transform.position + patrolpoint1, out hit, 20f, NavMesh.AllAreas);
				//					//each time, lose awareness of player
				//					alertness += 5f;
				//					if (alertness > 20f) {
				//						highAlert = false;
				//						nav.speed = 1.2f;
				//					}
				//				}
				GoToNextPoint();
				state = "walk";

			}
			//walk state
			if (state == "walk") {
				if (nav.remainingDistance <= nav.stoppingDistance && !nav.pathPending) {
					state = "search";
					wait = 5f;
				}

			}
			//search state
			if (state == "search") {
				if (wait > 0f) {
					wait -= Time.deltaTime;
					transform.Rotate (0f, 120f * Time.deltaTime, 0f);
				} 
				else {
					state = "idle";
				}
			}

			//chase state
			if (state == "chase") {
				nav.destination = player.transform.position;
				//lose sight of player
				float distance = Vector3.Distance(transform.position, player.transform.position);
				if (distance > 10f) {
					state = "hunt";
				}
				//kill player
				else if (nav.remainingDistance <= nav.stoppingDistance + 1f && !nav.pathPending) {
					state = "kill";
					player.GetComponent<player> ().alive = false;
					player.GetComponent<ThirdPersonUserControl> ().enabled = false;
					deathCam.SetActive (true);
					deathCam.transform.position = Camera.main.transform.position;
					deathCam.transform.rotation = Camera.main.transform.rotation;
					Camera.main.gameObject.SetActive (false);
					roar.pitch = 0.7f;
					roar.Play ();
					Invoke ("changelevel", 2f);

				}
			}
			//hunt state
			if (state == "hunt") {
				if (nav.remainingDistance <= nav.stoppingDistance && !nav.pathPending) {
					state = "search";
					wait = 5f;
					checkSight ();
				}
			}
			//kill
			if (state == "kill") {
				deathCam.transform.position = Vector3.Slerp (deathCam.transform.position, camPos.position, 10f * Time.deltaTime);
				deathCam.transform.rotation = Quaternion.Slerp (deathCam.transform.rotation, camPos.rotation, 10f * Time.deltaTime);
				nav.SetDestination (deathCam.transform.position);
			}
			//nav.SetDestination (player.transform.position);
		}
	}
	//reset
	void changelevel(){
		Application.LoadLevel ("gameover scene");
	}
}
