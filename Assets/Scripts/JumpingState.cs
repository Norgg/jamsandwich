using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class JumpingState : CaterpillarState
{
    PlayerMovement caterpillar;
    private float jumpTimer;

    public JumpingState(PlayerMovement caterpillar)
    {
        this.caterpillar = caterpillar;
        this.jumpTimer = caterpillar.jumpTime;
    }

    public void FixedUpdate()
    {
		caterpillar.ax = caterpillar.speed * VfigInput.GetAxis(caterpillar.playerNum - 1, VfigInput.Axis.LeftStickX);

        caterpillar.vx += caterpillar.ax * Time.fixedDeltaTime;
        caterpillar.vx *= caterpillar.friction;

        caterpillar.vy *= caterpillar.jumpArcFriction;

        jumpTimer -= Time.fixedDeltaTime;

		if (jumpTimer <= 0 || !VfigInput.GetButton(caterpillar.playerNum - 1, VfigInput.Button.A))
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