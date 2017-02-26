using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreadInTheThirdDimension : MonoBehaviour {
    private Rigidbody rigidbody;
    private float lifetime = 60.0f;

    public void MidairImpulse(bool zForward) {
        // Impulse and rotate out of the sky
        var rigidbody = GetComponent<Rigidbody>();
        float direction = (zForward ? -1f : 1f);
        Vector3 force = new Vector3(0f, 0f, direction * Random.Range(1f, 5f));
        Vector3 torque = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        rigidbody.AddForce(force, ForceMode.Impulse);
        rigidbody.AddTorque(torque, ForceMode.Impulse);
    }

    public void GroundImpulse() {
        // Impulse and rotate from the ground
        var rigidbody = GetComponent<Rigidbody>();
        float direction = ((Random.Range(0, 2) == 0) ? -1f : 1f);
        Vector3 force = new Vector3(Random.Range(-2f, 2f), Random.Range(2f, 5f), direction * Random.Range(1f, 5f));
        Vector3 torque = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        rigidbody.AddForce(force, ForceMode.Impulse);
        rigidbody.AddTorque(torque, ForceMode.Impulse);
    }

    void Update() {
        lifetime -= Time.deltaTime;
        if (lifetime < 0f) {
            Destroy(gameObject);
        }
    }
}
