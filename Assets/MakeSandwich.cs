using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeSandwich : MonoBehaviour {
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
		if (collision.gameObject.CompareTag("Bread") && !collidedAlready) {
			Jammable jam1 = GetComponent<Jammable>();
			Jammable jam2 = collision.transform.GetComponent<Jammable>();
			collision.gameObject.GetComponent<MakeSandwich>().SetCollided();
			var beingCarried = gameObject.GetComponent<Carriable>().IsBeingCarried();
			var otherBeingCarried = collision.gameObject.GetComponent<Carriable>().IsBeingCarried();

			if (!beingCarried && !otherBeingCarried) {
				if (jam1 != null && jam2 != null && (jam1.HasJam() || jam2.HasJam())) {
					SuccessSandwich(collision.gameObject);
				} else {
					FailSandwich(collision.gameObject);
				}
			}
		}
	}

	void FailSandwich(GameObject other) {
		var bread3D = GetComponent<I_Am_Bread>().BeAllYouCanBe();
		bread3D.GetComponent<BreadInTheThirdDimension>().MidairImpulse(true);

		var otherBread3D = other.GetComponent<I_Am_Bread>().BeAllYouCanBe();
		otherBread3D.GetComponent<BreadInTheThirdDimension>().MidairImpulse(false);

		other.GetComponent<I_Am_Bread>().BeAllYouCanBe();
	}

	void SuccessSandwich(GameObject other) {
		var sanFab = GameObject.Instantiate(sandwichFab, transform.position, Quaternion.identity);
		sanFab.GetComponent<Rigidbody2D>().angularVelocity = 200;
		GameObject.Destroy(gameObject);
		GameObject.Destroy(other);
	}
}
