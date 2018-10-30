using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{

	private int mag;
	
	// Use this for initialization
	void Start ()
	{
		mag = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0))
		{
			mag++;
			if (mag == 7)
			{
				mag = 0;
			}
		}
		else if (Input.GetMouseButtonDown(1))
		{
			mag--;
			if (mag == -1)
			{
				mag = 6;
			}
		}
		
		Vector3 move = Vector3.zero;
		float spd = 20f;

		if (Input.GetKey(KeyCode.W))
		{
			move.z += spd;
		}
		else if (Input.GetKey(KeyCode.S))
		{
			move.z -= spd;
		}
		
		if (Input.GetKey(KeyCode.A))
		{
			move.x -= spd;
		}
		else if (Input.GetKey(KeyCode.D))
		{
			move.x += spd;
		}
		
		transform.position = Vector3.Lerp(transform.position, new Vector3(0, (-mag * 15) + 100, 0) + move, Time.deltaTime * 2.5f);
	}
}
