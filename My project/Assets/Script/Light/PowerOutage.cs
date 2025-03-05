using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PowerOutage : MonoBehaviour, IPossessable
{
    [Header("Lights variables")]
    [SerializeField] private bool closeAllLightsInBuilding;  // Do we close all the lights inside the house
    [SerializeField] private GameObject[] closedLights; // Specific lights that should be close
    [SerializeField] private LayerMask wallFloorLayer;
    private GameObject[] allBuildingLights;   // Every lights inside the building
    private GameObject[] lightsToClose;

    private List<HumanNPCBehaviour> affectedNPCs = new List<HumanNPCBehaviour>();
    private bool isRepairing = false;
    
    void Start()
    {
        allBuildingLights = GameObject.FindGameObjectsWithTag("BuildingLight");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDepossessed()
    {
        // Nothing to implement when player depossessed the object
    }

    public void OnPossessed()
    {
        lightsToClose = closeAllLightsInBuilding ? closedLights : allBuildingLights;
        CloseOpenLights(lightsToClose, false );
    }

    // Close or open all lights in the array
    public void CloseOpenLights(GameObject[] lights, bool open)
    {
        foreach (GameObject light in lights)
        {
            light.SetActive(open);
        }
        HandleNPCs(lights);
    }

    private void HandleNPCs(GameObject[] lightsClosed)
    {
        // Find all NPC in the scene
        HumanNPCBehaviour[] allNPCs = FindObjectsByType<HumanNPCBehaviour>(FindObjectsSortMode.None);

        foreach (HumanNPCBehaviour npc in allNPCs)
        {
            if (IsNPCAffected(npc, lightsClosed))
            {
                if (!isRepairing)
                {
                    isRepairing = true;
                    RepairLights();
                }
                affectedNPCs.Add(npc);

                PatrollingNPCBehaviour patrollingNPC = npc.gameObject.GetComponent<PatrollingNPCBehaviour>();
                if (patrollingNPC != null)
                {
                    ///canMove = false;
                    /////TODO
                }
            }
        }
    }

    // Check if the NPC is currently in an area affected by the closed lights
    private bool IsNPCAffected(HumanNPCBehaviour npc, GameObject[] lights)
    {
        Collider2D npcCollider = npc.GetComponent<Collider2D>();
        foreach(GameObject gameObjectLight in lights)
        {
            Collider2D lightCollider = gameObjectLight.GetComponent<Collider2D>();
            if (LightUtility.IsPointHitByLight(lightCollider, npcCollider, wallFloorLayer))
            {
                return true;
            }
        }
        return false;
    }

    private void RepairLights()
    {
        // TODO
    }

    private void RestoreLights()
    {
        CloseOpenLights(lightsToClose, true);
        lightsToClose = null;
        foreach (HumanNPCBehaviour npc in affectedNPCs)
        {
            PatrollingNPCBehaviour patrollingNPC = npc.gameObject.GetComponent<PatrollingNPCBehaviour>();
            if (patrollingNPC != null)
            {
                ///canMove = true;
                /////TODO
            }
        }
        affectedNPCs.Clear();
    }
}
