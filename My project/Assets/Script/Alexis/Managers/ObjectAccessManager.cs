using UnityEngine;

public class ObjectAccessManager : InteractibleManager
{
    InventorySystem inventory;
    [SerializeField] KeyItemBehavior keyItemForActivation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        inventory = FindFirstObjectByType<InventorySystem>().GetComponent<InventorySystem>();
    }

    protected virtual void OnMouseDown()
    {
        if (inventory.ReadCondition(keyItemForActivation)[0] && inventory.ReadCondition(keyItemForActivation)[1])
        {
            GetComponent<Collider2D>().isTrigger = true;
        }
    }
}
