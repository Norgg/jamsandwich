using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	public int playerNum = 0;

	Rigidbody2D rb;

	int jumpTimer = 0;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update() {
		Vector2 vel = rb.velocity;
		if (Input.GetKey(KeyCode.A)) {
			vel.x = -4;
		}
		if (Input.GetKey(KeyCode.D)) {
			vel.x = 4;
		}
		rb.velocity = vel;

		if (Input.GetKey(KeyCode.W)) {
			Jump();
		}

		if (jumpTimer > 0) {
			jumpTimer--;
		}
	}

	void Jump() {
		if (jumpTimer == 0) {
			rb.velocity += new Vector2(0, 10);
			jumpTimer = 60;
		}
	}
}
