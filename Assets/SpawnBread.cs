using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBread : MonoBehaviour {
	public GameObject breadFab;
	HashSet<GameObject> currentBreads = new HashSet<GameObject>();

	public Vector2 baseDirection = new Vector2(0, 400);
	public float angleVariation = 15;
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
				GameObject newBread = GameObject.Instantiate(breadFab, transform.position, Quaternion.identity);
				Rigidbody2D rb = newBread.GetComponent<Rigidbody2D>();
				Vector2 force = Quaternion.Euler(0, 0, (0.5f - Random.value) * angleVariation) * baseDirection;
				rb.AddForce(force);
				currentBreads.Add(newBread);
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
