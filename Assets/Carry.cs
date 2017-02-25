using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carry : MonoBehaviour {
	GameObject holding = null;
	Rigidbody2D holdingRB = null;
	Rigidbody2D rb;
	public Vector2 holdingOffset = new Vector2(0, 2);

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

	void OnCollisionEnter2D(Collision2D collision) {
		if (collision.gameObject.CompareTag("Bread")) {
			holding = collision.gameObject;
			holding.layer = LayerMask.NameToLayer("Carried");
			holdingRB = holding.GetComponent<Rigidbody2D>();
			holdingRB.isKinematic = true;
		}
	}
}
