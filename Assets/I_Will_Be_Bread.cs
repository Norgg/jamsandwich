using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class I_Will_Be_Bread : MonoBehaviour {
	public GameObject breadPrefab;

	public void BeBreads() {
		Transform slice1 = transform.GetChild(0);
		GameObject bread1 = GameObject.Instantiate(breadPrefab, slice1.position, slice1.rotation);
		bread1.GetComponent<Jammable>().Jam();
		bread1.GetComponent<I_Am_Bread>().RemoveEyes();
		var bread3d1 = bread1.GetComponent<I_Am_Bread>().BeAllYouCanBe();
		bread3d1.GetComponent<BreadInTheThirdDimension>().GroundImpulse();

		Transform slice2 = transform.GetChild(1);
		GameObject bread2 = GameObject.Instantiate(breadPrefab, slice2.position, slice2.rotation);
		bread2.GetComponent<Jammable>().Jam();
		bread2.GetComponent<I_Am_Bread>().RemoveEyes();
		var bread3d2 = bread2.GetComponent<I_Am_Bread>().BeAllYouCanBe();
		bread3d2.GetComponent<BreadInTheThirdDimension>().GroundImpulse();

		GameObject.Destroy(gameObject);
	}
}
