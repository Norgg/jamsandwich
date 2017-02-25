using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sandwich : MonoBehaviour {
	public GameObject sandwichFab;
	bool collidedAlready = false;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetCollided() {
		collidedAlready = true;
	}

	void OnCollisionEnter2D(Collision2D collision) {
		if (collision.gameObject.CompareTag("Bread")) {
			if (!collidedAlready) {
				GameObject.Instantiate(sandwichFab, transform.position, Quaternion.identity);
				GameObject.Destroy(gameObject);
				GameObject.Destroy(collision.gameObject);
				collision.gameObject.GetComponent<Sandwich>().SetCollided();
			}
		}
	}
}
