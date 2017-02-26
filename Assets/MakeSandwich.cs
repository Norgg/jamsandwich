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
            var beingCarried = gameObject.GetComponent<Carriable>().IsBeingCarried();
            var otherBeingCarried = collision.gameObject.GetComponent<Carriable>().IsBeingCarried();
            if(!beingCarried && !otherBeingCarried)
            {
                var sanFab = GameObject.Instantiate(sandwichFab, transform.position, Quaternion.identity);
                sanFab.GetComponent<Rigidbody2D>().angularVelocity = 200;
                GameObject.Destroy(gameObject);
                GameObject.Destroy(collision.gameObject);
                collision.gameObject.GetComponent<MakeSandwich>().SetCollided();
            }
		}
	}
}
