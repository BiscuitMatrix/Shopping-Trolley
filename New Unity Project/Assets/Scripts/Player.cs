using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{

	bool thrusting;
	Vector2 thrustDir;
	float thrustForce = 50.0f;
	float kbRotateSp = 3;
	Rigidbody2D thisRB;
	const float rootHalf = 0.7071067811865475f;
	bool controlEnabled;

	// Use this for initialization
	void Start () 
	{
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
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (controlEnabled && !thrusting)
		{
			// Input to start.
			if (Input.GetKeyDown(KeyCode.Space))
			{
				thrusting = true;
			}
		}

		if (thrusting) 
		{
			// Alter the thrust direction based on input.

			float angleInput = 0;
			// Keyboard for now. Need to figure out motion control.
			if (Input.GetKey (KeyCode.LeftArrow) && Input.GetKey (KeyCode.RightArrow))
			{
				// Ignore both keys pressed at once
			}
			else if (Input.GetKey (KeyCode.LeftArrow))
			{
				angleInput = -kbRotateSp * Time.deltaTime;
			}
			else if (Input.GetKey (KeyCode.RightArrow))
			{
				angleInput = kbRotateSp * Time.deltaTime;
			}

			// Append the direction of thrust.
			if (angleInput != 0)
			{
				
				Vector2 newDir;
				newDir.x = Mathf.Cos (angleInput) * thrustDir.x + Mathf.Sin (angleInput) * thrustDir.y;
				newDir.y = -Mathf.Sin (angleInput) * thrustDir.x + Mathf.Cos (angleInput) * thrustDir.y;
				thrustDir = newDir;
			}

			// Find the direction of thrust with respect to going up.
			float thrustAngle = Mathf.Atan2 (thrustDir.y, thrustDir.x);


			// Cap motion between 45 degrees (NE) and 135 degrees (NW). AINT WORKIN.
/*			if (thrustAngle < -45)
			{
				thrustDir.x = rootHalf;
				thrustDir.y = rootHalf;
			} 
			else if (thrustAngle > 45)
			{
				thrustDir.x = -rootHalf;
				thrustDir.y = rootHalf;
			}
*/
			// Match the trolley's angle to the final thrust direction and apply the force.
			transform.rotation = Quaternion.LookRotation(Vector3.forward ,new Vector3(thrustDir.x, thrustDir.y));
			thisRB.AddForce (thrustDir * thrustForce, ForceMode2D.Force);
		}
	}

	void OnCollisionEnter2D(Collision2D coll)
	{
		bool collOnRight = coll.collider.transform.position.x > transform.position.x;
		if ((thrustDir.x > 0) == (collOnRight))
		{
			thrustDir.x = -thrustDir.x;
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{

	}

	public void DisableThrust()
	{
		controlEnabled = false;
		thrusting = false;

		transform.DetachChildren ();
	}
}
