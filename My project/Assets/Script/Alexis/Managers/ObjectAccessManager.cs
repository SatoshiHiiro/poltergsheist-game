using UnityEngine;

public class ObjectAccessManager : InteractibleManager
{
    InventorySystem inventory;
    [SerializeField] KeyItemBehavior keyItemForActivation;
    Collider2D objCollider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        inventory = FindFirstObjectByType<InventorySystem>().GetComponent<InventorySystem>();
        objCollider = GetComponent<Collider2D>();
        InventorySystem.Instance.OnResetKey += ResetObjectCollider;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (InventorySystem.Instance.IsKeyPickedUp(keyItemForActivation))
        {
            // Animation lock open up
            objCollider.isTrigger = true;
        }
    }

    private void ResetObjectCollider(KeyItemBehavior key)
    {
        if(key == keyItemForActivation)
        {
            objCollider.isTrigger = false;
        }
    }

    private void OnDisable()
    {
        InventorySystem.Instance.OnResetKey -= ResetObjectCollider;
    }
}
