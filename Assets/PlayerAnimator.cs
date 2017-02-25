using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour {

    private Animator _animator;

    private bool _carrying = false;

    // Use this for initialization
    void Start () {
        _animator = GetComponentInChildren<Animator>();
    }

    void Update () {
        bool inputLeftPressed = Input.GetKey("left");
        bool inputRightPressed = Input.GetKey("right");
        bool inputJumpPressed = Input.GetKey("space");
        bool inputCarryToggled = Input.GetKeyDown("c");
        bool inputThrowTriggered = Input.GetKeyDown("t");

        float speed;
        if (inputLeftPressed && !inputRightPressed) {
            speed = -1.0f;
        } else if (!inputLeftPressed && inputRightPressed) {
            speed = 1.0f;
        } else {
            speed = 0.0f;
        }

        bool jumping = inputJumpPressed;

        if (inputCarryToggled) {
            _carrying = !_carrying;
        }
        bool carrying = _carrying;

        bool throwNow = inputThrowTriggered;

        _animator.SetFloat("speed", speed);
        _animator.SetBool("carrying", carrying);
        _animator.SetBool("jumping", jumping);

        if (throwNow) {
            _animator.SetTrigger("throw");
        }
    }
}
