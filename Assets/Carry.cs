using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carry : MonoBehaviour {
	GameObject holding = null;
	Rigidbody2D holdingRB = null;
	Rigidbody2D rb;
	public float throwPower = 300;
	public Vector2 holdingOffset = new Vector2(0, 1);

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void FixedUpdate() {
		if (holding != null) {
			holdingRB.position = rb.position + holdingOffset;
		}
	}

	public void Throw(Vector2 direction) {
		if (holding != null) {
			holdingRB.isKinematic = false;
			holdingRB.velocity = direction * throwPower;
			holdingRB.angularVelocity = (0.5f - Random.value) * 1000;
			holding = null;
		}
	}

	void OnCollisionEnter2D(Collision2D collision) {
		if (holding == null && collision.gameObject.GetComponent<Carriable>() != null) {
			holding = collision.gameObject;
			holdingRB = holding.GetComponent<Rigidbody2D>();
			holdingRB.isKinematic = true;
			holdingRB.angularVelocity = 0;
		}
	}
}
