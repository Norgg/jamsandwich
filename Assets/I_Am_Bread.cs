using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class I_Am_Bread : MonoBehaviour {

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
}
