using System.Collections;
using UnityEngine;

public class KeyItemBehavior : PickupItemBehavior
{
    // This class manage the behavior of the key that can be collected by the player


    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        PossessionManager possessionManager = collision.GetComponent<PossessionManager>();
        if (collision.GetComponent<PlayerController>() != null || (possessionManager != null && possessionManager.IsPossessing))
        {
            InventorySystem.Instance.AddKeyToInventory(this);
        }
    }


}
