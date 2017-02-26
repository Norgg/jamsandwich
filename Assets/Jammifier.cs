using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jammifier : MonoBehaviour {
	void OnTriggerEnter2D(Collider2D other) {
		Jammable jammable = other.transform.GetComponent<Jammable>();
		if (jammable != null) {
			jammable.Jam();
		}
	}
}
