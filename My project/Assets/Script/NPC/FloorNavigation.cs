using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FloorNavigation : MonoBehaviour
{
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

    [SerializeField] private LayerMask stairLayer;
    private Dictionary<float, List<StairController>> stairsByFloorLevel = new Dictionary<float, List<StairController>>();

    private void listAllStairs()
    {
        StairController[] allStairs = FindObjectsByType<StairController>(FindObjectsSortMode.None); // Find all the stairs in the level
        
        foreach (StairController stair in allStairs)
        {
            float floorLevel = stair.FloorLevel;//Mathf.RoundToInt(stair.StartPoint.transform.position.y);
            
            if (!stairsByFloorLevel.ContainsKey(floorLevel))
            {
                stairsByFloorLevel[floorLevel] = new List<StairController>();
            }
            
            stairsByFloorLevel[floorLevel].Add(stair);
            
        }
    }

    public StairController FindNearestStairToFloor(HumanNPCBehaviour npc, float targetFloor,StairDirection neededDirection)
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

        // Determine if we need to go up or down
        //StairDirection neededDirection = (targetFloor > npcFloor) ? StairDirection.Upward : StairDirection.Downward;

        StairController closestStair = null;
        float closestDistance = float.MaxValue;

        

        foreach (StairController stair in stairsByFloorLevel[npcFloor])
        {
            
            // Check if this stair leads to the right direction
            bool canUseStair = false;

            if(neededDirection == StairDirection.Upward && stair.UpperFloor != null)
            {
                canUseStair = true;
            }
            else if (neededDirection == StairDirection.Downward && stair.BottomFloor != null)
            {
                canUseStair = true;
            }
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

    public List<StairController> FindPathToFloor(HumanNPCBehaviour npc, float targetFloor)
    {
        List<StairController> path = new List<StairController>();
        float npcFloor = npc.FloorLevel;
        

        while(npcFloor != targetFloor)
        {
            // Determine if we need to go up or down
            StairDirection direction = (targetFloor > npcFloor) ? StairDirection.Upward : StairDirection.Downward;
            
            StairController nextStair = FindNearestStairToFloor(npc,targetFloor, direction);

            if(nextStair == null)
            {
                print("cant find path");
                // Can't find a path
                break;
            }

            path.Add(nextStair);

            

            if(direction == StairDirection.Upward && nextStair.UpperFloor != null)
            {
                npcFloor = nextStair.UpperFloor.FloorLevel;
                //npc.UpdateFloorLevel(nextStair.FloorLevel);
            }
            else if(direction == StairDirection.Downward && nextStair.BottomFloor != null)
            {
                npcFloor = nextStair.BottomFloor.FloorLevel;
                //npc.UpdateFloorLevel(nextStair.FloorLevel);
            }
        }
        return path;
    }

}
