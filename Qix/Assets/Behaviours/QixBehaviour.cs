using UnityEngine;
using System.Collections;

public class QixBehaviour : MonoBehaviour
{
	Vector3 destination, desiredScale;
	public Vector3 direction;
	float dirTimer, rotTimer, rotation, startMoveTime, journeyDistance, scaleDiff, startScaleTime;
	bool newDirection, newDestination, newScale, rotDirection;
	
	public float maxDirTime = 1f, maxRotTime = 1f, rotSpeed, maxScale = 3f, minScale, speed = 1f, scaleSpeed = 0.1f, gridXUpper, gridXLower, gridYUpper, gridYLower;


	void Start ()
	{
		//generate initial destination
		dirTimer = RandomFloat(0f, maxRotTime);

		Random.seed = (int)System.DateTime.Now.Millisecond;

		int tempDirection = Random.Range(0, 4);
		
		SetDirection (tempDirection);

//		direction = new Vector3 (RandomFloat (-1f, 1f), RandomFloat (-1f, 1f), 0f);

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
		//if called to designate a new direction, regen a destination
		if (newDirection || (dirTimer <= 0)) 
		{
			//if (newDirection)
			{
				Random.seed = (int)System.DateTime.Now.Millisecond;

				int tempDirection = Random.Range(0, 4);

				SetDirection (tempDirection);

				dirTimer = Random.Range(0f, maxDirTime);
			}

			//Random.seed = (int)System.DateTime.Now.Millisecond;

			//direction = new Vector3 (RandomFloat (-1f, 1f), RandomFloat (-1f, 1f), 0f);
			newDirection = false;
		}

		/*if (newDestination)
		{
			destination = new Vector3 (RandomFloat (gridXLower, gridXUpper), RandomFloat (gridYLower, gridYUpper), 0f);
			journeyDistance = Vector3.Distance (transform.position, destination);
			startMoveTime = Time.time;
			newDestination = false;
		}*/

		//if called to designate a new scale, regen a destination
		if (newScale)
		{
			//generate desired scale
			desiredScale = new Vector3 (RandomFloat (minScale, maxScale), 1f, 1f);
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
		//Rotate ();

		//move

		MoveDirection ();

		//ToDestination ();

		//scale
		//Scale ();

		transform.rotation = Quaternion.identity;
        if (transform.position.x < 0f)
        {
            transform.position = new Vector3(0f, transform.position.y, transform.position.z);
        }
        if (transform.position.x > 130f)
        {
            transform.position = new Vector3(130f, transform.position.y, transform.position.z);
        }
        if (transform.position.y < 0f)
        {
            transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
        }
        if (transform.position.y > 64f)
        {
            transform.position = new Vector3(transform.position.x, 64f, transform.position.z);
        }
    }



	void LateUpdate ()
	{
		int tempX, tempY;
		tempX = (int) transform.position.x;
		if ((transform.position.x - tempX) > 0.5f)
		{
			//tempX++;
		}
		tempY = (int) transform.position.y;
		if ((transform.position.y - tempY) > 0.5f)
		{
			//tempY++;
		}

		transform.position = new Vector3 (tempX, tempY, 0f);
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

	void MoveDirection ()
	{
		transform.position += direction * speed;

		dirTimer -= Time.deltaTime;

		if (dirTimer <= 0) {
			newDirection = true;
		}
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

	void OnTriggerEnter2D (Collider2D col)
	{
		//if collision with the player occurs, disable player for now
		if (col.gameObject.tag == "Player") {

			col.gameObject.GetComponent<CharMovement>().alive = false;

		} else if (col.gameObject.tag == "Node") {

			if (col.gameObject.GetComponent<GridElement>().m_node.state == NodeState.construction)
			{
				//this kills the player

				//find node
				int seekedPlayerIndex = col.gameObject.GetComponent<GridElement>().m_node.owner;
				
				GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
					
				for (int i = 0; i < allPlayers.Length; i++)
				{
					if (allPlayers[i].GetComponent<CharMovement>().playerIndex == seekedPlayerIndex)
					{
						allPlayers[i].GetComponent<CharMovement>().alive = false;
						break;
					}
				}
			}
			//if a blank element, ignore and move on
			if (col.gameObject.GetComponent<GridElement>().m_node.state == NodeState.inactive)
			{
				return;
			}
		}

		//change move direction
		newDestination = true;

		direction *= -1;


		if (rotDirection) {
			rotDirection = false;
		} else {
			rotDirection = true;
		}

	}

	void OnCollisionStay2D (Collision2D col)
	{
		//if collision with the player occurs, disable player for now
		if (col.gameObject.tag == "Player")
		{
			col.gameObject.SetActive(false);
		}
		
		//change move direction
		//newDestination = true;
		
		//direction *= -1;
		
		
		if (rotDirection) {
			rotDirection = false;
		} else {
			rotDirection = true;
		}
		
	}

	void SetDirection (int _direction)
	{
		switch (_direction) {
		case 0: //up (0, 1, 0)
			direction = new Vector3(0, 1, 0);
			break;
		case 1: //down (0, -1, 0)
			direction = new Vector3(0, -1, 0);
			break;
		case 2: //left (-1, 0, 0)
			direction = new Vector3(-1, 0, 0);
			break;
		case 3: //right (1, 0, 0)
			direction = new Vector3(1, 0, 0);
			break;
		}
	}

}
