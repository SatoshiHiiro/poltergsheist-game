using UnityEngine;

public class ObjectAccessManager : InteractibleManager
{
    InventorySystem inventory;
    [SerializeField] KeyObjectType _keyObjectType;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        inventory = FindFirstObjectByType<InventorySystem>().GetComponent<InventorySystem>();
    }

    protected virtual void OnMouseDown()
    {
        if (inventory.ReadCondition((int)_keyObjectType))
        {
            Debug.Log("in " + (int)_keyObjectType);
        }
    }
}
