using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class JumpingState : CaterpillarState
{
    PlayerMovement caterpillar;
    Animator animator;
    private float jumpTimer;

    public JumpingState(PlayerMovement caterpillar)
    {
        this.caterpillar = caterpillar;
        animator = this.caterpillar.GetComponent<Animator>();
        animator.SetBool("Sticking", false);
        animator.SetBool("Walking", false);
        animator.SetBool("Jumping", true);
        this.jumpTimer = caterpillar.jumpTime;
    }

    public void FixedUpdate()
    {
		String axis = "Horizontal" + caterpillar.playerNum;
		caterpillar.ax = caterpillar.speed * Input.GetAxisRaw(axis);

        caterpillar.vx += caterpillar.ax * Time.fixedDeltaTime;
        caterpillar.vx *= caterpillar.friction;

        caterpillar.vy *= caterpillar.jumpArcFriction;

        Debug.Log("Time left: " + jumpTimer);
        jumpTimer -= Time.fixedDeltaTime;

		if (jumpTimer <= 0 || !Input.GetButton(axis))
        {
            caterpillar.ChangeState(new WalkingState(caterpillar));
            return;
        }

        caterpillar.SideCollisionLeft();
        caterpillar.SideCollisionRight();

        if (caterpillar.vy < 0 && caterpillar.GroundCollision())
        {
            Debug.Log("ground or low vy");
            caterpillar.ChangeState(new WalkingState(caterpillar));
            return;
        }

        if (caterpillar.vy >= 0 && caterpillar.CollisionTop())
        {
            Debug.Log("Top collision");
            caterpillar.vy = 0;
            caterpillar.ChangeState(new WalkingState(caterpillar));
            return;
        }

        caterpillar.transform.Translate(caterpillar.vx * Time.deltaTime, caterpillar.vy * Time.deltaTime, 0);
    }
}