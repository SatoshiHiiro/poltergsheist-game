using System.Collections;
using System.Linq;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    // Handles respawning of player and other objects

    public static CheckpointManager Instance { get; private set; }  // Singleton
    private Checkpoint currentCheckpoint;    // Current checkpoint when player dies
    private GameObject player;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void SetCheckPoint(Checkpoint newCheckpoint)
    {
        currentCheckpoint = newCheckpoint;
    }

    public void Respawn()
    {
        if(currentCheckpoint != null)
        {

            StartCoroutine(WaitBeforeReset());
        }
    }

    private void ResetEnemies()
    {
        HumanNPCBehaviour[] listNPC = GameObject.FindObjectsByType<HumanNPCBehaviour>(FindObjectsSortMode.None);
        foreach (HumanNPCBehaviour npc in listNPC){
            npc.ResetSeePolterg();
        }
    }

    private IEnumerator WaitBeforeReset()
    {
        //print("RESET");
        yield return new WaitForSeconds(0.1f);
        foreach (IResetInitialState resetGameObject in currentCheckpoint.ResetGameObjects)
        {
            MonoBehaviour component = resetGameObject as MonoBehaviour; // Cast en MonoBehaviour
            if (component != null && component.gameObject.activeInHierarchy)
            {
                resetGameObject.ResetInitialState();
                
            }
            //print(component.gameObject.name);
            //else
            //{
            //    print(component.gameObject.name);
            //}

        }
        if (player != null)
        {
            player.transform.position = currentCheckpoint.transform.position;
            player.GetComponent<PlayerController>().canMove = true;
        }
        ResetEnemies();
        SuspicionManager.Instance.ResetSuspicion();
    }


}
