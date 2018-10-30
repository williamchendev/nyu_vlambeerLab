using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationScript : MonoBehaviour {

	//Components
	
	//Settings
	
	//Variables
	private int[,] map_data;
	private int[,] building_map_data;
	private int[,] object_map_data;
	private float[,] object_heatmap_data;
	private Vector2 offset;

	private List<GameObject> debug_objs;
	
	//Init
	void Awake () {
		if (!isActiveAndEnabled)
		{
			return;
		}
		
		//Components
		
		//Settings
		
		//Variables
		map_data = new int[50, 50];
		offset = new Vector2(-map_data.GetLength(0), -map_data.GetLength(1));

		debugFunction();
	}
	
	//Generate Map Functions
	private void generateMapData ()
	{
		//Create an Empty Map
		map_data = new int[map_data.GetLength(0), map_data.GetLength(1)];
		
		//Generate Spaces
		generateWalker(0, 0, 120, 3);
		
		//Cellular Generation
		mapFilter();
	}
	
	private void generateWalker (int x, int y, int life_span, int walkers)
	{
		//Generate Walker Variables
		x += map_data.GetLength(0) / 2; //X position
		y += map_data.GetLength(1) / 2; //Y position
		int dir = 0; //Direction
		int l = 0; //Length to travel
		int walk_prob = 75; //Probability to generate a new walker

		//Iterate throughout lifecycle
		for (int i = 0; i < life_span; i++)
		{
			//Draw on position
			x = Mathf.Clamp(x, 0, map_data.GetLength(0) - 1);
			y = Mathf.Clamp(y, 0, map_data.GetLength(1) - 1);
			drawMapSize(x, y, 1, Random.Range(1, 51));
			
			//Check if on border
			if (x < 10)
			{
				while (dir == 1)
				{
					dir = Random.Range(0, 4);
				}
			}
			else if (x > map_data.GetLength(0) - 11)
			{
				while (dir == 0)
				{
					dir = Random.Range(0, 4);
				}
			}
			else if (y < 10)
			{
				while (dir == 3)
				{
					dir = Random.Range(0, 4);
				}
			}
			else if (y > map_data.GetLength(1) - 11)
			{
				while (dir == 2)
				{
					dir = Random.Range(0, 4);
				}
			}

			//Length of the direction the walker will travel
			l--;
			if (l <= 0)
			{
				l = Random.Range(2, 8);
				int temp_dir = dir;
				while (dir == temp_dir)
				{
					dir = Random.Range(0, 4);
				}
			}
			
			//Move in direction
			if (dir == 0)
			{
				x++;
			}
			else if (dir == 1)
			{
				x--;
			}
			else if (dir == 2)
			{
				y++;
			}
			else
			{
				y--;
			}

			//Generate new Walkers
			if (walkers > 0)
			{
				walk_prob += 5;
				if (Random.Range(0, 100) < walk_prob)
				{
					generateWalker(x - (map_data.GetLength(0) / 2), y - (map_data.GetLength(1) / 2), life_span / 2, walkers / 2);
					walkers--;
				}
			}
		}
	}
	
	//Generate Building Map Functions
	private void generateBuildingData ()
	{
		//Create an Empty Map
		building_map_data = new int[map_data.GetLength(0), map_data.GetLength(1)];
		
		//Generate Buildings
		for (int w = 0; w < building_map_data.GetLength(0); w++)
		{
			for (int h = 0; h < building_map_data.GetLength(1); h++)
			{
				int[] buildingtypes = shuffledArray();
				if (!placeBuilding(w, h, Random.Range(0, buildingtypes.Length)))
				{
					for (int q = 0; q < buildingtypes.Length; q++)
					{
						if (placeBuilding(w, h, buildingtypes[q]))
						{
							break;
						}
					}
				}
			}
		}
	}

	private void crossGen()
	{
		
		for (int w = 0; w < map_data.GetLength(0); w++)
		{
			for (int h = 0; h < map_data.GetLength(1); h++)
			{
				if (map_data[w, h] == 1)
				{
					if (building_map_data[w, h] == 1)
					{
						deleteBuilding(w, h);
					}
				}
			}
		}
		
		for (int w = 0; w < map_data.GetLength(0); w++)
		{
			for (int h = 0; h < map_data.GetLength(1); h++)
			{
				if (map_data[w, h] == 2)
				{
					for (int w2 = -6; w2 <= 6; w2++)
					{
						for (int h2 = -6; h2 <= 6; h2++)
						{
							int w_x = w + w2;
							int h_y = h + h2;
							
							int x_e = Mathf.Clamp(w_x, 0, building_map_data.GetLength(0) - 1);
							int y_e = Mathf.Clamp(h_y, 0, building_map_data.GetLength(1) - 1);
							
							if (building_map_data[x_e, y_e] == 0)
							{
								int[] directions = { getBuildingDataClamp(w_x + 1, h_y), getBuildingDataClamp(w_x - 1, h_y), getBuildingDataClamp(w_x, h_y + 1), getBuildingDataClamp(w_x, h_y - 1) };
							
								int full = 0;
								for (int i = 0; i < directions.Length; i++)
								{
									if (directions[i] == 1)
									{
										full++;
									}
								}

								if (full >= 2)
								{
									building_map_data[x_e, y_e] = 2;
								}
							}
						}
					}
				}
			}
		}
	}

	private int[] shuffledArray()
	{
		int[] building_types = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
		for (int i = 0; i < building_types.Length; i++)
		{
			int random_place = Random.Range(0, building_types.Length);
			int temp = building_types[random_place];
			building_types[random_place] = building_types[i];
			building_types[i] = temp;
		}
		return building_types;
	}

	private void deleteBuilding(int x, int y)
	{
		building_map_data[x, y] = 0;
		if (getBuildingDataClamp(x + 1, y) == 1)
		{
			deleteBuilding(x + 1, y);
		}
		if (getBuildingDataClamp(x - 1, y) == 1)
		{
			deleteBuilding(x - 1, y);
		}
		if (getBuildingDataClamp(x, y + 1) == 1)
		{
			deleteBuilding(x, y + 1);
		}
		if (getBuildingDataClamp(x, y - 1) == 1)
		{
			deleteBuilding(x, y - 1);
		}
	}

	private bool placeBuilding(int x, int y, int building_type)
	{
		int[,] building_type_data = buildingArrays(building_type);

		bool can_place = true;
		for (int w = 0; w < building_type_data.GetLength(0); w++)
		{
			for (int h = 0; h < building_type_data.GetLength(1); h++)
			{
				if (building_type_data[w, h] == 1)
				{
					int w_x = w + x;
					int h_y = h + y;
					int[] directions = { getBuildingDataClamp(w_x + 1, h_y + 1), getBuildingDataClamp(w_x + 1, h_y - 1), getBuildingDataClamp(w_x - 1, h_y - 1), getBuildingDataClamp(w_x - 1, h_y + 1), getBuildingDataClamp(w_x + 1, h_y), getBuildingDataClamp(w_x - 1, h_y), getBuildingDataClamp(w_x, h_y + 1), getBuildingDataClamp(w_x, h_y - 1) };
					for (int i = 0; i < directions.Length; i++)
					{
						if (directions[i] != 0)
						{
							can_place = false;
							break;
						}
					}
				}
			}
		}

		if (!can_place)
		{
			return false;
		}
		
		for (int w = 0; w < building_type_data.GetLength(0); w++)
		{
			for (int h = 0; h < building_type_data.GetLength(1); h++)
			{
				if (building_type_data[w, h] != 0)
				{
					int x_e = Mathf.Clamp(x + w, 0, building_map_data.GetLength(0) - 1);
					int y_e = Mathf.Clamp(y + h, 0, building_map_data.GetLength(1) - 1);

					building_map_data[x_e, y_e] = building_type_data[w, h];
				}
			}
		}
		
		return true;
	}

	private int[,] buildingArrays(int building_type)
	{
		int[,] building_array_data = new int[6,6];
		if (building_type == 0)
		{
			for (int w = 0; w < 2; w++)
			{
				for (int h = 0; h < 2; h++)
				{
					building_array_data[w, h] = 1;
				}
			}
		}
		else if (building_type == 1)
		{
			for (int w = 0; w < 4; w++)
			{
				for (int h = 0; h < 4; h++)
				{
					building_array_data[w, h] = 1;
				}
			}
		}
		else if (building_type == 2)
		{
			for (int w = 0; w < 2; w++)
			{
				for (int h = 0; h < 4; h++)
				{
					building_array_data[w, h] = 1;
				}
			}
		}
		else if (building_type == 3)
		{
			for (int w = 0; w < 3; w++)
			{
				for (int h = 0; h < 3; h++)
				{
					building_array_data[w, h] = 1;
				}
			}
		}
		else if (building_type == 4)
		{
			for (int w = 0; w < 4; w++)
			{
				for (int h = 0; h < 2; h++)
				{
					building_array_data[w, h] = 1;
				}
			}
		}
		else if (building_type == 5)
		{
			for (int w = 0; w < 6; w++)
			{
				for (int h = 0; h < 2; h++)
				{
					building_array_data[w, h] = 1;
				}
			}
		}
		else if (building_type == 6)
		{
			for (int w = 0; w < 4; w++)
			{
				for (int h = 0; h < 6; h++)
				{
					building_array_data[w, h] = 1;
				}
			}
		}
		else if (building_type == 7)
		{
			for (int w = 0; w < 3; w++)
			{
				for (int h = 0; h < 5; h++)
				{
					building_array_data[w, h] = 1;
				}
			}
		}
		else if (building_type == 8)
		{
			for (int w = 0; w < 6; w++)
			{
				for (int h = 0; h < 4; h++)
				{
					building_array_data[w, h] = 1;
				}
			}
		}
		else
		{
			for (int w = 0; w < 1; w++)
			{
				for (int h = 0; h < 3; h++)
				{
					building_array_data[w, h] = 1;
				}
			}
		}
		return building_array_data;
	}
	
	private int getBuildingDataClamp(int x, int y)
	{
		//Return data at clamped position
		x = Mathf.Clamp(x, 0, building_map_data.GetLength(0) - 1);
		y = Mathf.Clamp(y, 0, building_map_data.GetLength(1) - 1);
		if ((x < 0 || x >= building_map_data.GetLength(0)) || (y < 0 || y >= building_map_data.GetLength(1)))
		{
			return 0;
		}
		return building_map_data[x, y];
	}

	//Generate Object Map Functions
	private void generateObjectMapData ()
	{
		//Create an Empty Map
		object_map_data = new int[map_data.GetLength(0), map_data.GetLength(1)];
		object_heatmap_data = new float[map_data.GetLength(0), map_data.GetLength(1)];

		//Find all optimal Object Placement positions
		for (int w = 0; w < map_data.GetLength(0); w++)
		{
			for (int h = 0; h < map_data.GetLength(1); h++)
			{
				if (map_data[w, h] == 1)
				{
					int[] directions_map = { getDataClamp(w, h - 1), getDataClamp(w, h + 1), getDataClamp(w - 1, h), getDataClamp(w + 1, h) };
					int[] directions_buildings =  { getBuildingDataClamp(w + 1, h + 1), getBuildingDataClamp(w + 1, h - 1), getBuildingDataClamp(w - 1, h - 1), getBuildingDataClamp(w - 1, h + 1), getBuildingDataClamp(w + 1, h), getBuildingDataClamp(w - 1, h), getBuildingDataClamp(w, h + 1), getBuildingDataClamp(w, h - 1) };

					int full = 0;
					for (int i = 0; i < directions_map.Length; i++)
					{
						if (directions_map[i] == 1)
						{
							full++;
						}
					}

					if (full >= 3)
					{
						bool can_place = true;
						for (int q = 0; q < directions_buildings.Length; q++)
						{
							if (directions_buildings[q] == 1)
							{
								can_place = false;
								break;
							}
						}

						if (can_place)
						{
							object_map_data[w, h] = 1;
						}
					}
				}
			}
		}
		
		//Find Object Heatmap bounds
		Vector2[] cross_points = { new Vector2(object_map_data.GetLength(0) / 2f, object_map_data.GetLength(1) / 2f), new Vector2(object_map_data.GetLength(0) / 2f, object_map_data.GetLength(1) / 2f), new Vector2(object_map_data.GetLength(0) / 2f, object_map_data.GetLength(1) / 2f), new Vector2(object_map_data.GetLength(0) / 2f, object_map_data.GetLength(1) / 2f)};
		for (int w = 0; w < object_map_data.GetLength(0); w++)
		{
			for (int h = 0; h < object_map_data.GetLength(1); h++)
			{
				if (object_map_data[w, h] == 1)
				{
					if (Vector2.Distance(new Vector2(0, 0), cross_points[0]) < Vector2.Distance(new Vector2(0, 0), new Vector2(w, h)))
					{
						cross_points[0] = new Vector2(w, h);
					}
					if (Vector2.Distance(new Vector2(object_map_data.GetLength(0), 0), cross_points[1]) < Vector2.Distance(new Vector2(object_map_data.GetLength(0), 0), new Vector2(w, h)))
					{
						cross_points[1] = new Vector2(w, h);
					}
					if (Vector2.Distance(new Vector2(object_map_data.GetLength(0), object_map_data.GetLength(1)), cross_points[2]) < Vector2.Distance(new Vector2(object_map_data.GetLength(0), object_map_data.GetLength(1)), new Vector2(w, h)))
					{
						cross_points[2] = new Vector2(w, h);
					}
					if (Vector2.Distance(new Vector2(0, object_map_data.GetLength(1)), cross_points[3]) < Vector2.Distance(new Vector2(0, object_map_data.GetLength(1)), new Vector2(w, h)))
					{
						cross_points[3] = new Vector2(w, h);
					}
				}
			}
		}

		Bounds heatmap_bounds = new Bounds(new Vector3(object_map_data.GetLength(0) / 2f, 0, object_map_data.GetLength(1) / 2f), Vector3.zero);
		for (int i = 0; i < cross_points.Length; i++)
		{
			heatmap_bounds.Encapsulate(new Vector3(cross_points[i].x, 0, cross_points[i].y));
		}
		Vector2 heatmap_center = new Vector2(heatmap_bounds.center.x, heatmap_bounds.center.z);

		//Create Heatmap
		for (int w = 0; w < object_map_data.GetLength(0); w++)
		{
			for (int h = 0; h < object_map_data.GetLength(1); h++)
			{
				float center_distance = Vector2.Distance(new Vector2(w, h), heatmap_center) / ((object_map_data.GetLength(0) + object_map_data.GetLength(1) / 4f));
				object_heatmap_data[w, h] = Mathf.Pow(1 - Mathf.Clamp(center_distance * 2, 0, 1), 2.5f);
			}
		}
	}
	
	private int getObjectDataClamp(int x, int y)
	{
		//Return data at clamped position
		x = Mathf.Clamp(x, 0, object_map_data.GetLength(0) - 1);
		y = Mathf.Clamp(y, 0, object_map_data.GetLength(1) - 1);
		if ((x < 0 || x >= object_map_data.GetLength(0)) || (y < 0 || y >= object_map_data.GetLength(1)))
		{
			return 0;
		}
		return object_map_data[x, y];
	}

	//Draw Functions
	private void drawMapSize(int x, int y, int num, int size)
	{
		if (size >= 1)
		{
			drawMapSize(x, y, num);
		}
		if (size >= 20)
		{
			drawMapSize(x - 1, y, num);
			drawMapSize(x, y - 1, num);
			drawMapSize(x + 1, y, num);
			drawMapSize(x, y + 1, num);
		}
		if (size >= 50)
		{
			drawMapSize(x - 1, y - 1, num);
			drawMapSize(x + 1, y - 1, num);
			drawMapSize(x + 1, y + 1, num);
			drawMapSize(x - 1, y + 1, num);
			
			drawMapSize(x - 2, y, num);
			drawMapSize(x, y - 2, num);
			drawMapSize(x + 2, y, num);
			drawMapSize(x, y + 2, num);
		}
	}
	
	private void drawMapSize(int x, int y, int num)
	{
		x = Mathf.Clamp(x, 0, map_data.GetLength(0) - 1);
		y = Mathf.Clamp(y, 0, map_data.GetLength(1) - 1);
		map_data[x, y] = num;
	}
	
	//Cellular Automata Functions
	private int getDataClamp(int x, int y)
	{
		//Return data at clamped position
		x = Mathf.Clamp(x, 0, map_data.GetLength(0) - 1);
		y = Mathf.Clamp(y, 0, map_data.GetLength(1) - 1);
		if ((x < 0 || x >= map_data.GetLength(0)) || (y < 0 || y >= map_data.GetLength(1)))
		{
			return -1;
		}
		return map_data[x, y];
	}

	private void mapFilter()
	{
		//Filter map through cellular functions
		for (int q = 0; q < 5; q++)
		{
			for (int w = 0; w < map_data.GetLength(0); w++)
			{
				for (int h = 0; h < map_data.GetLength(1); h++)
				{
					if (map_data[w, h] != 0)
					{
						int[] directions =
						{
							getDataClamp(w, h - 1), getDataClamp(w, h + 1), getDataClamp(w - 1, h),
							getDataClamp(w + 1, h)
						};
						int empties = 0;
						for (int i = 0; i < directions.Length; i++)
						{
							if (directions[i] == 0)
							{
								empties++;
							}
						}

						if (empties > 2)
						{
							map_data[w, h] = 0;
						}
					}
				}
			}
		}

		for (int w = 0; w < map_data.GetLength(0); w++)
		{
			for (int h = 0; h < map_data.GetLength(1); h++)
			{
				if (map_data[w, h] == 0)
				{
					int[] directions =
					{
						getDataClamp(w, h - 1), getDataClamp(w, h + 1), getDataClamp(w - 1, h), getDataClamp(w + 1, h)
					};
					int empties = 0;
					for (int i = 0; i < directions.Length; i++)
					{
						if (directions[i] != 0)
						{
							empties++;
						}
					}

					if (empties == 2)
					{
						map_data[w, h] = 1;
					}
				}
			}
		}

		//Generate Edges
		for (int w = 0; w < map_data.GetLength(0); w++)
		{
			for (int h = 0; h < map_data.GetLength(1); h++)
			{
				if (map_data[w, h] == 0)
				{
					int[] directions = { getDataClamp(w, h - 1), getDataClamp(w, h + 1), getDataClamp(w - 1, h), getDataClamp(w + 1, h) };
					bool full = false;
					
					for (int i = 0; i < directions.Length; i++)
					{
						if (directions[i] == 1)
						{
							full = true;
							break;
						}
					}

					if (full)
					{
						map_data[w, h] = 2;
					}
				}
			}
		}
	}

	//Debug
	private void generateDebug()
	{
		if (debug_objs != null)
		{
			foreach (GameObject obj in debug_objs)
			{
				Destroy(obj.gameObject);
			}
		}
		debug_objs = new List<GameObject>();
		for (int w = 0; w < map_data.GetLength(0); w++)
		{
			for (int h = 0; h < map_data.GetLength(1); h++)
			{
				Color color = Color.white;

				if (map_data[w, h] == 1)
				{
					color = Color.red;
				}
				else if (map_data[w, h] == 2)
				{
					color = Color.magenta;
				}
				
				GameObject temp = createDebugBlock(color);
				temp.transform.position = new Vector3(w * 2, 0, h * 2) + new Vector3(offset.x, 0, offset.y);
				debug_objs.Add(temp);
			}
		}
	}
	
	private void generateDebug2()
	{
		LayerMask solids_layer = GameObject.FindWithTag("Grid").gameObject.GetComponent<GridBehaviour>().solidLayer;
		if (debug_objs != null)
		{
			foreach (GameObject obj in debug_objs)
			{
				Destroy(obj.gameObject);
			}
		}
		debug_objs = new List<GameObject>();
		for (int w = 0; w < building_map_data.GetLength(0); w++)
		{
			for (int h = 0; h < building_map_data.GetLength(1); h++)
			{
				if (object_map_data[w, h] == 1)
				{
					Color temp_color_pow = Color.Lerp(Color.blue, Color.cyan, object_heatmap_data[w, h]);
					GameObject temp2 = createDebugBlock(temp_color_pow);
					temp2.transform.position = new Vector3(w * 2, 0, h * 2) + new Vector3(offset.x, 0, offset.y);
					debug_objs.Add(temp2);
				}
				
				Color color = Color.white;

				if (building_map_data[w, h] == 1)
				{
					color = Color.red;
				}
				else if (building_map_data[w, h] == 2)
				{
					color = Color.magenta;
				}
				else
				{
					continue;
				}
				
				GameObject temp = createDebugBlock(color);
				temp.layer = 8;
				temp.transform.position = new Vector3(w * 2, 0, h * 2) + new Vector3(offset.x, 0, offset.y);
				debug_objs.Add(temp);
			}
		}
	}
	
	private void generateDebug3()
	{
		if (debug_objs != null)
		{
			foreach (GameObject obj in debug_objs)
			{
				Destroy(obj.gameObject);
			}
		}
		debug_objs = new List<GameObject>();
		
		List<GameObject> building_objs = new List<GameObject>();
		List<GameObject> walk_objs = new List<GameObject>();
		List<GameObject> park_objs = new List<GameObject>();
		GenerationShowScript show_gen = Instantiate(Resources.Load<GameObject>("generator")).GetComponent<GenerationShowScript>();
		debug_objs.Add(show_gen.gameObject);
		
		for (int w = 0; w < building_map_data.GetLength(0); w++)
		{
			for (int h = 0; h < building_map_data.GetLength(1); h++)
			{
				Vector3 position = new Vector3(w * 2, 0, h * 2) + new Vector3(offset.x, 0, offset.y);
				if (object_map_data[w, h] == 1)
				{
					//Create Park
					if (Random.Range(0f, 3f) < object_heatmap_data[w, h] * 0.8f)
					{
						GameObject temp = Instantiate(Resources.Load<GameObject>("park_" + Random.Range(1, 4)));
						temp.SetActive(false);
						temp.transform.position = position;
						temp.transform.eulerAngles = new Vector3(0, 90 * Random.Range(0, 4), 0);
						debug_objs.Add(temp);
						park_objs.Add(temp);
					}
				}

				if (building_map_data[w, h] == 1)
				{
					GameObject temp = Instantiate(Resources.Load<GameObject>("build_" + Random.Range(1, 5)));
					temp.SetActive(false);
					temp.transform.position = position;
					temp.transform.eulerAngles = new Vector3(0, 90 * Random.Range(0, 4), 0);
					GameObject temp2 = Instantiate(Resources.Load<GameObject>("sidewalk"));
					temp2.SetActive(false);
					temp2.transform.position = position;
					debug_objs.Add(temp);
					debug_objs.Add(temp2);
					building_objs.Add(temp);
					walk_objs.Add(temp2);
				}
				else if (building_map_data[w, h] == 2)
				{
					GameObject temp2 = Instantiate(Resources.Load<GameObject>("block"));
					temp2.SetActive(false);
					temp2.transform.position = position;
					debug_objs.Add(temp2);
					walk_objs.Add(temp2);
				}
				else
				{
					continue;
				}
			}
		}

		show_gen.buildings = building_objs;
		show_gen.sidewalks = walk_objs;
		show_gen.parks = park_objs;
	}

	private void debugFunction()
	{
		generateMapData();
		generateBuildingData();
		crossGen();
		generateObjectMapData();
		generateDebug3();
	}

	//Update Event
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			debugFunction();
		}
	}

	//Methods
	public GameObject createDebugBlock(Color color)
	{
		GameObject temp_debug_block = GameObject.CreatePrimitive(PrimitiveType.Cube);;
		temp_debug_block.transform.localScale = new Vector3(2, 2, 2);
		temp_debug_block.GetComponent<MeshRenderer>().material.color = color;
		return temp_debug_block;
	}
}
