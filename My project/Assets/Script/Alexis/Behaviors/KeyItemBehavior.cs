using System.Collections;
using UnityEngine;

public class KeyItemBehavior : PickupItemBehavior, IResetInitialState
{
    // This class manage the behavior of the key that can be collected by the player

    protected override void Start()
    {
        base.Start();
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        if (collision.GetComponent<PlayerController>() != null || collision.GetComponent<PossessionController>() != null)
        {
            InventorySystem.Instance.StockItem(this, true);
            InventorySystem.Instance.CreateUIItem(itemSprite.sprite, this);
        }
    }

    public void ResetInitialState()
    {
        InventorySystem.Instance.RemoveObject(this);
        itemSprite.enabled = true;
        itemCollider.enabled = true;
    }
}
