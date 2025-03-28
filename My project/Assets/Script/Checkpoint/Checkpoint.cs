using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IResetInitialState
{
    void ResetInitialState();
}

public class Checkpoint : MonoBehaviour
{
    // This class manage the reset of objets and enemies when the player dies
    // for some specefic part of the game

    List<IResetInitialState> resetGameObject;   // All objects and enemies that must be reset when dying at this checkpoint

    // Getters
    public List<IResetInitialState> ResetGameObjects => resetGameObject;

    private void Start()
    {
        FindAllResetObjects();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>())
        {
            CheckpointManager.Instance.SetCheckPoint(this);
        }
    }

    // Find all the objects that must be reset when failling a challenge
    private void FindAllResetObjects()
    {
        resetGameObject = transform.parent.GetComponentsInChildren<IResetInitialState>(true).ToList();
        print(resetGameObject.Count);
    }
}
