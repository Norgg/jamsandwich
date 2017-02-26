using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class I_Am_Bread : MonoBehaviour {
    public GameObject bread3DPrefab;

    void Start () {
        var googlies = GetComponentsInChildren<Googly>();
        // One in 100 chance of eyes
        if (Random.Range(0.0f, 1.0f) >= 0.01f) {
            for (int i = 0; i < googlies.Length; ++i) {
                var go = googlies[i].gameObject;
                Destroy(go);
            }
        }
    }

    public GameObject BeAllYouCanBe() {
        // Grow into the third dimension!
        GameObject bread3D = GameObject.Instantiate(bread3DPrefab, transform.position + 0.2f * Vector3.up, transform.rotation);
        Transform transform3D = bread3D.transform;
        Rigidbody body3D = bread3D.GetComponent<Rigidbody>();
        while (transform.childCount > 0) {
            var child = transform.GetChild(0);
            child.SetParent(transform3D, false);
        }
        Destroy(gameObject);
        return bread3D;
    }
}
