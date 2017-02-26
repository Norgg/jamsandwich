using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BabbyGrabby : MonoBehaviour {
    public PlatingArea platingArea;
    private bool movingTowardsPlate = false;
    private float towardsPlateMoveProgress = 0.0f;
    private Vector2 originalPos;
    private Vector2 plateVec;
    public float grabSpeed = 3.0f;
	public GameObject baby;
	Hunger babyHunger;
	float grabPlayerProgress = 0.0f;

	bool grabPlayers = false;
	public GameObject playerToGrab;
	AudioSource crunchSound;

    public void Start()
    {
		crunchSound = GetComponent<AudioSource>();
        originalPos = transform.position;
        plateVec = platingArea.gameObject.transform.position;
		babyHunger = baby.GetComponent<Hunger>();
    }

    public void Update()
    {
		if (babyHunger.EatPlayers()) {
			grabPlayers = true;
		}
		if (grabPlayers) {
			grabPlayerProgress += Time.deltaTime * grabSpeed;
			var lerpedPos = Vector2.Lerp(originalPos, playerToGrab.transform.position, grabPlayerProgress);
			transform.position = new Vector3(lerpedPos.x, lerpedPos.y, transform.position.z);
			if (grabPlayerProgress >= 1) {
				SceneManager.LoadScene("munch");
			}
		} else {
			// Check for sandwiches
			if (platingArea.sandwiches.Count > 0 && !movingTowardsPlate && towardsPlateMoveProgress <= 0.0) {
				movingTowardsPlate = true;
			}

			// Moving to grab
			if (movingTowardsPlate) {
				towardsPlateMoveProgress += Time.deltaTime * grabSpeed;
			} else {
				towardsPlateMoveProgress = Mathf.Max(0, towardsPlateMoveProgress - Time.deltaTime * grabSpeed);
			}

			if (towardsPlateMoveProgress >= 1 && movingTowardsPlate) {
				babyHunger.Eat(platingArea.sandwiches.Count);
				foreach (var sandwich in platingArea.sandwiches) {
					Destroy(sandwich.gameObject);
				}
				platingArea.sandwiches.Clear();
				crunchSound.PlayDelayed(1 / grabSpeed);

				movingTowardsPlate = false;
			}

			var lerpedPos = Vector2.Lerp(originalPos, plateVec, towardsPlateMoveProgress);
			transform.position = new Vector3(lerpedPos.x, lerpedPos.y, transform.position.z);
		}
    }

	public void GrabPlayer() {
		grabPlayers = true;
	}
}
