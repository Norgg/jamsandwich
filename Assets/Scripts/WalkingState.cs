using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class WalkingState : CaterpillarState
{
    private PlayerMovement caterpillar;
    private float onGroundTimer = 0;
	private Vector2 startScale;

    public WalkingState(PlayerMovement caterpillar)
    {
        this.caterpillar = caterpillar;
		startScale = caterpillar.transform.localScale;
    }
    
    public void FixedUpdate()
    {
		String axis = "Horizontal" + caterpillar.playerNum;
        caterpillar.ax = caterpillar.speed * Input.GetAxisRaw(axis);
        caterpillar.vx += caterpillar.ax * Time.deltaTime;
        caterpillar.vx *= caterpillar.friction;

        //Left and right collisions?
        caterpillar.SideCollisionLeft();
        caterpillar.SideCollisionRight();

        caterpillar.vy -= caterpillar.gravity * Time.deltaTime;
        if (caterpillar.vy < -caterpillar.maxDropSpeed * Time.deltaTime)
        {
            caterpillar.vy = -caterpillar.maxDropSpeed * Time.deltaTime;
        }

        if (caterpillar.vy < 0)
        {
            if (caterpillar.GroundCollision())
            {
                onGroundTimer = caterpillar.onGroundLeeway;
            }
        }
        else
        {
            caterpillar.CollisionTop();
            onGroundTimer = 0;
        }

        if (onGroundTimer > 0 && caterpillar.jumpPressed)
        {
            caterpillar.jumpPressed = false;
            caterpillar.vy = caterpillar.jumpVelocity * Time.deltaTime;

            caterpillar.ChangeState(new JumpingState(caterpillar));
            return;
        }

        onGroundTimer -= Time.deltaTime;
        caterpillar.transform.Translate(caterpillar.vx * Time.deltaTime, caterpillar.vy * Time.deltaTime, 0);
    }
}
