using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatingArea : MonoBehaviour {
    public List<GameObject> sandwiches = new List<GameObject>();

    public void Update()
    {
        print("Num of Sandwiches on plate: " + sandwiches.Count);
    }

    void OnTriggerEnter2D(Collider2D other) {
        print("Triggered");
		if (other.CompareTag("Sandwich") && !other.GetComponent<Carriable>().IsBeingCarried()) {
			sandwiches.Add(other.gameObject);
            Destroy(other.gameObject.GetComponent<Carriable>());
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		if (sandwiches.Contains(other.gameObject)) {
			sandwiches.Remove(other.gameObject);
		}
	}
}
