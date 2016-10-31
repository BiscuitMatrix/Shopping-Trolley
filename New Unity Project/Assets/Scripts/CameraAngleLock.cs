using UnityEngine;
using System.Collections;

public class CameraAngleLock : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void LateUpdate()
	{
		transform.rotation = Quaternion.identity;
	}
}
