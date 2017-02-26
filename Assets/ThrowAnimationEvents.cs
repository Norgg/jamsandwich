using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowAnimationEvents : MonoBehaviour {

	private ItsAliiiiiive itsAliiiiiive;

	void Start()
	{
		itsAliiiiiive = transform.parent.GetComponent<ItsAliiiiiive>();
		
	}

	public void ThrowObject(AnimationEvent evt) {
		itsAliiiiiive.ThrowObject(evt);
	}

	public void ThrowFinished(AnimationEvent evt) {
		itsAliiiiiive.ThrowFinished(evt);
	}
}
