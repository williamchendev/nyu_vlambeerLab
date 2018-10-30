using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationShowScript : MonoBehaviour
{

	public List<GameObject> buildings;
	public List<GameObject> sidewalks;
	public List<GameObject> parks;

	private int timer;
	
	// Use this for initialization
	void Start ()
	{
		timer = 0;
	}
	
	// Update is called once per frame
	void Update ()
	{
		timer++;
		
		if (buildings != null)
		{
			if (buildings.Count > 0)
			{
				//int index = Random.Range(0, buildings.Count);
				int index = 0;
				buildings[index].SetActive(true);
				buildings.RemoveAt(index);
			}
			else
			{
				buildings = null;
			}
		}

		if (timer > 240)
		{
			if (sidewalks != null)
			{
				if (sidewalks.Count > 0)
				{
					//int index = Random.Range(0, buildings.Count);
					int index = 0;
					sidewalks[index].SetActive(true);
					sidewalks.RemoveAt(index);
				}
				else
				{
					sidewalks = null;
				}
			}
		}

		if (timer > 480)
		{
			if (parks != null)
			{
				if (parks.Count > 0)
				{
					//int index = Random.Range(0, buildings.Count);
					int index = 0;
					parks[index].SetActive(true);
					parks.RemoveAt(index);
				}
				else
				{
					parks = null;
				}
			}
		}
	}
}
