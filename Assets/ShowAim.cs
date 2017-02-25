using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowAim : MonoBehaviour {

	Transform aimer;

	// Use this for initialization
	void Start() {
		aimer = transform.FindChild("aimer");
	}

	public void SetAim(float x, float y) {
		float angle = Mathf.Rad2Deg * Mathf.Atan2(x, y);
		aimer.rotation = Quaternion.Euler(0, 0, 180+angle);
		float scale = new Vector2(x, y).magnitude;
		aimer.localScale = new Vector3(scale, scale, 1);
	}
}
