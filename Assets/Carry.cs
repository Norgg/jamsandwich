using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carry : MonoBehaviour {
	GameObject holding = null;
	Rigidbody2D holdingRB = null;

	public float initialThrowPower = 1;
    public float throwGrowthSpeed = 1;
    public float maxThrowPower = 50;

    private float currentThrowPower;
	public Vector2 holdingOffset = new Vector2(0, 1);
    private int playerNum;

    private Vector2 currentAimDir;
	private ShowAim aim = null;
    private bool beginFiring = false;

	// Use this for initialization
	void Start () {
        playerNum = GetComponent<PlayerMovement>().playerNum;
		aim = GetComponent<ShowAim>();
        currentAimDir = new Vector2(0, 1.0f);
	}

    private void Update()
    {
        var newAimDir = new Vector2(Input.GetAxis("AimX" + playerNum), Input.GetAxis("AimY" + playerNum));
        if(newAimDir.magnitude > 0)
        {
            currentAimDir = Vector2.Lerp(currentAimDir, newAimDir.normalized, 0.25f);
        }

		aim.SetAim(currentAimDir, currentThrowPower, maxThrowPower);
        Debug.Log("Throwing " + currentAimDir);

        if (beginFiring)
        {
            currentThrowPower = Mathf.Clamp(currentThrowPower + throwGrowthSpeed * Time.deltaTime, 0, maxThrowPower);
        }

		if (Input.GetAxis("Fire" + playerNum) > 0 && !beginFiring) {
            beginFiring = true;
            currentThrowPower = initialThrowPower;
		}
        else if(Input.GetAxis("Fire" + playerNum) == 0 && beginFiring)
        {
            beginFiring = false;
			Throw(currentAimDir, currentThrowPower);
            currentThrowPower = initialThrowPower;
        }
    }
    // Update is called once per frame
    void FixedUpdate() {
		if (holding != null) {
            holdingRB.MovePosition((Vector2)transform.position + holdingOffset);
        }
    }

	public void Throw(Vector2 direction, float power) {
		if (holding != null) {
			holdingRB.isKinematic = false;
			holdingRB.velocity = direction * power;
			holdingRB.angularVelocity = (0.5f - Random.value) * 1000;
            holding.GetComponent<Carriable>().SetCarry(false);
			holding = null;
		}
	}

	void OnCollisionEnter2D(Collision2D collision) {
        var carriable = collision.gameObject.GetComponent<Carriable>();
		if (holding == null && carriable != null){
			holding = collision.gameObject;
            carriable.SetCarry(true);
			holdingRB = holding.GetComponent<Rigidbody2D>();
			holdingRB.isKinematic = true;
			holdingRB.angularVelocity = 0;
            holding.transform.rotation = Quaternion.Euler(0, 0, 0);
		}
	}
}
