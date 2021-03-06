﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatingArea : MonoBehaviour {
    public List<GameObject> sandwiches = new List<GameObject>();

    public void Update()
    {
    }

    void OnTriggerEnter2D(Collider2D other) {
		if (other.CompareTag("Sandwich")) {
			sandwiches.Add(other.gameObject);
            Destroy(other.gameObject.GetComponent<Carriable>());
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		if (sandwiches.Contains(other.gameObject)) {
			sandwiches.Remove(other.gameObject);
            other.gameObject.AddComponent<Carriable>();
		}
	}
}
