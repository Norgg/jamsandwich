using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	public int playerNum = 0;
	public float speed = 4;
	public float jumpSpeed = 8;

	Rigidbody2D rb;

	int jumpTimer = 0;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update() {
		Vector2 vel = rb.velocity;

		if (playerNum == 1) {
			vel.x = Input.GetAxis("Horizontal1") * speed;
			rb.velocity = vel;
			Debug.Log(jumpTimer);
			if (Input.GetAxis("Jump1") > 0) {
				Jump();
			}
		} else {
			vel.x = Input.GetAxis("Horizontal2") * speed;
			rb.velocity = vel;
			Debug.Log(jumpTimer);
			if (Input.GetAxis("Jump2") > 0) {
				Jump();
			}
		}


		if (jumpTimer > 0) {
			jumpTimer--;
		}
	}

	void Jump() {
		if (jumpTimer <= 0) {
			Debug.Log("Jump!");
			rb.velocity += new Vector2(0, jumpSpeed);
			jumpTimer = 60;
		}
	}
}
