using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowAim : MonoBehaviour {

	Transform aimer;

	// Use this for initialization
	void Start() {
		aimer = transform.FindChild("aimer");
	}

	public void SetAim(Vector2 direction, float currentPower, float maxPower) {
		float angle = Mathf.Rad2Deg * Mathf.Atan2(direction.x, direction.y);
		aimer.rotation = Quaternion.Euler(0, 0, -angle);
        float proportionOfMax = currentPower / maxPower;
        float scale = proportionOfMax * 3.0f;
		aimer.localScale = new Vector3(scale, scale, 1);
	}
}
