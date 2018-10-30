using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
                        .-`                                                                            
                        /./                                                                             
                        +.:-                                                                            
                        `:/+/:..--....-..-.`   ``..``                                                   
                            `.-+:.``       `.--`.-.....`                                                  
                            -/`       `    `..//```   `                                                  
                            -+`        ```  `:/-s-.-.                                                     
                        `` `o:.        ..`  -oydso `..                                                    
                        -//-.`       :/:- `.dyshs   `                                                    
                        `+/.`.``  .:osys..`+s:os                                                        
                            .o--.```+dyddo:-.`.:--+/                                                       
                            //--::.`-h:oy- ````...:+                                                       
                            s--//::``-.:/-`.`````.+-                                                       
                    `-..:/+:::++/-.......```.--++                      ~ Written by @mermaid_games ~                                  
                    `/+/o+::+++++oo:-----//++oo/                                                        
                        `-::-/+++/+ss/:-.:oysys++/`                                                       
            `      `-::..-:++++:odhyss/-ysoyyso+-                                                       
            `----.://----:/++/+/:dddyssyhyysyssyy+--                                                     
                `//o++oo/-:+oo:-+:-dhhyhyhdysosyhhhhhy                                                     
                    :o:/+:+/-/+:.ssydddhdhdyydhhhhd`                                                     
                    `+//o`+/-:+o+..yhdhhhmhdyhhhys+d`                                                     
                    .y+s-++:/++oo-`/yysyddhhhhhyso+h`                                                     
                    .y+y+o++oo+shho.:oshmh+oso+osoo-                                                      
                    .y+so++oso+ymmyyo+oymdoo++//o+                                                        
                    :s+++osyssmdmhssshdds///o+od                                                         
                    `+o+oyyyyhmmddssyddy/:/+/shd                                                         
                    `+osyyyhhydmdysshyhs/:::yhhd        ....`                                            
            ..```-:ooyhyyhhhhhddysyyshs/+oohhhs   `./ooo++++o-                                          
                .ooss+/ohhhhysshddhyyysyhy++s:yhd/:o+ssoooo++///s:                                         
                `.. `odhysssyddmhhhyshhsyo:syosysooooooo+oyso+os:                                        
                    :shyssssyyddmhdhsyhhds-:+oys++oooooo+++ooo++os:                                       
                -shhysssssyhddmdhyshhhy/++hhs++ooooooosyy+////+os`                                      
                .yhhhyssssyhhsmdhhsshdds-.odyssooooooosydd+::////oo                                      
                `dhhhysssssyyo+mdhysshdds/:ydhhysooosshd/`so//////+s:                                     
                +dhhyssssss//omhyo++syd+.+dhhhhhyyyhddm  .y+//////+o.                                    
                -o:ooyyss+///odo+---/oyoods+osyyhhddddo   /y+//////+s.                                   
                -+--`-os+///+hs++::::+ohds////++shhddm-   `sso++////oy`                                  
                ./--`  ./+oosdo++/:::++hh/////////+syh:.    /sooo+///oo`                                 
                /o:--.`---.oyo///--:oosd///////o++//+++/.`  .oyso+///o+`                                
                `-+++o//:-.so:-::-..://d+//////++++++ssso+//-.-oyo+/osd:                                
                    `...-::osyo:----/yodhyysssoooossssssoo+++++/+ddddddd:`                              
                            ``s/--.:.-hyyyyyyyyssysso++++///////++oyhhhdddo`                             
                            :s//./o+-o         `-/+so+++////////////+oyhdmo.                            
                            :so+:/-`+:o.            .:+oooo+++//////////+smdho/-.`                       
                            `.::-    ..`                 ..-///+syso+++++yhyyhhhys+/:..`                 
                                                                -dmh/:ooydhhyoosyyyyssssoo:-/oos/`       
                                                                hm+   `+ddh+oo+oyyso++++oyhhyhdm.       
                                                                oy:     ydhyyysoshyysssosyhhhhy+`       
                                                                        `odhhhhhhhhyhhhhhhhhh/`         
                                                                            .ydddddy:` .::::::.            
                                                                            `/ohdddo.                     
                                                                            -:ydh                     
                                                                                ```       
    
                  ~ Written by your local Socialist Code Witch Inno UwU ~
*/

