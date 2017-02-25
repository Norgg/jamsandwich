using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBread : MonoBehaviour {
	public GameObject breadFab;
	HashSet<GameObject> currentBreads = new HashSet<GameObject>();
	int spawnTime = 60;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate() {
		currentBreads.RemoveWhere(bread => bread == null);

		if (currentBreads.Count == 0) {
			if (spawnTime > 0) {
				spawnTime--;
			} else {
				currentBreads.Add(GameObject.Instantiate(breadFab, transform.position, Quaternion.identity));
				spawnTime = 60;
			}
		}
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.CompareTag("Bread")) {
			currentBreads.Add(other.gameObject);
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		if (currentBreads.Contains(other.gameObject)) {
			currentBreads.Remove(other.gameObject);
		}
	}
}
