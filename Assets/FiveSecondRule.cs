﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiveSecondRule : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter2D(Collision2D collision) {
		if (collision.gameObject.CompareTag("Ground")
		|| (collision.gameObject.CompareTag("Plate") && !gameObject.CompareTag("Sandwich"))) {
			var breadController = GetComponent<I_Am_Bread>();
			if (breadController == null) {
				var sandwichController = GetComponent<I_Will_Be_Bread>();
				if (sandwichController == null) {
					GameObject.Destroy(this);
				} else {
					sandwichController.BeBreads();
				}
			} else {
				var bread3D = breadController.BeAllYouCanBe();
				bread3D.GetComponent<BreadInTheThirdDimension>().GroundImpulse();
			}

		}
	}
}