public class InnoWaveBehavior : MonoBehaviour
{

	//Static
	private static int size = 180;
	
	//Settings
	[SerializeField] private float base_height = 0f;
	[Range(1, 3)]
	[SerializeField] private int wave_num = 3;
	[Range(0, 2)]
	[SerializeField] private float wave_spd = 1f;
	public Vector2 position = Vector2.zero;
	
	//Variables
	private Wave[] waves;
	private MeshFilter mesh_filter;

	void Awake()
	{
		mesh_filter = gameObject.AddComponent<MeshFilter>();
		gameObject.AddComponent<MeshRenderer>();
		GetComponent<MeshRenderer>().material = defaultMat();
		MeshData mesh_data = MeshGenerator.generateMesh(new float[size,size]);
		mesh_filter.mesh = mesh_data.createMesh();

		waves = new Wave[3];
		for (int i = 0; i < 3; i++)
		{
			waves[i] = new Wave(i);
		}
	}

	void Update()
	{
		//Create Height Map
		Vector3[,] heightmap = new Vector3[size,size];
		float wave_time = Time.time * wave_spd;
		
		//Set Height Map to Base Height
		for (int h = 0; h < size; h++)
		{
			for (int w = 0; w < size; w++)
			{
				float perlin1 = (Mathf.PerlinNoise((w * 0.8f) + (wave_time * 2), (h * 0.8f) + (wave_time * 2)) * 0.4f);
				float perlin2 = (Mathf.PerlinNoise((w * 0.01f) + (wave_time * 0.8f), (h * 0.01f) + (wave_time * 0.8f)) * 0.6f);
				heightmap[w, h] = new Vector3(w, base_height + perlin1 + perlin2, h);
			}
		}
		
		//Set Waves
		for (int h = 0; h < size; h++)
		{
			for (int w = 0; w < size; w++)
			{
				for (int wav = 0; wav < wave_num; wav++)
				{
					Vector2 temp_pos = position * 100;
					float temp_direction = (Mathf.Cos(waves[wav].direction * Mathf.Deg2Rad) * w) + (h * Mathf.Sin(waves[wav].direction * Mathf.Deg2Rad));
					float k = waves[wav].steepness / ((Mathf.PI * 2) / waves[wav].wavelength);
					float wave_speed = (wave_time / 4) * Mathf.Sqrt(9.8f / k);

					float y_wave_length = (temp_direction + temp_pos.x) * k;
					float y_displace = Mathf.Sin(y_wave_length + wave_speed) * waves[wav].amplitude;

					float x_wave_length = (temp_direction + temp_pos.y) * k;
					float x_displace = Mathf.Cos(x_wave_length + wave_speed);
					
					heightmap[w, h] += new Vector3(x_displace, y_displace, x_displace);
				}
			}
		}
		MeshData mesh_data = MeshGenerator.generateMesh(heightmap);
		
		//Set Mesh Vertices
		mesh_filter.mesh.vertices = mesh_data.vertices;
		mesh_filter.mesh.RecalculateNormals();
		//mesh_filter.mesh = mesh_data.createMesh();
	}

	private Material defaultMat()
	{
		GameObject primitive = GameObject.CreatePrimitive(PrimitiveType.Plane);
		primitive.SetActive(false);
		Material diffuse = primitive.GetComponent<MeshRenderer>().sharedMaterial;
		DestroyImmediate(primitive);
		return diffuse;
	}
	
}

public class Wave
{
	public float amplitude { get; private set; }
	public float wavelength { get; private set; }
	public float steepness { get; private set; }
	public float direction { get; private set; }

	public Wave(int wave_num)
	{
		if (wave_num == 0)
		{
			amplitude = 0.33f;
			wavelength = 0.33f;
			steepness = 1.33f;
			direction = 225f;
		}
		else if (wave_num == 1)
		{
			amplitude = 0.2f;
			wavelength = 0.25f;
			steepness = 5f;
			direction = 135f;
		}
		else if (wave_num == 2)
		{
			amplitude = 0.15f;
			wavelength = 1f;
			steepness = 2f;
			direction = 165f;
		}
		else
		{
			amplitude = Random.Range(0.5f, 3f);
			steepness = Mathf.Pow(2, wave_num);
			wavelength = steepness * 0.1f;
			direction = 45f;
		}
	}

}