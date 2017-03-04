using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carry : MonoBehaviour {
    private GameObject holding = null;
    Rigidbody2D holdingRB = null;
    private ItsAliiiiiive itsAliiiiiive;

    public float initialThrowPower = 1;
    public float throwGrowthSpeed = 1;
    public float maxThrowPower = 50;

    private float currentThrowPower;
    public Vector2 holdingOffset = new Vector2(0, 1);
    private int playerNum;

    private Vector2 currentAimDir;
    private ShowAim aim = null;
    private bool beginFiring = false;
    private bool animateFiring = false;
    private float catchDelay = 0.0f;

    // Use this for initialization
    void Start () {
	playerNum = GetComponent<PlayerMovement>().playerNum;
	aim = GetComponent<ShowAim>();
	currentAimDir = new Vector2(0, 1.0f);
        itsAliiiiiive = GetComponent<ItsAliiiiiive>();
    }

    private void Update()
    {
	var newAimDir = new Vector2(Input.GetAxis("AimX" + playerNum), Input.GetAxis("AimY" + playerNum));
	if(newAimDir.magnitude > 0)
	{
	    currentAimDir = Vector2.Lerp(currentAimDir, newAimDir.normalized, 0.25f);
	}

	aim.SetAim(currentAimDir, currentThrowPower, maxThrowPower);

	if (beginFiring)
	{
	    currentThrowPower = Mathf.Clamp(currentThrowPower + throwGrowthSpeed * Time.deltaTime, 0, maxThrowPower);
	}

	if (Input.GetAxis("Fire" + playerNum) < 0 && !beginFiring && !animateFiring) {
	    beginFiring = true;
	    currentThrowPower = initialThrowPower;
	}
	else if(Input.GetAxis("Fire" + playerNum) >= 0 && beginFiring && !animateFiring)
	{
            beginFiring = false;
            animateFiring = true;
            Vector2 aimDir = currentAimDir;
            float throwPower = currentThrowPower;
            itsAliiiiiive.StartThrow(
                // throwing point of animation
                () => { Throw(aimDir, throwPower); },
                // animation finished
                () => { animateFiring = false; }
            );
            currentThrowPower = initialThrowPower;
        }
    }
    // Update is called once per frame
    void FixedUpdate() {
	if (holding != null) {
	    holdingRB.MovePosition((Vector2)transform.position + holdingOffset);
	}

	// Countdown until we can catch again (fixes bread being caught before leaving the character!)
	catchDelay = Mathf.Max(0.0f, catchDelay - Time.fixedDeltaTime);
    }

    public void Throw(Vector2 direction, float power) {
	if (holding != null) {
	    holdingRB.isKinematic = false;
	    holdingRB.velocity = direction * power;
	    holdingRB.angularVelocity = (0.5f - Random.value) * 1000;
	    holding.GetComponent<Carriable>().SetCarry(false);
	    holding = null;
	    catchDelay = 0.25f;
	}
    }

    void OnCollisionEnter2D(Collision2D collision) {
	    var carriable = collision.gameObject.GetComponent<Carriable>();
	    if (holding == null && catchDelay == 0.0f && carriable != null) {
		holding = collision.gameObject;
		carriable.SetCarry(true);
		holdingRB = holding.GetComponent<Rigidbody2D>();
		holdingRB.isKinematic = true;
		holdingRB.angularVelocity = 0;
		holding.transform.rotation = Quaternion.Euler(0, 0, 0);
	    }
	}

    public bool IsCarrying() {
        return (holding != null);
    }
}
