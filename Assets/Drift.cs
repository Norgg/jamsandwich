                                                                                                                                                                                                                                                                                                                                                                                                                        using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Drift : MonoBehaviour {
	Vector3 initialPos;
	float t = 0;

	void Start() {
		initialPos = transform.position;
	}

	void Update() {
		t += Time.deltaTime;
		transform.position = initialPos + new Vector3(Mathf.Sin(t * 4) / 4, Mathf.Cos(t * 5) / 4, 0);
	}

}