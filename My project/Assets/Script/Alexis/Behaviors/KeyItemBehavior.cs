using UnityEngine;

public class KeyItemBehavior : PickupItemBehavior
{
    InventorySystem inventory;
    [SerializeField] KeyObjectType _keyObjectType;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        inventory = FindFirstObjectByType<InventorySystem>();
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        if (collision.GetComponent<PlayerController>() != null || collision.GetComponent<PossessionController>() != null)
        {
            inventory.StockItem((int)_keyObjectType, true);
            inventory.CreateUIItem(gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite);
        }
    }
}
