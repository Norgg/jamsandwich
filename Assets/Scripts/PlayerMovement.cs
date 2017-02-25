using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement: MonoBehaviour {

    [HideInInspector] public float ax = 0;
    [HideInInspector] public float vx = 0;
    [HideInInspector] public float vy = 0;
    [HideInInspector] public bool jumpPressed = false;

    public float speed = 5;
    public float friction = 0.9f;
    public float gravity = 0.4f;
    public float maxDropSpeed = 300f;

    public float jumpVelocity = 4;
    public float jumpTime = 2;
    public float jumpArcFriction = 0.5f;

    public float collisionRayLength = 0.5f;
    public float rayPadding = 0.01f;
    public LayerMask collisionRayMask;

    public float onGroundLeeway = 0.5f;
    private CaterpillarState state;

	public int playerNum = 0;

	private Carry carry = null;

    void Start()
    {
        state = new WalkingState(this);
		carry = GetComponent<Carry>();
		Debug.Log(carry);
    }

    public void ChangeState(CaterpillarState newState)
    {
        state = newState;
    }

    void FixedUpdate()
    {
        state.FixedUpdate();
        jumpPressed = false;
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump" + playerNum))
        {
            jumpPressed = true;
        }
		if (Input.GetButton("Fire" + playerNum)) {
			Debug.Log(carry);
			carry.Throw(new Vector2(100, 100));
		}
    }

    public RaycastHit2D CastAndDrawRay(Vector2 position, Vector2 direction, float length, LayerMask mask)
    {
        RaycastHit2D hit = Physics2D.Raycast(position, direction, length,mask);

        Color color;
        if (hit.collider != null)
        {
            color = Color.red;
        }
        else
        {
            color = Color.white;
        }
        Debug.DrawRay(position, direction * length, color);

        return hit;
    }

    public bool CollisionTop()
    {
        float playerLeft = transform.GetComponent<Collider2D>().bounds.min.x;
        float playerRight = transform.GetComponent<Collider2D>().bounds.max.x;
        float playerTop = transform.GetComponent<Collider2D>().bounds.max.y;
        float playerHeight = transform.GetComponent<Collider2D>().bounds.size.y;

        List<RaycastHit2D> topHits = new List<RaycastHit2D>();
        topHits.Add(CastAndDrawRay(new Vector2(playerLeft + rayPadding, transform.position.y), new Vector2(0, 1), collisionRayLength, collisionRayMask));
        topHits.Add(CastAndDrawRay(transform.position, new Vector2(0, 1), collisionRayLength, collisionRayMask));
        topHits.Add(CastAndDrawRay(new Vector2(playerRight - rayPadding, transform.position.y), new Vector2(0, 1), collisionRayLength, collisionRayMask));

        float maxY = Mathf.Infinity;
        foreach (RaycastHit2D hit in topHits)
        {
            if (hit.collider)
            {
                maxY = Mathf.Min(maxY, hit.collider.bounds.min.y);
            }
        }

        if (playerTop + vy * Time.deltaTime > maxY)
        {
            vy = 0;
            transform.position = new Vector2(transform.position.x, maxY - playerHeight / 2);
            return true;
        }

        return false;
    }

    public bool GroundCollision()
    {
        float playerLeft = transform.GetComponent<Collider2D>().bounds.min.x;
        float playerRight = transform.GetComponent<Collider2D>().bounds.max.x;
        float playerBottom = transform.GetComponent<Collider2D>().bounds.min.y;
        float playerHeight = transform.GetComponent<Collider2D>().bounds.size.y;

        List<RaycastHit2D> bottomHits = new List<RaycastHit2D>();
        bottomHits.Add(CastAndDrawRay(new Vector2(playerLeft + rayPadding, transform.position.y), new Vector2(0, -1), collisionRayLength, collisionRayMask));
        bottomHits.Add(CastAndDrawRay(transform.position, new Vector2(0, -1), collisionRayLength, collisionRayMask));
        bottomHits.Add(CastAndDrawRay(new Vector2(playerRight - rayPadding, transform.position.y), new Vector2(0, -1), collisionRayLength, collisionRayMask));

        float minY = -Mathf.Infinity;
        foreach (RaycastHit2D hit in bottomHits)
        {
            if (hit.collider)
            {
                minY = Mathf.Max(minY, hit.collider.bounds.max.y);
            }
        }

        if (playerBottom + vy * Time.deltaTime < minY)
        {
            vy = 0;
            transform.position = new Vector2(transform.position.x, minY + playerHeight / 2);
            return true;
        }

        return false;
    }

    public bool SideCollisionRight()
    {
        float playerRight = transform.GetComponent<Collider2D>().bounds.max.x;
        float playerTop = transform.GetComponent<Collider2D>().bounds.max.y;
        float playerBottom = transform.GetComponent<Collider2D>().bounds.min.y;
        float playerWidth = transform.GetComponent<Collider2D>().bounds.size.x;

        List<RaycastHit2D> rightHits = new List<RaycastHit2D>();
        rightHits.Add(CastAndDrawRay(new Vector2(transform.position.x, playerTop - rayPadding), new Vector2(1, 0), collisionRayLength, collisionRayMask));
        rightHits.Add(CastAndDrawRay(transform.position, new Vector2(1, 0), collisionRayLength, collisionRayMask));
        rightHits.Add(CastAndDrawRay(new Vector2(transform.position.x, playerBottom + rayPadding), new Vector2(1, 0), collisionRayLength, collisionRayMask));

        float maxX = Mathf.Infinity;
        foreach (RaycastHit2D hit in rightHits)
        {
            if (hit.collider)
            {
                maxX = Mathf.Min(maxX, hit.collider.bounds.min.x);
            }
        }

        if (playerRight + vx * Time.deltaTime > maxX)
        {
            vx = 0;
            transform.position = new Vector2(maxX - playerWidth / 2, transform.position.y);
            return true;
        }

        return false;
    }

    public bool SideCollisionLeft()
    {
        float playerLeft = transform.GetComponent<Collider2D>().bounds.min.x;
        float playerTop = transform.GetComponent<Collider2D>().bounds.max.y;
        float playerBottom = transform.GetComponent<Collider2D>().bounds.min.y;
        float playerWidth = transform.GetComponent<Collider2D>().bounds.size.x;

        List<RaycastHit2D> leftHits = new List<RaycastHit2D>();
        leftHits.Add(CastAndDrawRay(new Vector2(transform.position.x, playerTop - rayPadding), new Vector2(-1, 0), collisionRayLength, collisionRayMask));
        leftHits.Add(CastAndDrawRay(transform.position, new Vector2(-1, 0), collisionRayLength, collisionRayMask));
        leftHits.Add(CastAndDrawRay(new Vector2(transform.position.x, playerBottom + rayPadding), new Vector2(-1, 0), collisionRayLength, collisionRayMask));

        float minX = -Mathf.Infinity;
        foreach (RaycastHit2D hit in leftHits)
        {
            if (hit.collider)
            {
                minX = Mathf.Max(minX, hit.collider.bounds.max.x);
            }
        }

        if (playerLeft + vx * Time.deltaTime < minX)
        {
            vx = 0;
            transform.position = new Vector2(minX + playerWidth / 2, transform.position.y);
            return true;
        }

        return false;
    }
}
