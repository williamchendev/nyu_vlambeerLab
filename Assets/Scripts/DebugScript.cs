using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugScript : MonoBehaviour
{

	public int size = 50;
	public float node_size = 0.5f;
	public LayerMask solids_layer;

	private int array_size;
	private bool[,] on_off;
	private bool run_in_game;

	// Use this for initialization
	void Start()
	{
		array_size = (int) (size / node_size);
		on_off = new bool[array_size, array_size];
		run_in_game = true;
	}

	// Update is called once per frame
	void Update()
	{
		float x_pos = transform.position.x - (size / 2);
		float y_pos = transform.position.z - (size / 2);
		
		for (int w = 0; w < on_off.GetLength(0); w++)
		{
			for (int h = 0; h < on_off.GetLength(1); h++)
			{
				Vector3 temp_pos = new Vector3(x_pos + (node_size * w), 0, y_pos + (node_size * h));
				Collider[] hits = Physics.OverlapSphere(temp_pos, 0.25f, solids_layer);
				on_off[w, h] = false;
				if (hits.Length > 0)
				{
					on_off[w, h] = true;
				}
			}
		}
	}

	void OnDrawGizmos()
	{
		if (run_in_game)
		{
			float x_pos = transform.position.x - (size / 2);
			float y_pos = transform.position.z - (size / 2);
			
			for (int w = 0; w < on_off.GetLength(0); w++)
			{
				for (int h = 0; h < on_off.GetLength(1); h++)
				{
					Gizmos.color = Color.blue;
					if (on_off[w, h] == true)
					{
						Gizmos.color = Color.red;
					}

					Vector3 temp_pos = new Vector3(x_pos + (node_size * w), 0, y_pos + (node_size * h));
					Gizmos.DrawWireSphere(temp_pos, 0.25f);
				}
			}
		}
	}
}