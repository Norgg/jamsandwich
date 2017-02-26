using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jammable : MonoBehaviour {
	bool hasJam = false;
	public Mesh jammedMesh;

	void Start() {
	}

	public void Jam() {
		if (!hasJam) {
			hasJam = true;
			Debug.Log("Jammin");
			GetComponentInChildren<MeshFilter>().sharedMesh = jammedMesh;
		}
	}

	public bool HasJam() {
		return hasJam;
	}
}
