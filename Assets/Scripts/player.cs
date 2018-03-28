using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityEngine.SceneManagement;

public class player : MonoBehaviour {

	public bool alive = true;

	void OnTriggerEnter(Collider other){
		if (other.gameObject.name == "Cone") {
			other.transform.parent.GetComponent<enemy> ().checkSight ();
		}
	}
	void OnCollisionEnter(Collision collision){
		if (collision.gameObject.tag == "ball") {
			SceneManager.LoadScene ("gameover scene");
		}
	}
	void OnTriggerWin(Collision win){
		if (win.gameObject.tag == "winTrigger") {
			SceneManager.LoadScene ("win scene");
		}
	}

}
