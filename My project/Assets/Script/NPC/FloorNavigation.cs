using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FloorNavigation : MonoBehaviour
{
    // Pathfinding for our Human NPC

    [SerializeField] private LayerMask stairLayer;
    // Dictionaries of all staircases ordered by floor
    private Dictionary<float, List<StairController>> stairsByFloorLevel = new Dictionary<float, List<StairController>>();

    // Singleton pattern
    private static FloorNavigation instance;
    public static FloorNavigation Instance { get { return instance; } }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    private void Start()
    {
        listAllStairs();
    }

    
    // List all stairs in the scene and ordered them by floor level
    private void listAllStairs()
    {
        StairController[] allStairs = FindObjectsByType<StairController>(FindObjectsSortMode.None); // Find all the stairs in the level
        
        foreach (StairController stair in allStairs)
        {
            float floorLevel = stair.FloorLevel;
            
            if (!stairsByFloorLevel.ContainsKey(floorLevel))
            {
                stairsByFloorLevel[floorLevel] = new List<StairController>();
            }
            
            stairsByFloorLevel[floorLevel].Add(stair);
            
        }
    }

    // Find the closest staircase and on the same floor as the npc.
    private StairController FindNearestStairToFloor(HumanNPCBehaviour npc, float targetFloor,StairDirection neededDirection)
    {
        float npcFloor = npc.FloorLevel;

        // If we're already on the right floor, no stairs needed
        if (npcFloor == targetFloor)
        {
            return null;
        }

        if (!stairsByFloorLevel.ContainsKey(npcFloor))
        {
            
            // No stairs found on current floor level
            return null;
        }

        StairController closestStair = null;
        float closestDistance = float.MaxValue;        

        // Find a stair that leads to the targeted floor level
        foreach (StairController stair in stairsByFloorLevel[npcFloor])
        {
            
            // Check if this stair leads to the right direction
            bool canUseStair = false;

            // If the NPC needs to go up and the stair leads upward
            if(neededDirection == StairDirection.Upward && stair.UpperFloor != null)
            {
                canUseStair = true;
            }
            // If the NPC nees to go down and the stair leads downstair
            else if (neededDirection == StairDirection.Downward && stair.BottomFloor != null)
            {
                canUseStair = true;
            }
            // If a stair was found we checked if it's the closest one
            if (canUseStair && CanNPCUseStair(stair, npc))
            {
                float distance = Vector2.Distance(npc.transform.position, stair.StartPoint.transform.position);
                if(distance < closestDistance)
                {
                    closestDistance = distance;
                    closestStair = stair;
                }
            }
        }
        return closestStair;
    }

    // Check if the NPC can use this stair
    private bool CanNPCUseStair(StairController stair, HumanNPCBehaviour npc)
    {
        Renderer npcRenderer = npc.GetComponent<Renderer>();

        return npcRenderer.bounds.size.x <= stair.MaximumWidth && npcRenderer.bounds.size.y <= stair.MaximumHeight;
    }

    // Find all the stairs the NPC must used to go to the targeted floor
    public List<StairController> FindPathToFloor(HumanNPCBehaviour npc, float targetFloor)
    {
        List<StairController> path = new List<StairController>();   // List of all the stairs the NPC must used
        float npcFloor = npc.FloorLevel;    // Floor level where the NPC is
        
        // As long as the NPC is not on the desired floor
        while(npcFloor != targetFloor)
        {
            // Determine if we need to go up or down
            StairDirection direction = (targetFloor > npcFloor) ? StairDirection.Upward : StairDirection.Downward;
            // Find the nearest stair to used to go to the targeted floor
            StairController nextStair = FindNearestStairToFloor(npc,targetFloor, direction);

            // If there is no path for the NPC
            if(nextStair == null)
            {
                print("cant find path");
                return null;
                // Can't find a path
                //break;
            }
            // Add the found staircase to the list the NPC must used
            path.Add(nextStair);

            
            // Update floor level of the NPC
            if(direction == StairDirection.Upward && nextStair.UpperFloor != null)
            {
                npcFloor = nextStair.UpperFloor.FloorLevel;                
            }
            else if(direction == StairDirection.Downward && nextStair.BottomFloor != null)
            {
                npcFloor = nextStair.BottomFloor.FloorLevel;                
            }
        }
        return path;
    }

}
