using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItsAliiiiiive : MonoBehaviour {
	private Animator animator;
	private PlayerMovement playerMovement;
	private Carry carry;
	public bool flipped = false;

	public delegate void AnimationTriggerDelegate();

	AnimationTriggerDelegate throwObjectDelegate = null;
	AnimationTriggerDelegate throwFinishedDelegate = null;

	void Start () {
		animator = GetComponentInChildren<Animator>();
		playerMovement = GetComponent<PlayerMovement>();
		carry = GetComponent<Carry>();
		var controller = animator.runtimeAnimatorController;

		// Add events onto the throw animation
		foreach (var clip in controller.animationClips) {
			if (clip.name == "goalie_throw") {
				AnimationEvent throwEvent = new AnimationEvent();
				throwEvent.functionName = "ThrowObject";
				throwEvent.time = 0.28f;
				clip.AddEvent(throwEvent);

				AnimationEvent finishedEvent = new AnimationEvent();
				finishedEvent.functionName = "ThrowFinished";
				finishedEvent.time = 1.0f;
				clip.AddEvent(finishedEvent);
			}
		}

	}

	void Update () {
		animator.SetFloat("speed", flipped ? -playerMovement.vx : playerMovement.vx);
		animator.SetBool("carrying", carry.IsCarrying());
	}

	public void StartThrow(AnimationTriggerDelegate throwObject, AnimationTriggerDelegate throwFinished) {
		Debug.Log(@"StartThrow()");
		throwObjectDelegate = throwObject;
		throwFinishedDelegate = throwFinished;
		animator.SetTrigger("throw");
	}

	public void ThrowObject(AnimationEvent evt) {
		if (throwObjectDelegate != null) {
			Debug.Log(@"ThrowObject()");
			throwObjectDelegate();
			throwObjectDelegate = null;
		}
	}

	public void ThrowFinished(AnimationEvent evt) {
		if (throwFinishedDelegate != null) {
			Debug.Log(@"ThrowFinished()");
			throwFinishedDelegate();
			throwFinishedDelegate = null;
		}
	}
}
