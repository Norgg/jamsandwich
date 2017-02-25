using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBread : MonoBehaviour {
	public GameObject breadFab;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate() {
		if (Random.value < 0.01f) {
			GameObject.Instantiate(breadFab, transform.position, Quaternion.identity);
		}
	}
}
