using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public abstract class PickupItemBehavior : MonoBehaviour
{
    protected bool isDestroy;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        isDestroy = false;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    protected virtual void LateUpdate()
    {
        if (isDestroy)
            Destroy(gameObject);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>() != null || collision.GetComponent<PossessionController>() != null)
            ItemDestruction();
    }

    protected void ItemDestruction()
    {
        isDestroy = true;
    }
}
