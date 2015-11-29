using UnityEngine;
using System.Collections;

public class QixBehaviour : MonoBehaviour
{
	Vector3 destination, desiredScale;
	float rotTimer, rotation, startMoveTime, journeyDistance, scaleDiff, startScaleTime;
	bool newDestination, newScale, rotDirection;
	
	public float maxRotTime = 1f, rotSpeed, maxScale = 3f, minScale, speed = 1f, scaleSpeed = 0.1f, gridXUpper, gridXLower, gridYUpper, gridYLower;


	void Start ()
	{
		//generate initial destination
		destination = new Vector3 (RandomFloat (gridXLower, gridXUpper), RandomFloat(gridYLower, gridYUpper), 0f);
		journeyDistance = Vector3.Distance (transform.position, destination);
		startMoveTime = Time.time;

		//generate initial rotation (false = anticlockwise, true = clockwise)
		rotTimer = RandomFloat(0f, maxRotTime);
		if (RandomInt (0, 2) == 1)
		{
			rotDirection = true;
		}
		else
		{
			rotDirection = false;
		}

		//generate desired scale
		desiredScale = new Vector3 (RandomFloat (minScale, maxScale), transform.localScale.y, 0f);
		scaleDiff = Vector3.Distance (transform.localScale, desiredScale);
		startScaleTime = Time.time;
	}

	void Update ()
	{
		//if called to designate a new destination, regen a destination
		if (newDestination)
		{
			destination = new Vector3 (RandomFloat (gridXLower, gridXUpper), RandomFloat (gridYLower, gridYUpper), 0f);
			journeyDistance = Vector3.Distance (transform.position, destination);
			startMoveTime = Time.time;
			newDestination = false;
		}

		//if called to designate a new scale, regen a destination
		if (newScale)
		{
			//generate desired scale
			desiredScale = new Vector3 (RandomFloat (minScale, maxScale), transform.localScale.y, 0f);
			scaleDiff = Vector3.Distance (transform.localScale, desiredScale);
			startScaleTime = Time.time;
			newScale = false;
		}

		//if the rotation timer is done, regen rotation values
		if (rotTimer <= 0)
		{
			rotTimer = RandomFloat(0f, maxRotTime);
			if (RandomInt (0, 2) == 1)
			{
				rotDirection = true;
			}
			else
			{
				rotDirection = false;
			}
		}
		//rotate
		Rotate ();

		//move
		ToDestination ();

		//scale
		Scale ();
	}

	float RandomFloat (float _min, float _max)
	{
		float value;

		//Random.seed = System.DateTime.Now.Millisecond;

		value = Random.Range (_min, _max);

		return value;
	}

	int RandomInt (float _min, float _max)
	{
		int value;
		
		//Random.seed = System.DateTime.Now.Millisecond;
		
		value = (int) Random.Range (_min, _max);
		
		return value;
	}

	void ToDestination ()
	{
		float distCovered = (Time.time - startMoveTime) * speed;
		float fracJourney = distCovered / journeyDistance;
		transform.position = Vector3.Lerp (transform.position, destination, fracJourney);

		//once the Qix is acceptably close
		if (Vector3.Distance(transform.position, destination) <= 1f)
		{
			newDestination = true;
		}
	}

	void Scale ()
	{
		float scalingDone = (Time.time - startScaleTime) * scaleSpeed;
		float fracJourney = scalingDone / scaleDiff;
		transform.localScale = Vector3.Lerp (transform.localScale, desiredScale, fracJourney);
		
		//once the Qix is acceptably close
		if (Vector3.Distance(transform.localScale, desiredScale) <= 0.1f)
		{
			newScale = true;
		}
	}

	void Rotate ()
	{
		int multiplier = 1;
		if (!rotDirection)
		{
			multiplier = -1;
		}
		transform.Rotate (transform.forward, rotSpeed * multiplier);

		rotTimer -= Time.deltaTime;
	}

	void OnCollisionEnter (Collision col)
	{
		//if collision with the player occurs, disable player for now
		if (col.gameObject.tag == "Player")
		{
			col.gameObject.SetActive(false);
		}
	}

}
