using System.Collections;
using UnityEngine;

public class KeyItemBehavior : PickupItemBehavior
{
    // This class manage the behavior of the key that can be collected by the player


    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        if (collision.GetComponent<PlayerController>() != null || collision.GetComponent<PossessionController>() != null)
        {
            InventorySystem.Instance.AddKeyToInventory(this);
        }
    }


}
