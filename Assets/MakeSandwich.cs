using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeSandwich : MonoBehaviour {
	public GameObject sandwichFab;
	bool collidedAlready = false;
	AudioSource splat;

	// Use this for initialization
	void Start () {
		splat = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetCollided() {
		collidedAlready = true;
	}

	void OnCollisionEnter2D(Collision2D collision) {
		if (collision.gameObject.CompareTag("Bread") && !collidedAlready) {
			Jammable jam1 = GetComponent<Jammable>();
			Jammable jam2 = collision.transform.GetComponent<Jammable>();
			if (jam1 != null && jam2 != null && jam1.HasJam() && jam2.HasJam()) {				
				SuccessSandwich(collision.gameObject);
			} else {
				FailSandwich(collision.gameObject);
			}
		}
	}

	void FailSandwich(GameObject other) {
	}

	void SuccessSandwich(GameObject other) {
		var beingCarried = gameObject.GetComponent<Carriable>().IsBeingCarried();
		var otherBeingCarried = other.GetComponent<Carriable>().IsBeingCarried();
		if(!beingCarried && !otherBeingCarried)
		{
			var sanFab = GameObject.Instantiate(sandwichFab, transform.position, Quaternion.identity);
			sanFab.GetComponent<Rigidbody2D>().angularVelocity = 200;
			GameObject.Destroy(gameObject);
			GameObject.Destroy(other);
			gameObject.GetComponent<MakeSandwich>().SetCollided();
		}
	}
}
