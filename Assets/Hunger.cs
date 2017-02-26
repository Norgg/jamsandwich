using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hunger : MonoBehaviour {
	float hunger = 0;
	public float hungerSpeed = 0.001f;
	float maxHunger = 10;
	public float hungerPerSandwich = 1f;
	public GameObject hungerBar;
	
	// Update is called once per frame
	void FixedUpdate () {
		if (hunger < maxHunger) {
			hunger += hungerSpeed;
			Vector3 scale = hungerBar.transform.localScale;
			scale.x = hunger;
			hungerBar.transform.localScale = scale;
			Debug.Log(hunger);
			if (hunger > maxHunger / 2) {
				Material mat = hungerBar.GetComponent<MeshRenderer>().sharedMaterial;
				mat.color = new Color(1, 0, 0);
				hungerBar.GetComponent<MeshRenderer>().sharedMaterial = mat;
			}
		}
	}

	public void Eat(int sandwiches) {
		hunger -= sandwiches * hungerPerSandwich;
		if (hunger < 0) {
			hunger = 0;
		}
	}
}
