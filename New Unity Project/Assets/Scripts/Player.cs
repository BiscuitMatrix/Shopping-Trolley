using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{

	bool thrusting;
	Vector2 thrustDir;
	float thrustForce = 50.0f;
	float kbRotateSp = Mathf.Deg2Rad * 150.0f;
	Rigidbody2D thisRB;
	bool controlEnabled;
	const float fixedBounceAngle = Mathf.Deg2Rad * 30.0f;
	const float turnLimit = Mathf.Deg2Rad * 60.0f;
	float limitVectorX;
	float limitVectorY;
	float crashStunLeft;
	const float crashStunTotal = 0.25f;
	float skidAngle;
	const float maxSkidAngle = Mathf.Deg2Rad * 75.0f;
	float skidTimeLeft;
	const float skidTimeMax = 1.0f;

	public int score;

	// Use this for initialization
	void Start () 
	{
		limitVectorX = Mathf.Sin (turnLimit);
		limitVectorY = Mathf.Cos (turnLimit);
		thrusting = false;
		thrustDir = Vector2.up;
		thisRB = GetComponent<Rigidbody2D> ();
		if (thisRB == null)
		{
			Debug.Log ("No Rigidbody2D on the trolley. Adding ridigbody");
			thisRB = gameObject.AddComponent<Rigidbody2D> ();
			thisRB.gravityScale = 0;
		}
		controlEnabled = true;
		crashStunLeft = 0.0f;
		skidAngle = 0.0f;
		skidTimeLeft = 0.0f;
	}
	
	// Update is called once per frame
	void Update () 
	{

		// Check for input to start.
		if (controlEnabled && !thrusting)
		{
			// Input to start.
			if (Input.GetKeyDown(KeyCode.Space))
			{
				thrusting = true;
			}
		}

		// Keep the cart in a straight line if a crash was recent.
		if (crashStunLeft > 0.0f)
		{
			crashStunLeft -= Time.deltaTime;
			thisRB.angularVelocity = 0;
		} 
		else if (thrusting)
		{
			// Alter the thrust direction based on input.

			float angleInput = 0;
			// Keyboard for now. Need to figure out motion control.
			if (Input.GetKey (KeyCode.LeftArrow) && Input.GetKey (KeyCode.RightArrow))
			{
				// Ignore both keys pressed at once
			} else if (Input.GetKey (KeyCode.LeftArrow))
			{
				angleInput = -kbRotateSp * Time.deltaTime;
			} else if (Input.GetKey (KeyCode.RightArrow))
			{
				angleInput = kbRotateSp * Time.deltaTime;
			}

			// Add any involuntry input.
			angleInput += skidAngle * Time.deltaTime;

			// Append the direction of thrust.
			if (angleInput != 0)
			{
				
				Vector2 newDir;
				newDir.x = Mathf.Cos (angleInput) * thrustDir.x + Mathf.Sin (angleInput) * thrustDir.y;
				newDir.y = -Mathf.Sin (angleInput) * thrustDir.x + Mathf.Cos (angleInput) * thrustDir.y;
				thrustDir = newDir;
			}
		}

		// Decrease anytime on involuntry turning.
		if (skidTimeLeft > 0)
		{
			skidTimeLeft -= Time.deltaTime;
			if (skidTimeLeft < 0)
			{
				StopSkid ();
			}
		}

		// Find the direction of thrust with respect to going up.
		float thrustAngle = Mathf.Atan2 (thrustDir.y, thrustDir.x);


		// Cap motion between +/- the turn limit from north (north is +half-pi).
		if (thrustAngle < (Mathf.PI * 0.5f - turnLimit))
		{
			thrustDir.x = limitVectorX;
			thrustDir.y = limitVectorY;
		} 
		else if (thrustAngle > (Mathf.PI * 0.5f + turnLimit))
		{
			thrustDir.x = -limitVectorX;
			thrustDir.y = limitVectorY;
		}

		if (thrusting)
		{
			// Match the trolley's angle to the final thrust direction and apply the force.
			transform.rotation = Quaternion.LookRotation(Vector3.forward ,new Vector3(thrustDir.x, thrustDir.y));
			thisRB.AddForce (thrustDir * thrustForce, ForceMode2D.Force);
		}
	}

	void OnCollisionEnter2D(Collision2D coll)
	{

		// If the collision occured in the aisle, lock the motion to travel away.
		if (thrusting)
		{
			bool collOnRight = coll.collider.transform.position.x > transform.position.x;
			if ((thrustDir.x > 0) == (collOnRight))
			{
				float xMult;
				if (collOnRight)
				{
					xMult = -1;
				} else
				{
					xMult = 1;
				}
				thrustDir.x = xMult * Mathf.Sin (fixedBounceAngle);
				thrustDir.y = Mathf.Cos (fixedBounceAngle);

				crashStunLeft = crashStunTotal;

				// Set the rotation and stop any spinning.
				transform.rotation = Quaternion.LookRotation (Vector3.forward, new Vector3 (thrustDir.x, thrustDir.y));
				thisRB.angularVelocity = 0;
			}
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		Pickup pick = other.gameObject.GetComponent<Pickup> ();

		if (pick != null)
		{
			// Do stuff
		} 

		if (other.gameObject.layer == (LayerMask.NameToLayer("Spill")))
		{
			StartSkid();
		}
	}

	public void DisableThrust()
	{
		controlEnabled = false;
		thrusting = false;

		// The camera should be the only chidren. Change this if that's not the case anymore.
		transform.DetachChildren ();
	}

	void StopSkid()
	{
		skidAngle = 0;
	}

	void StartSkid()
	{
		int dir = Random.Range (0, 2) * 2 - 1;
		skidAngle = maxSkidAngle * dir;
		skidTimeLeft = skidTimeMax;
	}
}
