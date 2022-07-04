using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheceker : MonoBehaviour
{
	PlayerScript playerScript;

	void Awake()
	{
		playerScript = GetComponentInParent<PlayerScript>();
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == playerScript.gameObject)
			return;

		playerScript.SetGrounded(true);
	}

	void OnTriggerExit(Collider other)
	{
		if (other.gameObject == playerScript.gameObject)
			return;

		playerScript.SetGrounded(false);
	}

	void OnTriggerStay(Collider other)
	{
		if (other.gameObject == playerScript.gameObject)
			return;

		playerScript.SetGrounded(true);
	}
}
