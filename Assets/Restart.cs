using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour {

	float restartTime = 1;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		restartTime -= Time.deltaTime;
		if (restartTime <= 0 && Input.anyKey) {
			SceneManager.LoadScene("sandwich");
		}
	}
}
