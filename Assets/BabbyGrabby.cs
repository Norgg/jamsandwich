using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BabbyGrabby : MonoBehaviour {
    public PlatingArea platingArea;
    private bool movingTowardsPlate = false;
    private float towardsPlateMoveProgress = 0.0f;
    private Vector2 originalPos;
    private Vector2 plateVec;
    public float grabSpeed = 3.0f;
	public GameObject baby;
	Hunger babyHunger;

    public void Start()
    {
        originalPos = transform.position;
        plateVec = platingArea.gameObject.transform.position;
		babyHunger = baby.GetComponent<Hunger>();
    }

    public void Update()
    {
        // Check for sandwiches
        if(platingArea.sandwiches.Count > 0 && !movingTowardsPlate && towardsPlateMoveProgress <= 0.0)
        {
            movingTowardsPlate = true;
        }

        // Moving to grab
        if (movingTowardsPlate)
        {
            towardsPlateMoveProgress += Time.deltaTime * grabSpeed;
        }
        else
        {
            towardsPlateMoveProgress = Mathf.Max(0, towardsPlateMoveProgress - Time.deltaTime * grabSpeed);
        }

        if(towardsPlateMoveProgress >= 1 && movingTowardsPlate)
        {
			babyHunger.Eat(platingArea.sandwiches.Count);
            foreach(var sandwich in platingArea.sandwiches)
            {
                Destroy(sandwich.gameObject);
            }
            platingArea.sandwiches.Clear();

            movingTowardsPlate = false;
        }

        var lerpedPos = Vector2.Lerp(originalPos, plateVec, towardsPlateMoveProgress);
        transform.position = new Vector3(lerpedPos.x, lerpedPos.y, transform.position.z);
    }
}
