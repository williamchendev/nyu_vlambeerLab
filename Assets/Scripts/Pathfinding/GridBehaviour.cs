using System;
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
                    `/+/o+::+++++oo:-----//++oo/                               aka William Spelman bc that's my Twitter handle                         
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
    
                  ~ This Utility is provided to you by you local Socialist Code Witches! ~
*/

public class GridBehaviour : MonoBehaviour {

    //Settings
    [SerializeField] private int size;
    [SerializeField] private float node_size;
    [SerializeField] private LayerMask solids_layer;
    [SerializeField] private bool show_debug;

    private bool start_debug;
    private Node[,] grid;

    //Init Grid
    void Start() {
        //Create new BoxCollider
        createGrid();
        start_debug = true;
    }

    private void createGrid () {
        //Create Grid
        int array_width = (int) (size / node_size);
        float start_x = transform.position.x - (size / 2f);
        float start_y = transform.position.z - (size / 2f);
        
        grid = new Node[array_width, array_width];

        //Create Grid Check
        int i = 0;
        for (int w = 0; w < grid.GetLength(0); w++)
        {
            for (int h = 0; h < grid.GetLength(1); h++)
            {
                Vector3 node_position = new Vector3(start_x + (w * node_size), transform.position.y, start_y + (h * node_size));
                
                bool is_empty = true;
                Collider[] box_hits = Physics.OverlapBox(node_position, new Vector3(node_size / 2, node_size / 2, node_size / 2), new Quaternion(0, 0, 0, 0), solids_layer);
                //Collider[] hits = Physics.OverlapSphere(node_position, node_size / 2f, solids_layer);
                if (box_hits.Length > 0)
                {
                    is_empty = false;
                }
                
                grid[w, h] = new Node(is_empty, node_position.x, node_position.z, i);
                i++;
            }
        }
    }

    public List<Node> getNeighbors(Node node){
        List<Node> neighbors = new List<Node>();

        int grid_width = grid.GetLength(0);
        int grid_height = grid.GetLength(1);
        int node_check_x = node.getGridNum() % grid_width;
        int node_check_y = node.getGridNum() / grid_width;

        for (int w = -1; w <= 1; w++){
            for (int h = -1; h <= 1; h++){
                if (w == 0 && h == 0){
                    continue;
                }

                if (node_check_x + w >= 0 && node_check_x + w < grid_width){
                    if (node_check_y + h >= 0 && node_check_y + h < grid_height){
                        neighbors.Add(grid[node_check_x + w, node_check_y + h]);
                    }
                }
            }
        }

        return neighbors;
    }

    public int getDistance(Node nodeA, Node nodeB){
        int temp_distance;

        int grid_width = grid.GetLength(0);
        int w = Mathf.Abs((nodeA.getGridNum() % grid_width) - (nodeB.getGridNum() % grid_width));
        int h = Mathf.Abs((nodeA.getGridNum() / grid_width) - (nodeB.getGridNum() / grid_width));

        if (w > h){
            temp_distance = (h * 14) + ((w - h) * 10);
        }
        else {
            temp_distance = (w * 14) + ((h - w) * 10);
        }
        return temp_distance;
    }

    public Node getNodeFromWorld(float x, float y){
        Node start_node = grid[0, 0];
        Vector2 start_position = start_node.getPosition();

        float world_node_x = (x - start_position.x) / size;
        float world_node_y = (start_position.y - y) / size;

        int node_x = Mathf.RoundToInt(Mathf.Clamp(world_node_x, 0, grid.GetLength(0) - 1));
        int node_y = Mathf.RoundToInt(Mathf.Clamp(world_node_y, 0, grid.GetLength(1) - 1));

        return grid[node_x, node_y]; 
    }

    public Vector2 gridVector(int index){
        index = Mathf.Clamp(index, 0, maxSize());
        int x_val = index % grid.GetLength(0);
        int y_val = index / grid.GetLength(0);
        return grid[x_val, y_val].getPosition();
    }

    public int maxSize(){
        return (grid.GetLength(0) * grid.GetLength(1));
    }

    public LayerMask solidLayer {
        get {
            return solids_layer;
        }
    }

    //Debug
    void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, new Vector3(size, 0.15f, size));

        if (show_debug)
        {
            if (start_debug)
            {
                int array_width = (int) (size / node_size);
                float start_x = (transform.position.x - (size / 2f));
                float start_y = (transform.position.z - (size / 2f));
                for (int w = 0; w < array_width; w++)
                {
                    for (int h = 0; h < array_width; h++)
                    {
                        Gizmos.color = Color.red;
                        if (grid[w, h].isEmpty())
                        {
                            Gizmos.color = Color.blue;
                        }

                        Vector2 draw_position = new Vector2(start_x + (node_size * w), start_y + (node_size * h));
                        Gizmos.DrawWireSphere(new Vector3(draw_position.x, transform.position.y, draw_position.y),
                            node_size / 2f);
                    }
                }
            }
        }
    }

}
