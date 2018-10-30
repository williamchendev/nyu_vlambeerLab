using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour {

	//Components
	private PathScript path;
	
	//Settings
	private bool can_move;
	
	//Variables
	private Vector2 current_position;
	private Vector2 move_position;

	private int move_index;
	private Vector2[] move_path;
	
	//Init
	void Start ()
	{
		//Components
		path = gameObject.AddComponent<PathScript>();
		path.grid_behave = GameObject.FindWithTag("Grid").GetComponent<GridBehaviour>();
		
		//Settings
		can_move = true;

		//Variables
		current_position = new Vector2(transform.position.x, transform.position.z);
		move_position = current_position;
	}
	
	//Update
	void Update () {
		if (can_move)
		{
			if (Input.GetMouseButtonDown(0))
			{
				RaycastHit hit;
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(ray, out hit)) 
				{
					if (hit.collider.gameObject.CompareTag("Grid"))
					{
						//hit.collide
						//move_path = path.getPath(transform.position, )
					}
				}
			}
		}
	}
}
